using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;


namespace NumericalMethods2
{
    delegate decimal Foo(decimal x);
    delegate decimal Method(decimal a, decimal b, Foo f);
    public partial class Form1 : Form
    {
        private decimal a = 0.4m, b = 4;
        private int N = 11;
        public Form1()
        {
            InitializeComponent();
            Process();
        }
        private void ParseVector(List<List<decimal>> v)
        {
            try
            {
                using (StreamWriter file = new StreamWriter("data.txt"))
                {
                    for (int i = 0; i < v.Count; i++)
                    {
                        string s = "";
                        for (int j = 0; j < v[0].Count - 1; j++)
                        {
                            s += Math.Round(v[i][j], 10) + "\t";
                        }
                        s += Math.Round(v[i][v[i].Count - 1], 10);
                        file.WriteLine(s);
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
            List<List<decimal>> table = new List<List<decimal>>();
            decimal h = (b - a) / (N - 1);
            for (int i = 0; i < N; i++)
            {
                decimal x = a + i * h;
                List<decimal> line = EpsCalculate(0, x, Integrant, Gauss);
                line.Insert(0, x);
                line.Insert(1, Taylor(x));
                line.Insert(3, Math.Abs(Taylor(x) - line[2]));
                table.Add(line);
            }
            ParseVector(table);
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
        private decimal Integrant(decimal x)
        {
            return x != 0 ?(decimal)(Math.Cos((double)x) - 1) / x : 0m;
        }
        private List<decimal> EpsCalculate(decimal a, decimal b, Foo f, Method m, decimal n = 2, decimal eps = 0.000001m)
        {
            decimal sum = GenCalculate(a, b, f, m, n);
            decimal dsum = GenCalculate(a, b, f, m, 2 * n);
            if (Math.Abs(sum - dsum) <= eps) return new List<decimal>() { sum, n };

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
        private decimal GenCalculate(decimal a, decimal b, Foo f, Method m, decimal n = 2)
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