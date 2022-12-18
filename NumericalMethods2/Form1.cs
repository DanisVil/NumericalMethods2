using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;


namespace NumericalMethods2
{
    public partial class Form1 : Form
    {
        private decimal a = 0.4m, b = 4, eps = 0.000001m;
        private int numNodes = 6;
        private int numOfPoints = 11;

        private decimal maxError = 0;

        private List<List<decimal>> points;
        private List<decimal> diffs = new List<decimal>();
        public Form1()
        {
            InitializeComponent();
            List<List<decimal>> me = new List<List<decimal>>();
            for (int i = 1; i <= 40; i++)
            {
                maxError = 0;
                numNodes = i;
                Process();
                me.Add(new List<decimal> { i, maxError });
            }
            ParseVector(me);
        }
        private void ParseVector(ObservablePoint[] v)
        {
            try
            {
                using(StreamWriter file = new StreamWriter("data.txt"))
                {
                    for (int i = 0; i < v.Length; i++)
                    {
                        file.WriteLine(v[i].X + "\t" + v[i].Y);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        private void ParseVector(List<List<decimal>> v)
        {
            try
            {
                using (StreamWriter file = new StreamWriter("data.txt"))
                {
                    for (int i = 0; i < v.Count; i++)
                    {
                        file.WriteLine(v[i][0] + "\t" + v[i][1]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        private void Process()
        {
            if (numNodes == 1) points = new List<List<decimal>> { new List<decimal> { a, Taylor(a) } };
            else points = TabulateChebishev(a, b, numNodes, eps);

            diffs.Clear();
            for (int i = 0; i < numNodes - 1; i++)
            {
                diffs.Add(DividedDiffs(points.GetRange(0, i + 2)));
            }

            decimal step = (b - a) / (numOfPoints - 1);

            var newton = new ObservablePoint[numOfPoints];
            var taylor = new ObservablePoint[numOfPoints];
            var lagrange = new ObservablePoint[numOfPoints];
            var error = new ObservablePoint[numOfPoints];
            maxError = 0;
            for (int i = 0; i < numOfPoints; i++)
            {
                decimal x = a + i * step;

                newton[i] = new ObservablePoint((double)x, (double)Newton(x, diffs, points));
                taylor[i] = new ObservablePoint((double)x, (double)Taylor(x));
                lagrange[i] = new ObservablePoint((double)x, (double)Lagrange(x, points));

                decimal err = Math.Abs(Lagrange(x, points) - Newton(x, diffs, points));
                error[i] = new ObservablePoint((double)x, (double)err);
                if (err > maxError) maxError = err;
            }

            label1.Text = numNodes.ToString();
            textBox1.Text = Math.Round(maxError, 9).ToString();

            cartesianChart1.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X;
            cartesianChart1.Series = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Ci(x)",
                    Values = taylor,
                    LineSmoothness = 0,
                    GeometrySize = 3
                },
                //new LineSeries<ObservablePoint>
                //{
                //    Name = "L_n(x)",
                //    Values = lagrange,
                //    LineSmoothness = 0,
                //    GeometrySize = 3
                //}
                //new LineSeries<ObservablePoint>
                //{
                //    Name = "Error",
                //    Values = error,
                //    LineSmoothness = 0,
                //    GeometrySize = 3
                //}
                new LineSeries<ObservablePoint>
                {
                    Name = "Newton",
                    Values = newton,
                    LineSmoothness = 0,
                    GeometrySize = 3
                }
            };
        }
        private decimal Taylor(decimal x, decimal accuracy = 0.000001m)
        {
            decimal a = -x * x / 4, sum = a;
            decimal n = 1;
            while (Math.Abs(a) >= accuracy)
            {
                a *= -0.5m * n * x * x / (2 * n + 1) / (n + 1) / (n + 1);
                sum += a;
                n++;
            }
            return sum;
        }
        private List<List<decimal>> Tabulate(decimal a, decimal b, int numOfPoints, decimal accuracy = 0.000001m)
        {
            if (numOfPoints < 2) throw new Exception();
            decimal step = (b - a) / (numOfPoints - 1);
            List<List<decimal>> points = new List<List<decimal>>();
            for (int i = 0; i < numOfPoints; i++)
            {
                points.Add(new List<decimal>() { a + i * step, Taylor(a + i * step, accuracy) });
            }
            return points;
        }
        private List<List<decimal>> TabulateChebishev(decimal a, decimal b, int numOfPoints, decimal accuracy = 0.000001m)
        {
            if (numOfPoints < 2) throw new Exception();
            List<List<decimal>> points = new List<List<decimal>>();
            for (int i = 0; i < numOfPoints; i++)
            {
                decimal x = (a + b) / 2 + (b - a) / 2 * (decimal)Math.Cos((2 * i + 1) * Math.PI / (2 * numOfPoints));
                points.Add(new List<decimal>() { x, Taylor(x, accuracy) });
            }
            return points;
        }
        private decimal Lagrange(decimal x, List<List<decimal>> points)
        {
            decimal sum = 0, product;
            for (int i = 0; i < points.Count; i++)
            {
                product = points[i][1];
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) continue;
                    product *= (x - points[j][0]) / (points[i][0] - points[j][0]);
                }
                sum += product;
            }
            return sum;
        }
        //private decimal RectrangleMethod(decimal a, decimal b, Foo f, decimal eps = 0.0001, decimal n = 100)
        //{
        //    decimal h = (b - a) / n;
        //    decimal sum = 0;
        //    for (int i = 0; i < n; i++)
        //    {
        //        decimal x = a + i * h;
        //        sum += h * f(x + h / 2);
        //    }

        //    h = (b - a) / 2 / n;
        //    decimal dsum = 0;
        //    for (int i = 0; i < 2 * n; i++)
        //    {
        //        decimal x = a + i * h;
        //        dsum += h * f(x + h / 2);
        //    }
        //    if (Math.Abs(sum - dsum) <= eps) return sum;
        //    return RectrangleMethod(a, b, f, eps, 2 * n);
        //}
        private decimal DividedDiffs(List<List<decimal>> points)
        {
            decimal sum = 0;
            for (int j = 0; j < points.Count; j++)
            {
                decimal prod = 1;
                for (int i = 0; i < points.Count; i++)
                {
                    if (j == i) continue;
                    prod *= (points[j][0] - points[i][0]);
                }
                sum += points[j][1] / prod;
            }
            return sum;
        }
        private decimal Newton(decimal x, List<decimal> diffs, List<List<decimal>> points)
        {
            decimal sum = points[0][1];
            decimal prod = 1;
            for (int i = 0; i < points.Count - 1; i++)
            {
                prod *= x - points[i][0];
                sum += prod * diffs[i];
            }
            return sum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numNodes++;
            Process();
        }
    }
}