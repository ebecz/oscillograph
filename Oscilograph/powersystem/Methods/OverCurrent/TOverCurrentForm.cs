using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;

namespace PowerSystem.Methods.OverCurrent
{
    public partial class TOverCurrentForm : Form
    {
        class TCurve
        {
            public TGroup Group;
            public string Classification;
            public double k;
            public double a;
            public double L;
            public double b;
            public TCurve(string Name, string Classification, float k, float a, float L, float b, List<TGroup> LGroup)
            {
                this.Classification = Classification;
                this.k = k;
                this.a = a;
                this.L = L;
                this.b = b;
                foreach (TGroup Group in LGroup)
                {
                    if (Group.Name == Name)
                    {
                        this.Group = Group;
                    }
                }
                if (this.Group == null)
                {
                    this.Group = new TGroup(Name);
                    LGroup.Add(this.Group);
                }
                this.Group.Curvas.Add(this);
            }
            public override string ToString()
            {
                return Classification;
            }
            public double ComputeOn(double M)
            {
                return (k / (Math.Pow(M, a) - b) + L);
            }

            internal double ComputeOff(double M)
            {
                return ComputeOn(1 / M);
            }
        }
        class TGroup
        {
            public string Name;
            public List<TCurve> Curvas = new List<TCurve>();
            public TGroup(string Name)
            {
                this.Name = Name;
            }
            public override string ToString()
            {
                return Name;
            }
        }
        TCurve Active;
        static List<TGroup> LGroup = new List<TGroup>();
        static TCurve[] Curves = new TCurve[]{
            //Norma	Tipo da Curva	K	α	L	β	Selecionar
            new TCurve("IEC","Curva Inversa",0.14f,0.02f,0f,1f,LGroup),
            new TCurve("IEC","Moderadamente Inversa",0.05f,0.04f,0f,1f,LGroup),
            new TCurve("IEC","Muito Inversa",13.5f,1f,0f,1f,LGroup),
            new TCurve("IEC","Extremamente inversa",80f,2f,0f,1f,LGroup),

            new TCurve("IEEE","Moderadamente Inversa",0.0515f,0.02f,0.114f,1f,LGroup),
            new TCurve("IEEE","Muito Inversa",19.61f,2f,0.491f,1f,LGroup),
            new TCurve("IEEE","Extremamente inversa",28.2f,2f,0.1217f,1f,LGroup),

            new TCurve("I².t","Curva I².t",100f,2f,0f,0f,LGroup),
            new TCurve("Tempo Definido","Tempo Definido",0f,0f,1f,0f,LGroup)
        };
        public TOverCurrentForm()
        {
            InitializeComponent();
            foreach (TGroup Group in LGroup)
            {
                CurveSelection.Items.Add(Group);
            }
        }

        private void CurveSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurveSelection.SelectedItem != null)
            {
                ClassTipo.Items.Clear();
                TGroup Group = (TGroup)CurveSelection.SelectedItem;
                foreach (TCurve Curve in Group.Curvas)
                {
                    ClassTipo.Items.Add(Curve);
                }
            }
            ClassTipo.Text = "";
            Active = null;
            PaintPanel.Invalidate();
        }
        private void PaintPanel_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsState G = e.Graphics.Save();
            e.Graphics.TranslateTransform(0, PaintPanel.Height);
            e.Graphics.ScaleTransform(1, -1);
            double M10max = (double)UpDownDec.Value, M10min = -0.2;
            int W = PaintPanel.Width / 10;
            int Dp = (int)Math.Min(PaintPanel.Height * 0.05f, PaintPanel.Width * 0.05f);
            double T10max = (double)UpDownDec.Value, T10min = -1;
            Pen MyPen = new Pen(Brushes.Black, -1);
            var g = e.Graphics.Save();
            PointF[] P = null;
            if (Active != null)
            {
                P = new PointF[W];
                T10max = 0;
                T10min = double.MaxValue;
                for (int x = 1; x <= W; x++)
                {
                    double M10 = M10max * x / W;
                    double M = Math.Pow(10, M10);
                    double T = Active.ComputeOn(M) * (double)(UpDownMultiple.Value);
                    //(Active.k / (Math.Pow(M, Active.a) - Active.b) + Active.L) * ;
                    double T10 = Math.Log10(T);
                    T10max = Math.Max(T10, T10max);
                    T10min = Math.Min(T10, T10min);
                    P[x - 1].X = (float)M10;
                    P[x - 1].Y = (float)T10;
                }
            }
            //T10min = T10min - 3;
            T10min = Math.Min(T10min, -3);
            e.Graphics.ScaleTransform((float)(PaintPanel.Width / (M10max - M10min)), (float)(PaintPanel.Height / (T10max - T10min)));
            e.Graphics.TranslateTransform(-(float)M10min, -(float)T10min);
            if (P != null)
            {
                e.Graphics.DrawLines(MyPen, P);
                MyPen.Color = Color.Pink;
                float inst = (float)Math.Log10((float)UpDownInstanteneous.Value / (float)UpDownPickUp.Value);
                e.Graphics.DrawLine(MyPen, inst, (float)T10max, inst, (float)T10min);
            }
            foreach (TRelayCurve ICurve in ICurves)
            {
                //var G=e.Graphics.Save();
                //e.Graphics.ScaleTransform((float)(PaintPanel.Width / M10max), (float)(PaintPanel.Height / MaxY));
                //e.Graphics.TranslateTransform(0, 1);
                MyPen.Color = ICurve.Phasor.Channel.ph.Color;
                foreach (PointF[] Ps in ICurve.lPs)
                {
                    e.Graphics.DrawLines(MyPen, Ps);
                }
                //e.Graphics.Restore(G);
            }
            e.Graphics.Restore(g);
            foreach (TRelayCurve ICurve in ICurves)
            {
                if (ICurve.EnableSign)
                {
                    Brush B = new SolidBrush(ICurve.Phasor.Channel.ph.Color);
                    foreach (PointF PfC in new PointF[] { ICurve.LineSign, ICurve.TimeSign })
                    {
                        //PointF PfC = ICurve.LineSign;
                        double Tlog10 = PfC.Y, Mlog10 = PfC.X;
                        Tlog10 = ((Tlog10 - T10min) * PaintPanel.Height / (T10max - T10min));
                        Mlog10 = ((Mlog10 - M10min) * PaintPanel.Width / (M10max - M10min));
                        e.Graphics.FillEllipse(B, (float)Mlog10 - 5, (float)Tlog10 - 5, 10, 10);
                    }
                    B.Dispose();
                }
            }
            e.Graphics.Restore(G);
            if (Active != null)
            {
                double sy = PaintPanel.Height / (T10max - T10min);
                for (int y = (int)Math.Floor(T10min); y <= T10max; y++)
                {
                    e.Graphics.DrawString(y.ToString(), DefaultFont, Brushes.Black, 0, PaintPanel.Height - (y - (int)Math.Floor(T10min)) * (float)sy - 3 * DefaultFont.Height / 2);
                    //e.Graphics.DrawLine(MyPen, 0, y, Dp / 10, y);
                }
                double sx = PaintPanel.Width / (M10max - M10min);
                for (int x = 0; x <= M10max; x++)
                {
                    int X = (int)(x * PaintPanel.Width / M10max);
                    //e.Graphics.DrawLine(MyPen, X, Height, X, Dp);
                    e.Graphics.DrawString(x.ToString(), DefaultFont, Brushes.Black, X + DefaultFont.Height, PaintPanel.Height - DefaultFont.Height);
                }
            }
        }
        private void ClassTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Active = (TCurve)ClassTipo.SelectedItem;
            PaintPanel.Invalidate();
        }
        private void UpDownMultiple_ValueChanged(object sender, EventArgs e)
        {
            PaintPanel.Invalidate();
        }
        TInstance.TNODE.TLINE Linha;
        class TRelayCurve
        {
            public PointF[] RD;
            public List<PointF[]> lPs = new List<PointF[]>();
            public CMath.TPhasor Phasor;
            public PointF LineSign = new PointF(0, 0);
            public PointF TimeSign = new PointF(0, 0);
            public bool EnableSign = false;
            public TRelayCurve(IEEEComtrade.TComtrade.AChannel Channel, double PickUp, double Multiple, TCurve Curve)
            {
                RD = new PointF[1 + Channel.BaseComtrade.endsamp[0] / 10];
                Phasor = Channel.Phasor.Reader(CMath.TPhasor.TReaderMode.Secondary);
                double D = 0, lM = double.MaxValue;
                PointF lP = PointF.Empty;
                bool RelayOn = false;
                List<PointF> Ps = null;
                for (ulong i = 0; i < Channel.BaseComtrade.endsamp[0]; i += 10)
                {
                    float X, Y;
                    double M = (Phasor[i].Magnitude / PickUp);
                    double CoM;
                    CoM = Multiple * Curve.ComputeOn(M);
                    if (M >= 1)
                    {
                        D += 10 / (CoM * Channel.BaseComtrade.samp[0]);
                        if (!RelayOn)
                        {
                            RelayOn = true;
                            Ps = new List<PointF>();
                            X = (float)Math.Log10(M);
                            Y = (float)Math.Log10(D * CoM);
                            lP = new PointF(X, Y);
                        }
                    }
                    else
                    {
                        D -= 10 / (Multiple * Curve.ComputeOff(M) * Channel.BaseComtrade.samp[0]);
                    }
                    if (D <= 0.1 & M < 1)
                    {
                        D = 0;
                        if (RelayOn)
                        {
                            RelayOn = false;
                            Ps.Add(lP);
                            if (Ps.Count > 1)
                            {
                                lPs.Add(Ps.ToArray());
                            }
                        }
                    }
                    if (M == 0)
                    {
                        X = 0;
                    }
                    else
                    {
                        X = (float)Math.Log10(M);
                    }
                    Y = (float)Math.Log10(D * CoM);
                    RD[i / 10] = new PointF(X, Y);
                    if (RelayOn)
                    {
                        if (Math.Abs(M - lM) > 0.01)
                        {
                            Ps.Add(lP);
                            lP = RD[i / 10];
                            lM = M;
                        }
                    }
                }
                if (RelayOn)
                {
                    RelayOn = false;
                    if (Ps.Count > 1)
                    {
                        lPs.Add(Ps.ToArray());
                    }
                }
            }
        }
        List<TRelayCurve> ICurves = new List<TRelayCurve>();
        public TInstance.TNODE.TLINE Line
        {
            set
            {
                Linha = value;
                Linha.DE.Instance.Cursor.Change += new TTimeCursor.OnChange(Cursor_Change);
                ResetRelayCurves();
            }
            get
            {
                return Linha;
            }
        }
        void ResetRelayCurves()
        {
            if (Active != null)
            {
                ICurves.Clear();
                foreach (IEEEComtrade.TComtrade.AChannel Channel in Line.I3F)
                {
                    ICurves.Add(new TRelayCurve(Channel, (double)UpDownPickUp.Value, (double)UpDownMultiple.Value, Active));
                }
            }
        }
        static float Max(float a, float b)
        {
            return a > b ? a : b;
        }
        static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
        static float Abs(float x)
        {
            return x > 0 ? x : -x;
        }
        void Cursor_Change(TTimeCursor Sender, DateTime OldValue)
        {
            UpDate();
        }
        void UpDate()
        {
            if (Active != null)
            {
                double PickUp = (double)UpDownPickUp.Value;
                DateTime D = Linha.DE.Instance.Cursor.Cursor;
                foreach (TRelayCurve ICurve in ICurves)
                {
                    double M = ICurve.Phasor[D].Magnitude / (double)UpDownPickUp.Value;
                    float Tlog10 = float.MaxValue, Mlog10 = 0;
                    double T = 0;
                    if (M > 1)
                    {
                        T = (Active.k / (Math.Pow(M, Active.a) - Active.b) + Active.L) * (double)(UpDownMultiple.Value);
                        Tlog10 = (float)Math.Log10(T);
                        Mlog10 = (float)Math.Log10(M);
                    }
                    if (Tlog10 > PaintPanel.Height)
                    {
                        Tlog10 = PaintPanel.Height;
                    }
                    ICurve.LineSign = new PointF(Mlog10, Tlog10);
                    long EndSamp = (long)(ICurve.Phasor.Channel.BaseComtrade.endsamp[ICurve.Phasor.Channel.BaseComtrade.samp.Count() - 1]);
                    long n = EndSamp * (D - ICurve.Phasor.Channel.BaseComtrade.start_time).Ticks / (ICurve.Phasor.Channel.BaseComtrade.end_time - ICurve.Phasor.Channel.BaseComtrade.start_time).Ticks;
                    if (n >= 0 && n <= EndSamp)
                    {
                        ICurve.TimeSign = ICurve.RD[n / 10];
                        ICurve.EnableSign = true;
                    }
                    else
                    {
                        ICurve.EnableSign = false;
                    }
                }
                PaintPanel.Invalidate();
            }
        }
        private void UpDownPickUp_ValueChanged(object sender, EventArgs e)
        {
            //Reset Curves
            ResetRelayCurves();
            PaintPanel.Invalidate();
        }
        private void UpDownDec_ValueChanged(object sender, EventArgs e)
        {
            ResetRelayCurves();
            PaintPanel.Invalidate();
        }
    }
    class TPaintPanel : Panel
    {
        public TPaintPanel()
        {
            ResizeRedraw = true;
            DoubleBuffered = true;
        }
    }

}
