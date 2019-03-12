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

        public static string[] CreateGraph(Chart chart, Chart chartMD, DataGrid dataGrid, DataGrid dataGridMD)
        {
            var n = 12;
            var W = 2400;
            var N = 1024;
            string[] xAxi = Enumerable.Range(0, N).Select(x => x.ToString()).ToArray();
            string[] mdXAxi = Enumerable.Range(0, n).Select(x => x.ToString()).ToArray();
     
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            double[][] harmonics = GenerateHarmonics(n, N, W);
            double[] combined = Combine(harmonics);

            var m = harmonics.Select(x => CalculateM(x)).ToArray();
            var d = harmonics.Select((x, i) => CalculateD(x, m[i])).ToArray();

            var mx = CalculateM(combined);
            var dx = CalculateD(combined, mx);

            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed;

            PrintGraph(chart, xAxi, harmonics);
            FillTable(dataGrid, harmonics);

            PrintGraph(chartMD, xAxi, new double[][] { combined });
            FillMDTable(dataGridMD, new double[][] { m, d });

            return new[] { $"seconds: {elapsedTime.Seconds} milleseconds: {elapsedTime.Milliseconds}", mx.ToString(), dx.ToString() };
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

        private static void FillTable(DataGrid dataGrid, double[][] harmonics)
        {
            dataGrid.Items.Clear();

            for (int i = 0; i < harmonics[0].Length; i++)
            {
                var col = new DataGridTextColumn();
                col.Header = $"X(t{i})";
                col.Binding = new Binding(string.Format("[{0}]", i));
                dataGrid.Columns.Add(col);
            }

            for (var i = 0; i < harmonics.GetLength(0); i++)
                dataGrid.Items.Add(harmonics[i]);
        }

        private static void FillMDTable(DataGrid dataGrid, double[][] data)
        {
            dataGrid.Items.Clear();

            for (int i = 0; i < data[0].Length; i++)
            {
                var col = new DataGridTextColumn();
                col.Header = $"n{i}";
                col.Binding = new Binding(string.Format("[{0}]", i));
                dataGrid.Columns.Add(col);
            }

            for (var i = 0; i < data.GetLength(0); i++)
                dataGrid.Items.Add(data[i]);
        }

        private static double[] GenerateSignal(int N, double W)
        {
            var random = new Random();
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
    }
}
