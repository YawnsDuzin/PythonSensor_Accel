using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using Itlog.Util;

// namespace Itlog.Device
namespace Itlog
{
    public class AdvancedDbvCalibrator
    {
        private double a = 1.0; // 보정 스케일 기울기
        private double b = 0.0; // 보정 오프셋
        private double alpha = 0.2; // EMA 스무딩 계수
        private double? smoothedValue = null;
        private List<double> sensorDbvList = new List<double>();
        private List<double> referenceDbvList = new List<double>();

        // 보정값 추출 보정 및 평활 함수 (사용)
        public double GetCorrectedDbv(double rawDbv)
        {
            try
            {
                double corrected = a * rawDbv + b;
            if (smoothedValue == null) smoothedValue = corrected;
            else smoothedValue = alpha * corrected + (1 - alpha) * smoothedValue.Value;
            return smoothedValue.Value;
            }
            catch (Exception ex)
            { 
            //Log.LogWrite(3, "EGYRO", "❌ GetCorrectedDbv 오류:"+ ex.Message);
                return rawDbv; // 오류 시 원본 반환
            }
        }

        // 보정 파라미터 수동 확인
        public (double Scale, double Offset) GetCorrectionParameters()
        {
            try
            {
                return (a, b);
        }
        catch (Exception ex)
        {
                //Log.LogWrite(3, "EGYRO", "❌ GetParameters 오류: "+ex.Message);
            return (1.0, 0.0);
        }
}

        // 보정 수행: 수치 입력
        public void Calibrate(List<double> rawSensorDbvList, List<double> referenceDbvList)
        {
                try
                {
                    if (rawSensorDbvList.Count != referenceDbvList.Count || rawSensorDbvList.Count < 3)
                throw new ArgumentException("입력 데이터 수가 일치하지 않거나 부족합니다.");

            // 1. 중복 제거
            var uniquePairs = rawSensorDbvList
                .Zip(referenceDbvList, (sensor, reference) => new { sensor, reference })
                .Distinct()
                .ToList();

            var sensorList = uniquePairs.Select(p => p.sensor).ToList();
            var referenceList = uniquePairs.Select(p => p.reference).ToList();

            // 2. 이상값 제거 (IQR 방식)
            (sensorList, referenceList) = RemoveOutliers(sensorList, referenceList);

            // 3. 이동 평균 스무딩 적용 (moving average 3-point)
            sensorList = Smooth(sensorList);
            referenceList = Smooth(referenceList);

            // 4. 선형 회귀
            double avgSensor = sensorList.Average();
            double avgRef = referenceList.Average();

            double numerator = 0, denominator = 0;
            for (int i = 0; i < sensorList.Count; i++)
            {
                double xs = sensorList[i] - avgSensor;
                double ys = referenceList[i] - avgRef;
                numerator += xs * ys;
                denominator += xs * xs;
            }

            a = numerator / denominator;
            b = avgRef - a * avgSensor;
            }
            catch (Exception ex)
            { //Log.LogWrite(3, "EGYRO", "❌ Calibrate 오류:"+ex.Message);
             }
        }

        // IQR 기반 이상값 제거
        private (List<double>, List<double>) RemoveOutliers(List<double> sensor, List<double> reference)
        {
            try { 
            var q1 = Quantile(sensor, 0.25);
            var q3 = Quantile(sensor, 0.75);
            var iqr = q3 - q1;
            var lower = q1 - 1.5 * iqr;
            var upper = q3 + 1.5 * iqr;

            var filtered = sensor
                .Zip(reference, (s, r) => new { s, r })
                .Where(p => p.s >= lower && p.s <= upper)
                .ToList();

            return (
                filtered.Select(p => p.s).ToList(),
                filtered.Select(p => p.r).ToList()
            );
            }
            catch (Exception ex)
            {
                //Log.LogWrite(3, "EGYRO", "❌ RemoveOutliers 오류"+ex.Message);
                return (sensor, reference);
                 }
        }

        // 이동 평균 (3-point)
        private List<double> Smooth(List<double> data)
        {
            try { 
            if (data.Count < 3) return new List<double>(data);
            List<double> smoothed = new List<double>();
            smoothed.Add(data[0]);
            for (int i = 1; i < data.Count - 1; i++)
            {
                smoothed.Add((data[i - 1] + data[i] + data[i + 1]) / 3.0);
            }
            smoothed.Add(data[data.Count - 1]);
            return smoothed;
            }
            catch (Exception ex)
            { 
            //Log.LogWrite(3, "EGYRO", "❌ Smooth 오류: "+ex.Message);
                return data;
            }
        }

        // 사분위 계산
        private double Quantile(List<double> data, double percentile)
        {
            try { 
            var sorted = data.OrderBy(x => x).ToList();
            double pos = (sorted.Count - 1) * percentile;
            int lowerIndex = (int)Math.Floor(pos);
            int upperIndex = (int)Math.Ceiling(pos);
            if (lowerIndex == upperIndex) return sorted[lowerIndex];
            return sorted[lowerIndex] + (sorted[upperIndex] - sorted[lowerIndex]) * (pos - lowerIndex);
            }
            catch (Exception ex)
            { 
            //Log.LogWrite(3, "EGYRO", "❌ Quantile 오류:"+ex.Message);
                return 0;
            }
        }

        // 문자열로 보정 데이터 설정
        public bool DbvCalibrationSet(string sensorCsv, string refCsv)
        {
            bool bRtn = false;
            try
            {
                var sensorList = ParseStringToList(sensorCsv);
                var refList = ParseStringToList(refCsv);

                if (sensorList.Count != refList.Count)
                {
                    Console.WriteLine($"⚠️ 센서 데이터와 참조 데이터의 개수가 일치하지 않습니다. 센서: {sensorList.Count}, 참조: {refList.Count}");
                }
                else
                {
                    bRtn = true;
                }

                Calibrate(sensorList, refList);
                
                Console.WriteLine($"✅ 보정 완료: scale={a:F4}, offset={b:F4}");
                MessageBox.Show($"✅ 보정 완료: scale={a:F4}, offset={b:F4}");
            }
            catch(Exception ex)
            { 
                //Log.LogWrite(3, "EGYRO", "❌ DbvCalibrationSet 보정 중 오류 발생:" + ex.Message);
            }

            return bRtn;
        }

        // 2025.06.13 duzin - Updated to read from a single CSV file
        public bool DbvCalibrationSet2(string csvFilePath)
        {
            bool bRtn = false;
            var sensorList = new List<double>();
            var refList = new List<double>();

            try
            {
                // Check if file exists
                if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine($"❌ CSV 파일을 찾을 수 없습니다: {csvFilePath}");
                    return false;
                }

                // Read all lines from CSV file
                var lines = File.ReadAllLines(csvFilePath);
                if (lines.Length <= 1) // Need at least header + 1 data row
                {
                    Console.WriteLine($"⚠️ CSV 파일에 유효한 데이터가 없습니다: {csvFilePath}");
                    return false;
                }

                // Parse header to find column indices
                var headers = lines[0].Split(',');
                int hwtIndex = -1;
                int acoIndex = -1;

                for (int i = 0; i < headers.Length; i++)
                {
                    string header = headers[i].Trim();
                    if (header.Equals("HWT905", StringComparison.OrdinalIgnoreCase))
                        hwtIndex = i;
                    else if (header.Equals("ACO3233", StringComparison.OrdinalIgnoreCase))
                        acoIndex = i;
                }

                if (hwtIndex == -1 || acoIndex == -1)
                {
                    string missingColumns = "";
                    if (hwtIndex == -1) missingColumns += "HWT905, ";
                    if (acoIndex == -1) missingColumns += "ACO3233, ";
                    Console.WriteLine($"❌ CSV 파일에 필요한 컬럼이 없습니다: {missingColumns.TrimEnd(',', ' ')}");
                    return false;
                }

                // Parse data rows
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    var columns = lines[i].Split(',');
                    if (columns.Length > Math.Max(hwtIndex, acoIndex) &&
                        double.TryParse(columns[hwtIndex].Trim(), out double hwtValue) &&
                        double.TryParse(columns[acoIndex].Trim(), out double acoValue))
                    {
                        sensorList.Add(hwtValue);
                        refList.Add(acoValue);
                    }
                }

                if (sensorList.Count == 0 || refList.Count == 0)
                {
                    Console.WriteLine($"⚠️ 유효한 데이터가 없습니다. HWT905: {sensorList.Count}개, ACO3233: {refList.Count}개");
                    return false;
                }

                if (sensorList.Count != refList.Count)
                {
                    Console.WriteLine($"⚠️ HWT905과 ACO3233 데이터의 개수가 일치하지 않습니다. HWT905: {sensorList.Count}, ACO3233: {refList.Count}");
                    // Use the smaller count to avoid index out of range
                    int minCount = Math.Min(sensorList.Count, refList.Count);
                    sensorList = sensorList.Take(minCount).ToList();
                    refList = refList.Take(minCount).ToList();
                    Console.WriteLine($"⚠️ {minCount}개의 데이터로 보정을 진행합니다.");
                }

                bRtn = true;
                Calibrate(sensorList, refList);
                
                Console.WriteLine($"✅ 보정 완료: scale={a:F4}, offset={b:F4}");
                MessageBox.Show($"✅ 보정 완료: scale={a:F4}, offset={b:F4}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DbvCalibrationSet2 실행 중 오류 발생: {ex.Message}");
                //Log.LogWrite(3, "EGYRO", $"❌ DbvCalibrationSet2 실행 중 오류 발생: {ex.Message}");
            }

            return bRtn;
        }

        // 문자열 → List<double> 변환 함수
        List<double> ParseStringToList(string data)
        {
            try { 
            return data.Split(',')
                      .Select(s => double.TryParse(s.Trim(), out var val) ? val : double.NaN)
                      .Where(v => !double.IsNaN(v))
                      .ToList();
            }
            catch (Exception ex)
            { 
                //Log.LogWrite(3, "EGYRO", "❌ ParseStringToList 오류 발생:" + ex.Message);
                return new List<double>();
            }
        }
    }
}
