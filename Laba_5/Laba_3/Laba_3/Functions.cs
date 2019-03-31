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

        public static void CreateCalculation(int N)
        {
            var n = 12;
            var W = 2400;
        
            double[][] harmonics = GenerateHarmonics(n, N, W);
            double[] combined = Combine(harmonics);

            var mx = CalculateM(combined);
            var dx = CalculateD(combined, mx);

            var fp = CalculateFpWithTable(combined);
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

        private static Tuple<double, double>[] GenerateWTable(int N)
        {
            var result = new Tuple<double, double>[N];
            for (var i = 0; i < N; i++)
                result[i] = Tuple.Create(Math.Cos(2 * Math.PI * i / N), Math.Sin(2 * Math.PI * i / N));

            return result;
        }

        private static double[] CalculateFpWithTable(double[] signal)
        {
            var result = new double[signal.Length];
            var wTable = GenerateWTable(signal.Length);
            var N = signal.Length;

            for (var i = 0; i < signal.Length; i++)
            {
                double Re = 0;
                double Im = 0;
                for (var j = 0; j < signal.Length; j++)
                {
                    var w = wTable[(i * j) % N];
                    Re += signal[j] * w.Item1;
                    Im += signal[j] * w.Item2;
                }
                result[i] = Math.Sqrt(Math.Pow(Re, 2) + Math.Pow(Im, 2));
            }

            return result;
        }
    }
}
