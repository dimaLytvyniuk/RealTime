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

        public static string CreateGraph(Chart chart, Chart chartMD, DataGrid dataGrid)
        {
            var n = 12;
            var W = 2400;
            var N = 1024;
            string[] xAxi = Enumerable.Range(0, N).Select(x => x.ToString()).ToArray();
            string[] mdXAxi = Enumerable.Range(0, n).Select(x => x.ToString()).ToArray();

            Stopwatch stopwatch = new Stopwatch();
            var stringXAxi = new string[10];

            var forGraphTable = new double[2][]
            {
                new double[10],
                new double[10]
            };

            for (var newN = 400; newN <= 4000; newN += 400)
            {
                double[][] harmonics = GenerateHarmonics(n, newN, W);
                double[] combined = Combine(harmonics);

                stopwatch.Restart();
                var fpWithOutTable = CalculateFpWithTable(combined);
                stopwatch.Stop();
                forGraphTable[0][newN / 400 - 1] = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
                var fpWithTable = CalculateQuickFp(combined);
                stopwatch.Stop();
                forGraphTable[1][newN / 400 - 1] = stopwatch.ElapsedMilliseconds;

                stringXAxi[newN / 400 - 1] = newN.ToString();
            }
            PrintGraph(chart, stringXAxi, forGraphTable);

            //PrintGraph(chart, xAxi, new double[][] { combined });
            //PrintGraph(chartMD, xAxi, new double[][] { fpWithTables });

            return $"";
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

        private static void FillTimeTable(DataGrid dataGrid, string[][] timeLines)
        {
            dataGrid.Items.Clear();

            var n = timeLines[0].Length;
            
            for (int i = 0; i < n; i++)
            {
                var col = new DataGridTextColumn();
                col.Header = $"{(i + 1) * 400}";
                col.Binding = new Binding(string.Format("[{0}]", i));
                dataGrid.Columns.Add(col);
            }

            for (var i = 0; i < timeLines.GetLength(0); i++)
                dataGrid.Items.Add(timeLines[i]);
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

        private static double[] CalculateFp(double[] signal)
        {
            var result = new double[signal.Length];

            for (var i = 0; i < signal.Length; i++)
            {
                double Re = 0;
                double Im = 0;
                for (var j = 0; j < signal.Length; j++)
                {
                    Re += signal[j] * Math.Cos(2 * Math.PI * i * j / signal.Length);
                    Im += signal[j] * Math.Sin(2 * Math.PI * i * j / signal.Length);
                }
                result[i] = Math.Sqrt(Math.Pow(Re, 2) + Math.Pow(Im, 2));
            }

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
            var N = signal.Length / 2 - 1;

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

        private static Tuple<double, double>[] CalculateFIIp(double[] signal, Tuple<double, double>[] WTableN2)
        {
            var N = signal.Length;
            var result = new Tuple<double, double>[N];

            for (var i = 0; i < signal.Length; i++)
            {
                double Re = 0;
                double Im = 0;
                for (var j = 0; j < N; j+= 2)
                {
                    var w = WTableN2[(i * j / 2) % (N / 2)];
                    Re += signal[j] * w.Item1;
                    Im += signal[j] * w.Item2;
                }
                result[i] = Tuple.Create(Re, Im);
            }

            return result;
        }

        private static Tuple<double, double>[] CalculateFIp(double[] signal, Tuple<double, double>[] WTableN2, Tuple<double, double>[] WTableN)
        {
            var N = signal.Length;
            var result = new Tuple<double, double>[N];

            for (var i = 0; i < signal.Length; i++)
            {
                double Re = 0;
                double Im = 0;
                for (var j = 1; j < N; j+= 2)
                {
                    var w = WTableN2[(i * (j - 1) / 2) % (N / 2)];
                    Re += signal[j] * w.Item1;
                    Im += signal[j] * w.Item2;
                }
                var wP = WTableN[i];
                result[i] = Tuple.Create(Re * wP.Item1, Im * wP.Item2);
            }

            return result;
        }

        private static double[] CalculateQuickFp(double[] signal)
        {
            var result = new double[signal.Length];
            var wTableN2 = GenerateWTable(signal.Length / 2);
            var wTableN = GenerateWTable(signal.Length);

            var fpsTasks = new Task<Tuple<double, double>[]>[2];

            fpsTasks[0] = Task.Factory.StartNew(() => CalculateFIp(signal, wTableN2, wTableN));
            fpsTasks[1] = Task.Factory.StartNew(() => CalculateFIIp(signal, wTableN2));
            Task.WhenAll(fpsTasks);

            var fpI = fpsTasks[0].Result;
            var fpII = fpsTasks[1].Result;

            for (var i = 0; i < signal.Length; i++)
            {
                double Re = fpI[i].Item1 + fpII[i].Item1;
                double Im = fpI[i].Item2 + fpII[i].Item2;
                result[i] = Math.Sqrt(Math.Pow(Re, 2) + Math.Pow(Im, 2));
            }

            return result;
        }
    }
}
