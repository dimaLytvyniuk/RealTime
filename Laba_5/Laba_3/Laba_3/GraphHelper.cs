using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Laba_3
{
    public class GraphHelper
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

        public static void PrintGraph(Chart chart, string[] xAxi, int[][] harmonics)
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
    }
}
