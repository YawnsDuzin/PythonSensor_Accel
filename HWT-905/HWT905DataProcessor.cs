using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Itlog
{
    // **추가 개선사항: 클래스 레벨에서 버퍼 관리 최적화**
    public class HWT905DataProcessor
    {
        private ItlogMain itlogMain;

        public double d_ref = 0;

        private ConstructionVibrationMeter vibrationMeter = new ConstructionVibrationMeter();

        private const int BUFFER_SIZE = 256; // FFT를 위한 버퍼 크기
        private byte[] RxBuffer = new byte[1000]; // 버퍼 크기 증가
        

        private byte[] tempBuffer = new byte[11]; // 재사용 가능한 임시 버퍼
        private ushort usRxLength = 0;
        private readonly object bufferLock = new object(); // 스레드 안전성을 위한 락

        public HWT905DataProcessor(ItlogMain main)
        {
            this.itlogMain = main;
        }

        public void HWT905DataGetProcessOptimized(SerialPort serial)
        {
            lock (bufferLock) // 스레드 안전성 보장
            {
                try
                {
                    if (serial?.IsOpen != true) return;

                    // 사용 가능한 모든 데이터를 한 번에 읽기
                    // - 수신 버퍼에 읽을 데이터가 있는지 확인합니다.
                    // - 데이터가 없으면 메서드를 종료합니다.
                    int availableBytes = serial.BytesToRead;
                    if (availableBytes == 0) return;

                    // 버퍼 오버플로우 방지
                    // - 읽을 수 있는 최대 데이터 크기를 계산합니다.
                    // - RxBuffer의 남은 공간과 수신 가능한 바이트 수 중 작은 값을 선택합니다.
                    // - 버퍼가 가득 찬 경우(maxReadSize <= 0) 로그를 남기고 버퍼를 리셋합니다.
                    int maxReadSize = Math.Min(availableBytes, RxBuffer.Length - usRxLength);
                    if (maxReadSize <= 0)
                    {
                        // 버퍼가 가득 찬 경우 처리
                        itlogMain.LogWrite("Buffer overflow detected, resetting buffer");
                        usRxLength = 0;
                        return;
                    }

                    // 데이터 읽기
                    // - 시리얼 포트로부터 데이터를 읽어 RxBuffer에 저장합니다.
                    // - usRxLength는 현재 버퍼에 저장된 데이터의 길이를 추적합니다.
                    int bytesRead = serial.Read(RxBuffer, usRxLength, maxReadSize);
                    usRxLength += (ushort)bytesRead;

                    // itlogMain.LogWrite($"Start - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - " + $"len:{bytesRead}, " + $"{BitConverter.ToString(RxBuffer).Replace("-", " ")}");
                    // itlogMain.LogWrite($"Start - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - " + $"len:{bytesRead}");

                    // 패킷 처리
                    ProcessPackets();
                }
                catch (Exception ex)
                {
                    itlogMain.LogWrite("HWT905DataGetProcessOptimized : " + ex.Message);
                }
            }
        }

        private void ProcessPackets()
        {
            int processedBytes = 0;

            // 패킷 처리 루프
            // - 버퍼에 처리할 충분한 데이터(최소 11바이트)가 남아있는 동안 반복합니다.
            // - usRxLength: 현재 버퍼에 저장된 전체 데이터 길이
            // - processedBytes: 현재까지 처리된 바이트 수
            while (usRxLength - processedBytes >= 11)
            {
                int currentPos = processedBytes;

                // 패킷 헤더 검증
                // - 0x55 0x51: 가속도계와 온도 데이터를 포함하는 패킷 헤더
                // - 헤더가 유효하지 않으면 1바이트씩 이동하며 유효한 헤더를 찾습니다.
                if (!((RxBuffer[currentPos] == 0x55) && (RxBuffer[currentPos + 1] == 0x51)))            // Acceleration + Temperature
                {
                    processedBytes++;
                    continue;
                }

                // 체크섬 검증
                // - 패킷의 무결성을 검증하기 위해 체크섬을 계산합니다.
                // - 처음 10바이트를 더한 값의 하위 8비트(& 0xff)를 11번째 바이트(체크섬)와 비교합니다.
                // - 체크섬이 일치할 때만 패킷을 유효한 것으로 간주합니다.
                byte checksum = 0;
                for (int i = 0; i < 10; i++)
                {
                    checksum += RxBuffer[currentPos + i];
                }
                // 체크섬 맞을때만 처리
                if ((checksum & 0xff) == RxBuffer[currentPos + 10])
                {
                    // 유효한 패킷 처리
                    // - 유효한 패킷을 임시 버퍼에 복사합니다.
                    Array.Copy(RxBuffer, currentPos, tempBuffer, 0, 11);
                    ProcessValidPacket(tempBuffer);
                }

                // 처리된 바이트 수 갱신
                // - 
                processedBytes += 11;
            }

            // 처리되지 않은 데이터를 버퍼 앞으로 이동
            // - 처리 된 버퍼 제거
            if (processedBytes > 0)
            {
                int remainingBytes = usRxLength - processedBytes;
                if (remainingBytes > 0)
                {
                    Array.Copy(RxBuffer, processedBytes, RxBuffer, 0, remainingBytes);
                }
                usRxLength = (ushort)remainingBytes;
            }
        }

        int idx = 0;
        StringBuilder _sb = new StringBuilder();

        private DateTime _lastSecond = DateTime.Now;
        private int _executionCount = 0;

        
        private List<double> accelerationBuffer = new List<double>();

        // HWT905 데이터 파싱 후 g값 추출
        double accelX_g = 0, accelY_g = 0, accelZ_g = 0;
        double totalAccel_g = 0;

        private void ProcessValidPacket(byte[] packet)
        {
            _executionCount++;

            accelX_g = BitConverter.ToInt16(packet, 2) / 32768.0 * 16;
            accelY_g = BitConverter.ToInt16(packet, 4) / 32768.0 * 16;
            accelZ_g = BitConverter.ToInt16(packet, 6) / 32768.0 * 16;

            // 합성 가속도 계산 (3축)
            totalAccel_g = Math.Sqrt(accelX_g * accelX_g + accelY_g * accelY_g + accelZ_g * accelZ_g);
            //totalAccel_g = Math.Sqrt(accelZ_g * accelZ_g);

            accelerationBuffer.Add(totalAccel_g);

            // 1초마다 실행 횟수 출력
            if (DateTime.Now.Subtract(_lastSecond).TotalSeconds >= 1)
            {
                itlogMain.LogWrite($"1초간 데이터 수신: {_executionCount}회 ({_executionCount}Hz)");
                _executionCount = 0;
                _lastSecond = DateTime.Now;
            }

            // 버퍼가 충분히 찼을 때 dBV 계산
            if (accelerationBuffer.Count >= BUFFER_SIZE)
            {
                var result = vibrationMeter.AnalyzeVibration(accelerationBuffer.ToArray(), d_ref);
                string level = vibrationMeter.GetVibrationLevel(result.VelocityRMS_MMS);

                itlogMain.LogWrite($"가속도: {result.AccelRMS_G:F4} g");
                itlogMain.LogWrite($"진동속도: {result.VelocityRMS_MMS:F3} mm/s");
                itlogMain.LogWrite($"dB(V): {result.DBV:F1} dB");
                itlogMain.LogWrite($"진동등급: {level}");

                itlogMain.LabelSet("lblAccelRMS_G", result.AccelRMS_G.ToString("F4"));
                itlogMain.LabelSet("lblVelocityRMS_MMS", result.VelocityRMS_MMS.ToString("F3"));
                itlogMain.LabelSet("lblRMS", result.VelocityRMS_MS.ToString());
                itlogMain.LabelSet("lblDBV", result.DBV.ToString("F1"));
                itlogMain.LabelSet("lblDBV_lv", level);

                // 버퍼의 절반만 유지 (오버랩)
                // accelerationBuffer.RemoveRange(0, BUFFER_SIZE / 2);
                accelerationBuffer.RemoveRange(0, 50);
                // accelerationBuffer.RemoveRange(0, 100);
                //accelerationBuffer.RemoveRange(0, 150);
                //accelerationBuffer.RemoveRange(0, 200);
            }


            //_sb.Append($" - {DateTime.Now:yyyy-MM-dd HH:mm:  ss  fff} - " + $"len:{packet.Length}, " + $"{BitConverter.ToString(packet).Replace("-", " ")}\r\n");
            //idx++;

            //if (idx > 200)
            //{
            //    itlogMain.LogWrite(_sb.ToString());

            //    idx = 0;
            //}

            // itlogMain.LogWrite($" - {DateTime.Now:yyyy-MM-dd HH:mm:  ss  fff} - " + $"len:{packet.Length}, " + $"{BitConverter.ToString(packet).Replace("-", " ")}");

            // 실제 데이터 처리 로직
            // HWT905DecodeData(packet);
            // DataReceive_Log(packet);
        }

    }
}
