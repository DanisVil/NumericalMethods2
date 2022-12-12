using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;


namespace NumericalMethods2
{
    delegate double Foo(double x);
    public partial class Form1 : Form
    {
        private double a = 0.4, b = 4, eps = 0.000001;
        private int numOfPoints = 3;
        private int coeff = 2;
        public Form1()
        {
            InitializeComponent();
            Process();
        }
        private void Process()
        {
            label1.Text = numOfPoints.ToString();
            double a = 0.4, b = 4, eps = 0.000001;
            List<List<double>> points = Tabulate(a, b, numOfPoints, eps);

            var taylor = new ObservablePoint[numOfPoints];
            for (int i = 0; i < numOfPoints; i++)
            {
                taylor[i] = new ObservablePoint(points[i][0], points[i][1]);
            }

            var lagrange = new ObservablePoint[coeff * numOfPoints];
            for (int i = 0; i <= coeff * (numOfPoints - 1); i++)
            {
                double step = (b - a) / coeff / (numOfPoints - 1);
                double x = a + i * step;
                lagrange[i] = new ObservablePoint(x, Lagrange(x, points));
            }

            List<double> diffs = new List<double>();
            for (int i = 0; i < numOfPoints - 1; i++)
            {
                diffs.Add(DividedDiffs(points.GetRange(0, i + 2)));
            }
            var newton = new ObservablePoint[coeff * numOfPoints];
            for (int i = 0; i <= coeff * (numOfPoints - 1); i++)
            {
                double step = (b - a) / coeff / (numOfPoints - 1);
                double x = a + i * step;
                newton[i] = new ObservablePoint(x, Newton(x, diffs, points));
            }

            var erf = new ObservablePoint[coeff * numOfPoints];
            for (int i = 0; i <= coeff * (numOfPoints - 1); i++)
            {
                double step = (b - a) / coeff / (numOfPoints - 1);
                double x = a + i * step;
                erf[i] = new ObservablePoint(x, Erf(Lagrange(x, points)));
            }

            var error = new ObservablePoint[coeff * numOfPoints];
            double maxError = 0;
            for (int i = 0; i <= coeff * (numOfPoints - 1); i++)
            {
                double step = (b - a) / coeff / (numOfPoints - 1), x = a + i * step;
                double err = Math.Abs(Taylor(x, eps) - Lagrange(x, points));
                error[i] = new ObservablePoint(x, err);
                if (err > maxError) maxError = err;
            }
            textBox1.Text = Math.Round(maxError, 9).ToString();

            cartesianChart1.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X;
            cartesianChart1.Series = new ISeries[]
            {
                //new LineSeries<ObservablePoint>
                //{
                //    Name = "Ci(x)",
                //    Values = taylor,
                //    LineSmoothness = 0,
                //    GeometrySize = 3
                //},
                new LineSeries<ObservablePoint>
                {
                    Name = "L_n(x)",
                    Values = lagrange,
                    LineSmoothness = 0,
                    GeometrySize = 3
                },
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
                //new LineSeries<ObservablePoint>
                //{
                //    Name = "Erf",
                //    Values = erf,
                //    LineSmoothness = 0,
                //    GeometrySize = 3
                //}
            };
        }
        private double Taylor(double x, double accuracy = 0.000001)
        {
            double a = -x * x / 4, sum = a;
            double n = 1;
            while (Math.Abs(a) >= accuracy)
            {
                a *= -0.5 * n * x * x / (2 * n + 1) / (n + 1) / (n + 1);
                sum += a;
                n++;
            }
            return sum;
        }
        private List<List<double>> Tabulate(double a, double b, int numOfPoints, double accuracy = 0.000001)
        {
            if (numOfPoints < 2) throw new Exception();
            double step = (b - a) / (numOfPoints - 1);
            List<List<double>> points = new List<List<double>>();
            for (int i = 0; i < numOfPoints; i++)
            {
                points.Add(new List<double>() { a + i * step, Taylor(a + i * step, accuracy) });
            }
            return points;
        }
        private List<List<double>> TabulateChebishev(double a, double b, int numOfPoints, double accuracy = 0.000001)
        {
            if (numOfPoints < 2) throw new Exception();
            List<List<double>> points = new List<List<double>>();
            for (int i = 0; i < numOfPoints; i++)
            {
                double x = 0.5 * (a + b) + 0.5 * (b - a) * Math.Cos((2 * i + 1) * Math.PI / (2 * numOfPoints));
                points.Add(new List<double>() { x, Taylor(x, accuracy) });
            }
            return points;
        }
        private double Lagrange(double x, List<List<double>> points)
        {
            double sum = 0, product;
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
        private double Erf(double x)
        {
            return 2 / Math.Sqrt(Math.PI) * RectrangleMethod(0, x, Exp);
        }
        private double Exp(double x)
        {
            return Math.Pow(Math.E, -x * x);
        }
        private double RectrangleMethod(double a, double b, Foo f, double eps = 0.0001, double n = 100)
        {
            double h = (b - a) / n;
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                double x = a + i * h;
                sum += h * f(x + h / 2);
            }

            h = (b - a) / 2 / n;
            double dsum = 0;
            for (int i = 0; i < 2 * n; i++)
            {
                double x = a + i * h;
                dsum += h * f(x + h / 2);
            }
            if (Math.Abs(sum - dsum) <= eps) return sum;
            return RectrangleMethod(a, b, f, eps, 2 * n);
        }
        private double DividedDiffs(List<List<double>> points)
        {
            double sum = 0;
            for (int j = 0; j < points.Count; j++)
            {
                double prod = 1;
                for (int i = 0; i < points.Count; i++)
                {
                    if (j == i) continue;
                    prod *= (points[j][0] - points[i][0]);
                }
                sum += points[j][1] / prod;
            }
            return sum;
        }
        private double Newton(double x, List<double> diffs, List<List<double>> points)
        {
            double sum = points[0][1];
            double prod = 1;
            for (int i = 0; i < diffs.Count; i++)
            {
                prod *= x - points[i][0];
                sum += prod * diffs[i];
            }
            return sum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numOfPoints++;
            Process();
        }
    }
}