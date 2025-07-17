using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itlog
{
    public class ConstructionVibrationMeter
    {
        // 2025.06.10 duzin
        //private const double SAMPLING_RATE = 201.5; // Hz
        private const double SAMPLING_RATE = 100; // Hz

        private const double G_TO_MS2 = 9.81; // 중력가속도

        public class VibrationResult
        {
            public double AccelRMS_G { get; set; }
            public double VelocityRMS_MS { get; set; } // m/s       // 2025.06.16 duzin            
            public double VelocityRMS_MMS { get; set; } // mm/s
            public double VelocityRMS_CMS { get; set; } // cm/s
            public double DBV { get; set; } // dB(V)
        }

        public VibrationResult AnalyzeVibration(double[] accelerationData_g, double d_ref)
        {
            // 0. DC 성분(중력) 제거 - 평균값 빼기
            double[] accelData_noDC = RemoveDCComponent(accelerationData_g);

        //// 2025.06.10 duzin
        //string sTemp = "C:\\_dzP\\Project\\sensor\\_logs\\ArrayData_log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        //SaveArrayDataToCsv(sTemp, accelerationData_g, accelData_noDC, mean);


            // 1. 가속도 RMS 계산 (g 단위)
            double accelRMS_g = CalculateRMS(accelData_noDC);

            // 3. m/s²로 변환
            double[] accelData_ms2 = accelData_noDC.Select(x => x * G_TO_MS2).ToArray();

            // 4. 고역통과 필터 적용 (추가 노이즈 제거)
            double[] filteredAccel = HighPassFilter(accelData_ms2, 1.0); // 1Hz 이하 제거

            // 5. 적분하여 속도로 변환
            double[] velocity_ms = IntegrateAcceleration(filteredAccel);

            // 4. 속도 RMS 계산
            double velocityRMS_ms = CalculateRMS(velocity_ms);
            double velocityRMS_mms = velocityRMS_ms * 1000; // mm/s로 변환
            double velocityRMS_cms = velocityRMS_ms * 100;  // cm/s로 변환

            // 5. dB(V) 계산 - 진동속도 기준
            // 참조값: 1×10⁻⁸ m/s (일반적 기준) 또는 1×10⁻⁹ m/s
            // double dbV = 20 * Math.Log10(Math.Max(velocityRMS_ms / 1e-8, 1e-10)); // 최소값 제한
            double dbV = 20 * Math.Log10(Math.Max(velocityRMS_ms / d_ref, 1e-10)); // 최소값 제한

            return new VibrationResult
            {
                AccelRMS_G = accelRMS_g,
                VelocityRMS_MS = velocityRMS_ms,
                VelocityRMS_MMS = velocityRMS_mms,
                VelocityRMS_CMS = velocityRMS_cms,
                DBV = dbV
            };
        }


        private void SaveArrayDataToCsv(string filePath,
                               double[] accelerationData_g,
                               double[] accelData_noDC,
                               double mean)
                                // ,DateTime startTime)
        {
            using (var writer = new StreamWriter(filePath, append: false, Encoding.UTF8))
            {
                // 헤더 작성
                writer.WriteLine("Index, AccelData_G, AccelData_NoDC, mean");

                // 데이터 길이 확인 (두 배열이 같은 길이여야 함)
                int dataLength = Math.Min(accelerationData_g.Length, accelData_noDC.Length);

                // 샘플링 간격 계산 (201.5Hz 기준)
                //double timeInterval = 1.0 / 201.5; // 초 단위

                for (int i = 0; i < dataLength; i++)
                {
                    // 각 샘플의 시간 계산
                    //DateTime sampleTime = startTime.AddSeconds(i * timeInterval);
                    //double timeDiff_ms = i * timeInterval * 1000; // ms 단위

                    writer.WriteLine($"{i}," +
                                   //$"{sampleTime:yyyy-MM-dd HH:mm:ss.fff}," +
                                   $"{accelerationData_g[i]:F6}," +
                                   $"{accelData_noDC[i]:F6}," +
                                   $"{mean}");
                //," +
                //               $"{timeDiff_ms:F3}");
            }
            }
        }


        double mean = 0;
        // DC 성분(중력) 제거
        private double[] RemoveDCComponent(double[] data)
        {
            mean = data.Average();
            return data.Select(x => x - mean).ToArray();
        }

        private double CalculateRMS(double[] data)
        {
            if (data.Length == 0) return 0;

            double sumOfSquares = data.Sum(x => x * x);
            return Math.Sqrt(sumOfSquares / data.Length);
        }

        // 개선된 적분 (트래페지오이드 룰)
        private double[] IntegrateAcceleration(double[] acceleration)
        {
            double[] velocity = new double[acceleration.Length];
            double dt = 1.0 / SAMPLING_RATE;

            velocity[0] = 0;
            for (int i = 1; i < acceleration.Length; i++)
            {
                // 트래페지오이드 적분
                velocity[i] = velocity[i - 1] + (acceleration[i - 1] + acceleration[i]) * dt / 2.0;
            }

            // 적분 후 다시 DC 제거 (적분 드리프트 방지)
            return RemoveDCComponent(velocity);
        }

        // 개선된 고역통과 필터
        private double[] HighPassFilter(double[] data, double cutoffFreq)
        {
            double alpha = Math.Exp(-2 * Math.PI * cutoffFreq / SAMPLING_RATE);
            double[] filtered = new double[data.Length];

            filtered[0] = 0; // 첫 번째 값은 0으로
            for (int i = 1; i < data.Length; i++)
            {
                filtered[i] = alpha * (filtered[i - 1] + data[i] - data[i - 1]);
            }

            return filtered;
        }

        // 진동 등급 판정
        public string GetVibrationLevel(double velocityRMS_mms)
        {
            if (velocityRMS_mms < 0.15) return "무감지";
            else if (velocityRMS_mms < 0.3) return "약간감지";
            else if (velocityRMS_mms < 1.0) return "감지";
            else if (velocityRMS_mms < 2.0) return "강하게감지";
            else if (velocityRMS_mms < 5.0) return "매우강하게감지";
            else return "손상위험";
        }
    }
}
