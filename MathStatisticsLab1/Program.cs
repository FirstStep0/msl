using MathStatisticsLab1;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathStatisticsLab1
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    class random_value
    {
        public List<double> res_exp;
        public Random rand;
        public double a;
        public random_value(double a) {
            res_exp = new List<double>();
            rand = new Random();
            this.a = a;
        }
        public double rule(double x) {
            if (0.0 <= x && x <= 0.5) return -Math.Pow(2.0 - 4.0 * x, 0.25);
            else if (0.5 < x && x <= 1.0) return Math.Acos(a / 2.0 + 1.0 - x * a) / a;
            throw new Exception("x<0||1<x");
        }
        public void test()
        {
            double val = rand.NextDouble();
            res_exp.Add(rule(val));
        }
        public double getLastTest() {
            if (res_exp.Count == 0) throw new Exception("res_exp is empty");
            return res_exp.Last();
        }
        public void raffle(int count) {
            for (int i = 0; i < count; ++i) test();
            res_exp.Sort();
        } 
        public void clear()
        {
            res_exp.Clear();
            //MessageBox.Show("capacity = " + res_exp.Capacity);
        }
        //стат мат ожидание
        public double get_st_E()
        {
            if (res_exp.Count == 0) throw new Exception("res_exp is empty");
            double sum = 0.0;
            foreach (double x in res_exp)
            {
                sum += x;
            }
            sum /= res_exp.Count;
            return sum;
        }
        //стат дисперсия
        public double get_st_D()
        {
            if (res_exp.Count == 0) throw new Exception("res_exp is empty");
            double E = get_st_E();
            double sum = 0.0;
            foreach (double x in res_exp)
            {
                sum += Math.Pow(x - E, 2.0);
            }
            sum /= res_exp.Count;
            return sum;
        }
        //размах выборки
        public double get_st_R()
        {
            if (res_exp.Count == 0) throw new Exception("res_exp is empty");
            res_exp.Sort();
            return res_exp.Last() - res_exp.First();
        }
        //выборочная медиана
        public double get_st_Me()
        {
            if (res_exp.Count == 0) throw new Exception("res_exp is empty");
            res_exp.Sort();
            int mid = res_exp.Count / 2;
            if ((res_exp.Count % 2) == 1)
            {
                return res_exp[mid];
            }
            else
            {
                return (res_exp[mid] + res_exp[mid - 1]) / 2;
            }
        }
        public double get_y0() { 
            double y0 = Math.Acos(1.0 - a / 2.0) / a;
            return y0;
        }
        public double get_true_E() {
            double y0 = get_y0();
            double E = -Math.Pow(2.0, 1.25) / 5 - y0 * (1 - a / 2.0) / a + Math.Sin(a * y0) / (a * a);
            return E;
        }
        public double get_true_D() {
            double E = get_true_E();
            double y0 = get_y0();
            double D = Math.Pow(2.0, 1.5)/6.0 - Math.Pow(y0, 2.0) * Math.Cos(a * y0) / a + 2.0 * y0 * Math.Sin(a * y0) / Math.Pow(a, 2.0) + 2.0 * Math.Cos(a * y0) / Math.Pow(a, 3.0) - 2.0 / Math.Pow(a, 3.0) - E * E;
            return D;
        }
        public double true_F(double x)
        {
            double x0 = -Math.Pow(2.0, 0.25);
            double x1 = get_y0();
            if (x <= x0) return 0.0;
            else if (x <= 0.0) { return -Math.Pow(x, 4.0) / 4.0 + 0.5; }
            else if (x <= x1) { return 0.5 + 1.0 / a - Math.Cos(a * x) / a; }
            else return 1.0;
        }
        public double true_f(double x)
        {
            double x0 = -Math.Pow(2.0, 0.25);
            double x1 = get_y0();
            if (x <= x0) return 0.0;
            else if (x <= 0.0) return -Math.Pow(x, 3.0);
            else if (x <= x1) return Math.Sin(a * x);
            else return 0.0;
        }

       /* List<double> get_graph_F(double k)
        {
            double x0 = -Math.Pow(2.0, 0.25);
            double x1 = Math.Acos(1.0 - a / 2.0) / a;

            double x, h = (x1 - x0) / k;
            List<double> v;
            for (int i = 0; i <= k; ++i)
            {
                x = h * i;
            }
        }*/

    };

    /*double Fs(const vector<double>& v, double x) {
        if (v.size() <= 1)return 0.0;
        int ind = lower_bound(v.begin(), v.end(), x) - v.begin();
        return (double)ind / (v.size() - 1);
    }*/

   /* public  double f(double x)
    {
        double x0 = -Math.Pow(2.0, 0.25);
        double x1 = Math.Acos(1.0 - a / 2.0) / a;
        if (x <= x0) return 0.0;
        else if (x <= 0) return -Math.Pow(x, 3.0);
        else if (x <= x1) return Math.Sin(a * x);
        else return 0.0;
    }*/

    //get rand val of experiment
    /*double exp_v20(double x)
    {
        if (0.0 <= x && x <= 0.5) return -pow(2.0 - 4.0 * x, 0.25);
        else if (0.5 <= x && x <= 1.0) return acos(a / 2.0 + 1.0 - x * a) / a;
        //throw std::string("exp_v20");
    }*/

    /*double get_xs(const vector<double>& v)
    {
        double sum = 0.0;
        for (int i = 0; i < v.size(); ++i)
        {
            sum += v[i];
        }
        sum /= (v.size());
        return sum;
    }*/

    /*double get_S2(const vector<double>& v)
    {
        double xs = get_xs(v);
        double S2 = 0.0;
        for (int i = 0; i < v.size(); ++i)
        {
            double x = v[i];
            S2 += pow(x - xs, 2.0);
        }
        S2 /= (double)(v.size() - 1.0);
        return S2;
    }*/

    /*double get_D(const vector<double>& v) {
        double D = -1.0;
        for (auto x : v) {
            double tD = max(abs(F(x) - Fs(v, x)), abs(F(x) - Fs(v, x + 1e-6)));
            D = max(D, tD);
        }
        return D;
    }*/

    /*List<double> get_graph_F(double k)
    {
        double x0 = -pow(2.0, 0.25);
        double x1 = acos(1.0 - a / 2.0) / a;

        double x, h = (x1 - x0) / k;
        List<double> v;
        //v.push_back({ DBL_MIN,0.0 });
        for (int i = 0; i <= k; ++i)
        {
            x = h * i;
            // v.push_back({ x,F(x) });
        }
    }*/
    /*int miain()
    {
        cout.precision(10);
        //srand(time(0));
        /*double a = 1.0;
        double x = -pow(2.0, 0.8) / 5 - y0 * (1 - a / 2.0) / a + sin(a * y0) / (a * a);
        cout << x;

        int n, k;
        a = data["a"];
        n = data["n"];
        k = data["k"];

        random_value ra(exp_v20);
        for (int i = 0; i < n; ++i)
        {
            double x = ra.test();
        }
        sort(ra.res_exp.begin(), ra.res_exp.end());

        double y0 = acos(1.0 - a / 2.0) / a;
        double E = -pow(2.0, 1.25) / 5 - y0 * (1 - a / 2.0) / a + sin(a * y0) / (a * a);
        double xs = ra.get_st_E();
        double D = pow(2.0, 1.5) - pow(y0, 2.0) * cos(a * y0) / a - 2.0 * y0 * sin(a * y0) / pow(a, 2.0) - 2.0 * cos(a * y0) / pow(a, 3.0) + 2.0 / pow(a, 3.0) - E * E;
        double S2 = ra.get_st_D();
        double Me = ra.get_st_Me();
        double R = ra.get_st_R();

        for (int j = 0; j < border.size() - 1; ++j)
        {
            /*int nj = lower_bound(border.begin(), border.end(), border[j + 1]) - lower_bound(border.begin(), border.end(), border[j]);
            double delta = border[j + 1] - border[j];
            outData.Fs.push_back({ border[j],border[j + 1], (double)nj / (n * delta) });
        }
    }*/
}

