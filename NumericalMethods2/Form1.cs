using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;


namespace NumericalMethods2
{
    delegate decimal Foo(decimal x, decimal accuracy = 0.000001m);
    delegate decimal Method(decimal a, decimal b, Foo f);
    public partial class Form1 : Form
    {
        private decimal a = 0.4m, b = 4, eps = 0.000001m;
        private int n = 12;
        public Form1()
        {
            InitializeComponent();
            Process();
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
            //decimal[] r = EpsCalculate(a, b, Taylor, CentralRectangle, 11);
            //nbox.Text = r[0].ToString();
            //resbox.Text = r[1].ToString();
            resbox.Text = GenCalculate(a, b, Taylor, Gauss, n).ToString();
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
        private List<List<decimal>> Tabulate(decimal a, decimal b, int n, decimal accuracy = 0.000001m)
        {
            if (n < 2) throw new Exception();
            decimal step = (b - a) / (n - 1);
            List<List<decimal>> points = new List<List<decimal>>();
            for (int i = 0; i < n; i++)
            {
                points.Add(new List<decimal>() { a + i * step, Taylor(a + i * step, accuracy) });
            }
            return points;
        }
        private decimal[] EpsCalculate(decimal a, decimal b, Foo f, Method m, decimal n = 3, decimal eps = 0.00000000001m)
        {
            decimal sum = GenCalculate(a, b, f, m, n);
            decimal dsum = GenCalculate(a, b, f, m, 2 * n);
            if (Math.Abs(sum - dsum) <= eps) return new decimal[] { n, sum };

            return EpsCalculate(a, b, f, m, 2 * n, eps);
        }
        private decimal LeftRectangle(decimal a, decimal b, Foo f)
        {
            return (b - a) * f(a);
        }
        private decimal CentralRectangle(decimal a, decimal b, Foo f)
        {
            return (b - a) * f((a + b) / 2);
        }
        private decimal Trapezoid(decimal a, decimal b, Foo f)
        {
            return (b - a) * (f(a) + f(b)) / 2;
        }
        private decimal Simpson(decimal a, decimal b, Foo f)
        {
            return (b - a) / 6 * (f(a) + 4 * f((a + b) / 2) + f(b));
        }
        private decimal Gauss(decimal a, decimal b, Foo f)
        {
            decimal x1 = a + (b - a) / 2 * (1 - (decimal)Math.Sqrt(3) / 3),
                x2 = a + (b - a) / 2 * (1 + (decimal)Math.Sqrt(3) / 3);
            return (b - a) /2 * (f(x1) + f(x2));
        }
        private decimal GenCalculate(decimal a, decimal b, Foo f, Method m, decimal n = 3)
        {
            decimal h = (b - a) / (n - 1);
            decimal sum = 0;
            for (int i = 0; i < n - 1; i++)
            {
                sum += m(a + i * h, a + (i + 1) * h, f);
            }
            return sum;
        }
    }
}