using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Itlog
{
    public partial class ItlogMain : Form
    {
        // 2025.06.20 duzin
        private readonly HWT905DataProcessor HWT905_DP;

        private AdvancedDbvCalibrator adc = new AdvancedDbvCalibrator();

        // 2025.06.10 duzin
        // public static string g_sGyroBaud = "9600";
        // public static string g_sGyroBaud = "19200";
        public static string g_sGyroBaud = "38400";

        // 2025.05.33 duzin
        // zedGraph_dbv
        private GraphPane myPane_dbv;
        private RollingPointPairList xList_dbv;
        private RollingPointPairList yList_dbv;
        private RollingPointPairList zList_dbv;
        private RollingPointPairList rmsList_dbv;
        private RollingPointPairList dbvList_dbv;
        private LineItem xCurve_dbv;
        private LineItem yCurve_dbv;
        private LineItem zCurve_dbv;
        private LineItem rmsCurve_dbv;
        private LineItem dbvCurve_dbv;
        private int maxPoints_dbv = 500;
        private int idx_dbv = 0;
        // zedGraph_var
        private GraphPane myPane_var;
        private RollingPointPairList valList_Xvar;
        private RollingPointPairList valList_Yvar;
        private RollingPointPairList valList_Zvar;
        private LineItem valCurve_Xvar;
        private LineItem valCurve_Yvar;
        private LineItem valCurve_Zvar;
        private int maxPoints_var = 500;
        private int idx_var = 0;


        // 2025.05.22 duzin
        // 임계값 초과 시, 경광등??
        private System.Windows.Forms.Timer blinkTimer;
        private bool isBlinking = false;
        private int blinkCount = 0;
        private const int BLINK_DURATION = 10000; // 10초
        private const int BLINK_INTERVAL = 500;   // 0.5초 간격으로 반짝임

        private Image alarmOnImage;
        private Image alarmOffImage;


        public ItlogMain()
        {
            InitializeComponent();

            //string HWT905 = "48.5,85.6,50.4,58.6,47.7,41.2,102,48.1,46.7,73.7,47.4,48.1";
            //string ACO3233 = "43.5,94.6,71.7,85.7,43.7,69.4,109.4,62.3,43.9,90.7,43.4,58.7";

            //adc.DbvCalibrationSet(HWT905, ACO3233);
            //// adc.DbvCalibrationSet2("C:\\_dzP\\Project\\sensor_new\\_opencv_ocr\\Source\\Anaysis\\monitoring_data_20250613_0.7.csv");

            // 2025.06.10 duzin
            HWT905_DP = new HWT905DataProcessor(this);
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                // 수신된 데이터 처리
                if (serialPort1.IsOpen)
                {
                    // 2025.06.10 duzin
                    // _hwt905.HWT905DataGetProcess(serialPort1);
                    HWT905_DP.HWT905DataGetProcessOptimized(serialPort1);
                }
            }
            catch (TimeoutException ex)
            {
                // 타임아웃 발생 시 처리
                Invoke(new Action(() =>
                {
                    textBox1.AppendText("타임아웃 오류: " + ex.Message + Environment.NewLine);
                }));
            }
            catch (IOException ex)
            {
                // 포트가 닫혔거나 연결이 끊긴 경우
                Invoke(new Action(() =>
                {
                    textBox1.AppendText("시리얼 포트 오류: " + ex.Message + Environment.NewLine);
                }));
            }
            catch (InvalidOperationException ex)
            {
                // 포트가 이미 닫힌 경우
                Invoke(new Action(() =>
                {
                    textBox1.AppendText("포트가 닫혀 있습니다: " + ex.Message + Environment.NewLine);
                }));
            }
        }

        private void PortInit()
        {
            try
            {
                string[] sarrPorts = System.IO.Ports.SerialPort.GetPortNames();
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(sarrPorts);
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;

                // 2025.06.10 duzin
                cmbBaudRate.Text = "19200";
                g_sGyroBaud = cmbBaudRate.Text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        private void ItlogMain_Load(object sender, EventArgs e)
        {
            PortInit();
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            serialPort1.DataReceived += serialPort1_DataReceived;
            button2.Text = "열기";
        }

        private void ItlogMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void SerialOpenClose()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DataReceived -= serialPort1_DataReceived; // 이벤트 핸들러 제거
                serialPort1.Close();
                button2.Text = "열기";

                serialPort1.DataReceived += serialPort1_DataReceived; // 필요시 다시 등록
            }
            else
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = int.Parse(g_sGyroBaud);
                serialPort1.DataReceived += serialPort1_DataReceived; // 이벤트 핸들러 등록
                serialPort1.Open();
                button2.Text = "닫기";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            PortInit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //_hwt905.bRealAll = true;

            HWT905_DP.d_ref = Convert.ToDouble(txt_d_ref.Text);

            g_sGyroBaud = cmbBaudRate.Text;
            SerialOpenClose();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //_hwt905.bRealAll = false;
            SerialOpenClose();
        }


        public void TextSet(string textBoxName, string msg)
        {
            Control[] controls = this.Controls.Find(textBoxName, true);
            if (controls.Length > 0 && controls[0] is TextBox textBox)
            {
                textBox.Invoke(new Action(() => textBox.Text = msg));
            }
        }

        public void LabelSet(string labelName, string msg)
        {
            Control[] controls = this.Controls.Find(labelName, true);
            if (controls.Length > 0 && controls[0] is System.Windows.Forms.Label label)
            {
                label.Invoke(new Action(() => label.Text = msg));
            }
        }


        private void LogConsole(string msg)
        {
            // 로그 출력용 메서드
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}");
        }


        public void LogWrite(string data)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    textBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] "+ data + Environment.NewLine); // 수신된 데이터를 텍스트박스에 출력
                }));

            }
            catch (Exception ex)
            {
                // 예외 처리
                MessageBox.Show("로그 기록 중 오류 발생: " + ex.Message);
            }
        }

        private void btnSensorSet_br_Click(object sender, EventArgs e)
        {
            try
            {
                // 변경하려는 Baud Rate
                int newBaudRate = 19200;

                // 1. 잠금 해제 명령 전송 (FF AA 69 88 B5)
                byte[] unlockCommand = { 0xFF, 0xAA, 0x69, 0x88, 0xB5 };
                serialPort1.Write(unlockCommand, 0, unlockCommand.Length);
                Console.WriteLine("잠금 해제 명령 전송: FF AA 69 88 B5");
                Thread.Sleep(100); // 센서 처리 대기 (필요에 따라 조정)

                // 2. Baud Rate 설정 (19200bps = 0x0300) 명령 전송 (FF AA 04 03 00)
                byte[] setBaudRateCommand = { 0xFF, 0xAA, 0x04, 0x03, 0x00 };
                serialPort1.Write(setBaudRateCommand, 0, setBaudRateCommand.Length);
                Console.WriteLine($"Baud Rate {newBaudRate}bps 설정 명령 전송: FF AA 04 03 00");
                Thread.Sleep(100); // 센서 처리 대기

                // 5. 설정 저장 명령 전송 (FF AA 00 00 00)
                byte[] saveCommand = { 0xFF, 0xAA, 0x00, 0x00, 0x00 };
                serialPort1.Write(saveCommand, 0, saveCommand.Length);
                Console.WriteLine("설정 저장 명령 전송: FF AA 00 00 00");
                Thread.Sleep(500); // 저장 시간 대기 (충분히 기다리는 것이 좋음)

                Console.WriteLine("설정 변경 명령 시퀀스 전송 완료.");
                Console.WriteLine($"센서의 Baud Rate가 {newBaudRate}bps로 변경되었습니다.");
                Console.WriteLine("시리얼 포트 연결을 종료하고, 다시 연결 시 Baud Rate를 19200bps로 설정해야 합니다.");

                MessageBox.Show($"센서 설정이 완료되었습니다.\nBaud Rate: {newBaudRate}bps\n다시 연결 시 Baud Rate를 19200bps로 설정하세요.", "센서 설정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                // 예외 처리
                MessageBox.Show("센서 설정 중 오류 발생: " + Ex.Message);
                Console.WriteLine("센서 설정 중 오류 발생: " + Ex.Message);
            }
        }

        // 2025.06.10 duzin
        private void btnSensorSet_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 잠금 해제 명령 전송 (FF AA 69 88 B5)
                byte[] unlockCommand = { 0xFF, 0xAA, 0x69, 0x88, 0xB5 };
                serialPort1.Write(unlockCommand, 0, unlockCommand.Length);
                Console.WriteLine("잠금 해제 명령 전송: FF AA 69 88 B5");
                Thread.Sleep(100); // 센서 처리 대기 (필요에 따라 조정)

                // 3. Output Content (Acc Only = 0x0002) 설정 명령 전송 (FF AA 02 02 00)
                // Bit 0: Time (0)
                // Bit 1: Acc (1)
                // Bit 2: Gyro (0) - assumed off
                // Bit 3: Angle (0)
                // Bit 4: Mag (0)
                // Bit 5: Port (0)
                // Bit 6: Press (0) - assumed off
                // Bit 7: GPS (0) - assumed off
                // Bit 8: Velocity (0)
                // Bit 9: Quater (0) - assumed off
                // Bit 10: GSA (0) - assumed off
                // 결과 값: 0x0002
                byte[] setOutputContentCommand = { 0xFF, 0xAA, 0x02, 0x02, 0x00 };
                serialPort1.Write(setOutputContentCommand, 0, setOutputContentCommand.Length);
                Console.WriteLine("Output Content (Acc Only) 설정 명령 전송: FF AA 02 02 00");
                Thread.Sleep(100); // 센서 처리 대기

                // 4. Output Rate (100Hz = 0x0009) 설정 명령 전송 (FF AA 03 09 00) [5, 6]
                // RRATE 레지스터(0x03) 하위 4비트(RRATE[3:0])에 0x09 (100Hz) 설정 [5]
                byte[] setOutputRateCommand = { 0xFF, 0xAA, 0x03, 0x09, 0x00 };
                serialPort1.Write(setOutputRateCommand, 0, setOutputRateCommand.Length);
                Console.WriteLine("4. Output Rate (100Hz) 설정 명령 전송: FF AA 03 09 00");
                Thread.Sleep(100); // 센서 처리 대기 (10S 이내 유지 중요 [1, 2])

                // 5. 설정 저장 명령 전송 (FF AA 00 00 00)
                byte[] saveCommand = { 0xFF, 0xAA, 0x00, 0x00, 0x00 };
                serialPort1.Write(saveCommand, 0, saveCommand.Length);
                Console.WriteLine("설정 저장 명령 전송: FF AA 00 00 00");
                Thread.Sleep(500); // 저장 시간 대기 (충분히 기다리는 것이 좋음)

                Console.WriteLine("설정 변경 명령 시퀀스 전송 완료.");
                Console.WriteLine("시리얼 포트 연결을 종료하고, 다시 연결 시 Baud Rate를 19200bps로 설정해야 합니다.");

                MessageBox.Show($"센서 설정이 완료되었습니다.", "센서 설정 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                // 예외 처리
                MessageBox.Show("센서 설정 중 오류 발생: " + Ex.Message);
                Console.WriteLine("센서 설정 중 오류 발생: " + Ex.Message);
            }
        }

        
    }
}