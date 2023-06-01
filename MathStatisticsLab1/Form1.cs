using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MathNet.Numerics;
using MathNet.Numerics.Random;
//using System.Web.UI.DataVisualization.Charting
//using System.Collections.Generic;
namespace MathStatisticsLab1
{
   // class<K, V> Pair{
    
//}
    public partial class Form1 : Form
    {
        random_value value;

        DataTable table;
        DataTable chrt;
        DataTable border;
        DataTable border2;
        DataTable q;
        DataTable info;
        DataTable view1;
        //List<List<Pair<double, double>>> FR0_result;
        int dx, dy;
        string pathCalc = @".\tmp\random.exe";
        string pathResult = @".\tmp\result.txt";
        string pathInput = @".\tmp\input.txt";
        public Form1()
        {
            InitializeComponent();
        }
        public class Params
        {
            public double a;
            public int n;
            public int c;
            public List<double> border;
            public List<double> border2;
            public double alpha;
            public Params()
            {
                border = new List<double>();
                border2 = new List<double>();
            }
        }
        private bool checkParams(Params p)
        {
            if (!double.TryParse(textBox_a.Text, out p.a) || !((0.0 < p.a) && (p.a <= 4.0))) { MessageBox.Show("Некорректное значение a\n"); return false; }
            else if (!int.TryParse(textBox_N.Text, out p.n) || !(p.n > 0)) { MessageBox.Show("Некорректное значение N\n"); return false; }
            else if (!double.TryParse(textBox_alpha.Text, out p.alpha)) { MessageBox.Show("Некорректное значение alpha\n"); return false; }
            else if (!int.TryParse(textBox4.Text, out p.c)) { MessageBox.Show("некорректное значение Count\n"); return false; }
            List<double> border_list = p.border;
            List<double> border_list2 = p.border2;
            List<DataRow> delete_list = new List<DataRow>();

            //check border
            double val;
            foreach (DataRow row in border.Rows)
            {
                if (double.TryParse(row.ItemArray[0].ToString(), out val)) border_list.Add(val);
                else delete_list.Add(row);
            }
            foreach (DataRow item in delete_list) item.Delete();

            foreach (DataRow row in border2.Rows)
            {
                if (double.TryParse(row.ItemArray[0].ToString(), out val)) border_list2.Add(val);
                else delete_list.Add(row);
            }

            foreach (DataRow item in delete_list) item.Delete();

            p.border.Sort();
            p.border2.Sort();

            return true;
        }

        private void set_a() {
            double a;
            if (double.TryParse(textBox_a.Text, out a) && 0.0 < a && a <= 4.0)
            {
                value.a = a;
                textBox2.Text = String.Format("{0:F10}", value.get_y0());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //double R = 3.00000000000002;
           // MessageBox.Show(F_R0(R, 1).ToString());
            value = new random_value(1.0);

            q = new DataTable();
            dataGridView_qj.DataSource = q;
            q.Columns.Add("[xj-1,xj]", typeof(string));
            q.Columns.Add("qj", typeof(double));
            q.Columns.Add("nj/n", typeof(double));
            q.Columns.Add("nj", typeof(int));
            q.Columns.Add("nj-n*qj", typeof(double));
            q.Columns.Add("R", typeof(double));

            table = new DataTable();
            DataTable_table.DataSource = table;
            table.Columns.Add("n", typeof(int));
            table.Columns.Add("xj", typeof(double));

            info = new DataTable();
            dataGridView_info.DataSource = info;
            info.Columns.Add("(xj,xj+1]", typeof(string));
            info.Columns.Add("zj", typeof(double));
            info.Columns.Add("f(zj)", typeof(double));
            info.Columns.Add("Rj", typeof(double));

            chrt = new DataTable();
            dataGridView_characteristics.DataSource = chrt;
            chrt.Columns.Add("E");
            chrt.Columns.Add("x'");
            chrt.Columns.Add("|E-x'|");
            chrt.Columns.Add("D");
            chrt.Columns.Add("S2");
            chrt.Columns.Add("|D-S2|");
            chrt.Columns.Add("Me'");
            chrt.Columns.Add("R'");

            border = new DataTable();
            dataGridView_border.DataSource = border;
            border.Columns.Add("xj", typeof(double));

            border2 = new DataTable();
            dataGridView_border2.DataSource = border2;
            dataGridView2.DataSource = border2;
            border2.Columns.Add("xj", typeof(double));

            view1 = new DataTable();
            dataGridView1.DataSource = view1;
            view1.Columns.Add("numb", typeof(int));
            view1.Columns.Add("N", typeof(int));
            view1.Columns.Add("count", typeof(int));
            view1.Columns.Add("alpha", typeof(double));
            view1.Columns.Add("result", typeof(double));

            textBox1.Text = String.Format("{0:F10}", -Math.Pow(2.0, 0.25));

            set_a();
        }

        private void textBox_a_TextChanged(object sender, EventArgs e)
        {
            set_a();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in dataGridView_border.SelectedRows)
                {
                    dataGridView_border.Rows.RemoveAt(item.Index);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            GC.Collect();
            Params p = new Params();
            //MessageBox.Show(gamma(6).ToString());

            if (checkParams(p))
            {
                textBox11.Text = F_R0(Math.Max(p.border2.Count - 4, 0), p.border2.Count - 2).ToString();

                value.clear();
                value.raffle(p.n);

                textBox_FR0.Text = get_F_R0(p).ToString();

                double val = 0.0;

                if (p.c > 0) {
                    int count = 0;
                    for (int i = 0; i < p.c; ++i)
                    {
                        value.clear();
                        value.raffle(p.n);

                        val = test_F_R0(p);
                        if (val < p.alpha) count++;
                        //textBox6.Text = (i + 1).ToString();
                    }
                    textBox5.Text = ((double)count / p.c).ToString();
                    view1.Rows.Add((view1.Rows.Count + 1), p.n, p.c, p.alpha, (double)count / p.c);
                }

                fill_table(value.res_exp);

                fill_chrt();

                fill_stat_f(p);
                fill_f(p);
                //true F
                fill_graph();
                //stat_F
                fill_stat_graph();

                /*if (p.border2.Count >= 2)
                {
                    q.Rows.Clear();
                    for (int i = 1; i < p.border2.Count; ++i) {
                        q.Rows.Add("[" + p.border2[i - 1] + ";" + p.border2[i] + "]", value.true_F(p.border2[i]) - value.true_F(p.border2[i - 1]));
                    }
                }*/
                //double crit_val = binary_search(0.0, 1.0, p.alpha, 100, (double x) => { return F_R0(x, p.border2.Count - 1); });
                //textBox4.Text = p.alpha.ToString();
            }
        }
        /*private double Fn_R0(double R0, int r)
        {
            return 1.0 - trapezoidalIntegral(0.0, R0, 1000, r);
        }*/
        double fn(double x, int r)
        {
            double res;
            if (x <= 0) { res = 0.0; }
            else
            {
                res = Math.Pow(0.5, r / 2.0) * Math.Pow(SpecialFunctions.Gamma((double)(r) / 2.0), (-1.0)) * Math.Pow(x, (r / 2.0) - 1.0) * Math.Exp((-x / 2.0));
            }
            return res;
        }

        /*double trapezoidalIntegral(double a, double b, int n, int r)
        {
            double width = (b - a) / (double)n;
            double trapezoidal_integral = 0.0;
            for (int step = 0; step <= n - 1; ++step)
            {
                double x1 = a + step * width;
                double x2 = a + (step + 1) * width;
                trapezoidal_integral += width * (fn(x1, r) + fn(x2, r)) * 0.5;
            }
            return trapezoidal_integral;
        }*/
        private delegate double func(double a);
        /*private double binary_search(double left, double right, double val, int count, func f) {
            double mid = (left + right) / 2.0;
            for (int i = 0; i < count; ++i) {
                mid = (left + right) / 2.0;
                if (f(mid) < val) left = mid;
                else right = mid;
                //MessageBox.Show(f(mid).ToString());
            }
            return mid;
        }*/
        /*private double get_critical_border(func f, double val) {
            binary_search(-5, 100, );
        }*/
        /*public double gamma(int m2) {
            double g, a;
            double last = (double)m2 / 2.0;
            if (m2 % 2 == 1)
            {
                g = Math.Pow(Math.PI, 0.5);
                a = 0.5;
            }
            else {
                g = 1.0;
                a = 1.0;
            }
            for (double x = a; x < last; x += 1.0)
            {
                g *= x;
            }
            return g;
        }*/
        private double fx2(double x, int r) {
            if (x <= 0) return 0.0;
            else {
                //SpecialFunctions.Gamma
                return Math.Pow(2.0, -r / 2.0) * Math.Pow(x, ((double)(r) / 2.0) - 1.0) * Math.Exp(-x / 2) / SpecialFunctions.Gamma(((double)r) / 2.0);
            }
        }
        private double integral(double a, double b, func f) {
            return (f((a + b) / 2.0)) * (b - a);
        }
        //double n123;
        private double integral(double a, double b, func f, int n) {
            double h = (b - a) / n;
            double x1, x2;
            decimal ans = 0;
            for (int i = 0; i <= n - 1; ++i) 
            {
                x1 = a + h * i;
                x2 = x1 + h;
                ans += (decimal)integral(x1, x2, f);
            }
            return (double)ans;
        }
        /*private double integral(double a, double b, func f, int n) {
            double ans1, ans2;
            double eps = 1e-3;
            do
            {
                ans1 = _integral(a, b, f, n);
                ans2 = _integral(a, b, f, n * 2);
                n *= 2;
            } while (Math.Abs(ans1 - ans2) >= eps);
            // n123 = n;
            return ans2;
        }*/
        private double F_R0(double R0, int r) {
            return 1.0 - integral(0.0, R0, (double x) => { return fx2(x, r); }, 10000);
        }
        private double test_F_R0(Params p) {
            //q.Rows.Clear();
            List<double> res = value.res_exp;
            List<double> b = p.border2;
            int n = value.res_exp.Count;
            double R = 0;
            double FR = 0;
            if (b.Count >= 2)
            {
                b.Sort();
                res.Sort();

                int i = 0;
                if (b.Count > 0)
                {
                    while (i < res.Count && res[i] <= b[0]) ++i;
                }
                R = 0;
                for (int index = 1; index < b.Count; ++index)
                {
                    int count = 0;
                    for (; i < res.Count && res[i] <= b[index]; ++i)
                    {
                        count++;
                    }

                    double qj = value.true_F(b[index]) - value.true_F(b[index - 1]);
                    //if (qj > 0)
                    //{
                        if (qj < 0) MessageBox.Show("!");
                        double tmp = ((double)count) - (qj * ((double)n));
                        double Ri = Math.Pow(tmp, 2.0) / (((double)n) * qj);
                        R += Ri;
                        // q.Rows.Add("[" + p.border2[index - 1] + ";" + p.border2[index] + "]", qj, (double)count / n, Ri);
                    /*}
                    else {
                        throw new Exception("q < 0");
                    }*/
                }
                FR = F_R0(R, p.border2.Count - 2);
                //MessageBox.Show(R.ToString() + " FR: " + FR + " FnR: " + Fn_R0(R, p.border2.Count - 2) + "n:" + n123);
            }
            return FR;
        }
        private double get_F_R0(Params p) {
            q.Rows.Clear();
            List<double> res = value.res_exp;
            List<double> b = p.border2;
            int n = value.res_exp.Count;
            double R = 0;
            double FR = 0;
            if (b.Count >= 2)
            {
                p.border2.Sort();
                res.Sort();

                int i = 0;
                if (b.Count > 0)
                {
                    while (i < n && res[i] <= b[0]) ++i;
                }
                
                for (int index = 1; index < b.Count; ++index)
                {
                    int count = 0;
                    for (; i < n && res[i] <= b[index]; ++i)count++;
                    /*if (Math.Abs(value.true_F(b[index]) - value.true_F(b[index - 1])) > 0) 
                    {*/
                        double qj = value.true_F(b[index]) - value.true_F(b[index - 1]);
                        if (qj < 0) MessageBox.Show("!");
                        double tmp = (double)count - qj * (double)n;
                        double Ri = Math.Pow(tmp, 2.0) / ((double)n * qj);
                        R += Ri;
                        q.Rows.Add("[" + p.border2[index - 1] + ";" + p.border2[index] + "]", qj, (double)count/n, count, tmp, Ri);
                    //}
                }
                textBox10.Text = R.ToString();
                FR = F_R0(R, p.border2.Count - 2);
            }
            return FR;
        }
        private void fill_stat_graph() {
            if (value.res_exp.Count > 0) {
                double D = Double.MinValue;
                chart1.Series[1].Points.Clear();
                List<double> res = value.res_exp;
                int n = res.Count;

                double x0 = -Math.Pow(2.0, 0.25);
                chart1.Series[1].Points.AddXY(x0, 0);
                chart1.Series[1].Points.AddXY(res[0], 0);

                double left, right = res[0];
                for (int i = 0; i < n - 1; ++i) 
                {
                    //left = res[i]; right = res[i + 1];
                    left = right; right = res[i + 1];
                    double val = (double)(i + 1) / n;
                    chart1.Series[1].Points.AddXY(left, val);
                    D = Math.Max(D, Math.Abs(value.true_F(left) - val));
                    D = Math.Max(D, Math.Abs(value.true_F(right) - val));
                    chart1.Series[1].Points.AddXY(right, val);
                }

                double x1 = value.get_y0();
                chart1.Series[1].Points.AddXY(res.Last(), 1.0);
                chart1.Series[1].Points.AddXY(x1, 1.0);
                textBox_D.Text = D.ToString();
            }
        }
        private void fill_f(Params p) {
            chart2.Series[1].Points.Clear();
            int k = 3000;
            double x0 = -Math.Pow(2.0, 0.25);
            double x1 = value.get_y0();
            double h = (x1 - x0) / k;
            for (int i = 0; i <= k; ++i)
            {
                double x = x0 + h * i;
                chart2.Series[1].Points.AddXY(x, value.true_f(x));
            }
        }
        private void fill_graph() {
            chart1.Series[0].Points.Clear();
            int k = 30;
            double x0 = -Math.Pow(2.0, 0.25);
            double x1 = value.get_y0();
            double h = (x1 - x0) / k;
            for (int i = 0; i <= k; ++i)
            {
                double x = x0 + h * i;
                chart1.Series[0].Points.AddXY(x, value.true_F(x));
            }
        }
        private void fill_table(List<double> list) {
            table.Rows.Clear();
            //table.Rows = new DataRowCollection();
            for (int i = 0; i < list.Count; ++i) {
                table.Rows.Add(i + 1, list[i]);
            }
        }
        private void fill_stat_f(Params p)
        {
            if (p.border.Count >= 2)
            {
                info.Clear();
                chart2.Series[0].Points.Clear();

                p.border.Sort();
                value.res_exp.Sort();

                List<double> b = p.border;
                List<double> res = value.res_exp;
                int i = 0;
                if (b.Count > 0)
                {
                    while (i < res.Count && i <= b[0]) ++i;
                }
                chart2.Series[0].Points.AddXY(b[0], 0);
                double ma = Double.MinValue;
                for (int index = 1; index < b.Count; ++index)
                {
                    int count = 0;
                    for (; i < res.Count && res[i] <= b[index]; ++i)
                    {
                        count++;
                    }
                    double zj = (b[index] + b[index - 1]) / 2.0;
                    double fs = 0;
                    if (b[index] != b[index - 1])
                    {
                        fs = ((double)count) / (res.Count * (b[index] - b[index - 1]));
                    }
                    double f = value.true_f(zj);
                    info.Rows.Add("(" + b[index - 1] + "; " + b[index] + "]", zj, f, fs);
                    ma = Math.Max(ma, Math.Abs(f - fs));
                    chart2.Series[0].Points.AddXY(b[index - 1], fs);
                    chart2.Series[0].Points.AddXY(b[index], fs);
                }
                chart2.Series[0].Points.AddXY(b.Last(), 0);
                textBox_max.Text = ma.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in dataGridView_border2.SelectedRows)
                {
                    dataGridView_border2.Rows.RemoveAt(item.Index);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int n;
            if (!int.TryParse(textBox3.Text, out n)){
                MessageBox.Show("Некорр значение n");
            }
            else {
                border2.Clear();
                double h = 1.0 / n;
                for (int i = 0; i <= n; ++i)
                {
                    double y = h * i;
                    border2.Rows.Add(value.rule(y));
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int n;
            if (!int.TryParse(textBox6.Text, out n))
            {
                MessageBox.Show("Некорр значение n");
            }
            else
            {
                border2.Clear();
                double h = 1.0 / n;
                for (int i = 0; i <= n; ++i)
                {
                    double y = h * i;
                    border2.Rows.Add(value.rule(y));
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in dataGridView2.SelectedRows)
                {
                    dataGridView2.Rows.RemoveAt(item.Index);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.RemoveAt(item.Index);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        void UpdateSelectCount() {
            label15.Text = String.Format("{0} / {1}", dataGridView1.SelectedRows.Count, dataGridView1.Rows.Count);
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectCount();
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            UpdateSelectCount();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double alpha;
            if (!double.TryParse(textBox_alpha.Text, out alpha)) return;
            int m, r, b;
            m = r = b = 0;
            int indexColumn = 4;
            double sum = 0;
            for (int i = 0; i < dataGridView1.RowCount; ++i) {
                double value = (double)dataGridView1[indexColumn, i].Value;
                if (value < alpha) m++;
                else if (Math.Abs(value - alpha) < 1e-7) r++;
                else if (value > alpha) b++;
                sum += value;
            }
            textBox7.Text = String.Format("{0:f3}", ((double)m / dataGridView1.RowCount));
            textBox8.Text = String.Format("{0:f3}", ((double)r / dataGridView1.RowCount));
            textBox9.Text = String.Format("{0:f3}", ((double)b / dataGridView1.RowCount));

            textBox12.Text = (sum / dataGridView1.RowCount).ToString();
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void fill_chrt() {
            double E, xs, D, S2, Me, R;
            E = value.get_true_E();
            xs = value.get_st_E();
            D = value.get_true_D();
            S2 = value.get_st_D();
            Me = value.get_st_Me();
            R = value.get_st_R();
            chrt.Rows.Clear();
            chrt.Rows.Add(E, xs, Math.Abs(E - xs), D, S2, Math.Abs(D - S2), Me, R);
        }
    }

    class outData {
        public List<double> res_exp;
        public List<List<double>> Fs;
        public List<List<double>> F;
        public double y0, E, xs, D, S2, Me, R;
    };
}
