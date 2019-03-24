using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;

namespace Laba_3
{
    public static class Functions
    {
        private static Random random = new Random();

        private static Dictionary<int, Color> colorDictionary = new Dictionary<int, Color>
        {
            {0, Color.Red },
            {1, Color.Black },
            {2, Color.Orange },
            {3, Color.Green },
            {4, Color.Blue },
            {5, Color.Pink },
            {6, Color.Yellow },
            {7, Color.Brown },
            {8, Color.Purple },
            {9, Color.Silver },
            {10, Color.Violet },
            {11, Color.Wheat }
        };

        public static string[] CreateGraph(Chart chart, Chart chartMD)
        {
            var n = 12;
            var W = 2400;
            var N = 1024;
            var t = 2;
            string[] xAxi = Enumerable.Range(0, N - t).Select(x => x.ToString()).ToArray();
            string[] mdXAxi = Enumerable.Range(0, N).Select(x => x.ToString()).ToArray();
     
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            double[][] harmonicsX = GenerateHarmonics(n, N, W);
            double[] combinedX = Combine(harmonicsX);

            double[][] harmonicsY = GenerateHarmonics(n, N, W);
            double[] combinedY = Combine(harmonicsY);

            var rxx = CalculateRxx(combinedX);
            var rxy = CalculateRxy(combinedX, combinedY);

            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed;

            PrintGraph(chart, xAxi, new double[][] { rxx });
            PrintGraph(chartMD, mdXAxi, new double[][] { rxy });

            return new[] { $"seconds: {elapsedTime.Seconds} milleseconds: {elapsedTime.Milliseconds}" };
        }

        private static void PrintGraph(Chart chart, string[] xAxi, double[][] harmonics)
        {
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.ChartAreas.Add(new ChartArea("Default"));

            for (var i = 0; i < harmonics.GetLength(0); i++)
            {
                var name = $"Harmonic_{i}";
                chart.Series.Add(new Series(name));

                chart.Series[name].ChartArea = "Default";
                chart.Series[name].ChartType = SeriesChartType.Line;

                chart.Series[name].Points.DataBindXY(xAxi, harmonics[i]);
                chart.Series[name].Color = colorDictionary[i];
            }
        }

        private static double[] GenerateSignal(int N, double W)
        {
            var A = random.NextDouble() * 2 + 2;
            var phi = random.NextDouble() * 2 * Math.PI;

            var result = new double[N];
            for (var i = 0; i < N; i++)
                result[i] = A * Math.Sin(W * i + phi);

            return result;
        }

        private static double CalculateM(double[] harmonic)
        {
            var M = harmonic.Sum();

            return M / harmonic.Length;
        }

        private static double CalculateD(double[] harmonic, double M)
        {
            var D = harmonic.Sum(x => Math.Pow(x - M, 2));

            return D / (harmonic.Length - 1);
        }

        private static double[][] GenerateHarmonics(int n, int N, double W)
        {
            var result = new double[n][];
            for (var i = 0; i < n; i++)
                result[i] = GenerateSignal(N, (W * (i + 1)) / n);

            return result;
        }

        private static double[] Combine(double[][] harmonics)
        {
            var result = new double[harmonics[0].Length];
            for (var i = 0; i < harmonics.GetLength(0); i++)
                for (var j = 0; j < result.Length; j++)
                    result[j] += harmonics[i][j];

            return result;
        }

        private static double[] CalculateRxx(double[] signal)
        {
            var t = 2;

            var mx = CalculateM(signal);

            double[] result = new double[signal.Length - t];

            for (var i = 0; i < signal.Length - t; i++)
            {
                result[i] = (signal[i] - mx) * (signal[i + t] - mx);
            }

            return result;
        }
        
        private static double[] CalculateRxy(double[] signalX, double[] signalY)
        {
            var t = signalX.Length;

            var newY = new double[signalX.Length * 2];
            Array.Copy(signalY, newY, signalY.Length);

            var mx = CalculateM(signalX);
            var my = CalculateM(newY);

            double[] result = new double[signalX.Length];

            for (var i = 0; i < newY.Length - t; i++)
            {
                result[i] = (signalX[i] - mx) * (newY[i + t] - my);
            }

            return result;
        }
    }
}
