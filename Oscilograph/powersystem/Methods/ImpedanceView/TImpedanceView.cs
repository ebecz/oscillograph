using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using PowerSystem.WaveView;
using PowerSystem.CMath;
using PowerSystem.NetWork;
using PowerSystem.IEEEComtrade;
using System.Runtime.InteropServices;

namespace PowerSystem.Methods.ImpedanceView
{
    public partial class TImpedanceView : Form
    {
            TImpedanceControl A;
            public TImpedanceView(TInstance.TNODE.TLINE Line)
            {
                InitializeComponent();
                A = new TImpedanceControl(Line);
                Text = Line.Name;
                A.Parent = this;
                A.Dock = System.Windows.Forms.DockStyle.Fill;
                //Box.Items.AddRange(new string[]{"ABC","AB","BC","CA","AN","BN","CN"});
            }
            const float Times = 5;
            public partial class TImpedanceControl : UserControl
            {
                public class Curve
                {
                    public List<Complex[]> Z = new List<Complex[]>();
                    public string Name;
                    public Pen PColor = new Pen(Color.Black, -1);
                    TInstance.TNODE.TLINE Line;
                    public Curve(string Name, TInstance.TNODE.TLINE Line, Color Color, TfZ fZ)
                    {
                        PColor.Color = Color;
                        this.Name = Name;
                        this.fZ = fZ;
                        this.Line = Line;
                        Complex[] Zn = new Complex[Line.I3F[0].BaseComtrade.endsamp[0]];
                        Complex Za = fZ(0), Zb;
                        int n = 0, m = 0;
                        for (ulong i = 1; i < Line.I3F[0].BaseComtrade.endsamp[0]; i++)
                        {
                            Zb = fZ(i);
                            if ((Zb - Za).Magnitude > Line.Z1.Magnitude / 50)
                            {
                                Zn[m] = Zb;
                                Za = Zb;
                                if (Zn[m].Magnitude > Times * Line.Z1.Magnitude)
                                {
                                    if (1 + n != m && m != n)
                                    {
                                        Z.Add(Zn.Skip(n).Take(m - n).ToArray());
                                    }
                                    n = m;
                                }
                                m++;
                            }
                        }
                        if (1 + n != m && m > 1)
                        {
                            Z.Add(Zn.Skip(n).Take(m - n).ToArray());
                        }
                    }
                    public void OnPaint(PaintEventArgs e,TTimeCursor TimeCursor,float Max)
                    {
                        PointF Pa, Pb;
                        foreach (Complex[] Zn in Z)
                        {
                            Pa = new PointF((float)Zn[0].Real, -(float)Zn[0].Imaginary);
                            for (long i = 1; i < Zn.Length; i++)
                            {
                                Pb = new PointF((float)Zn[i].Real, -(float)Zn[i].Imaginary);
                                e.Graphics.DrawLine(PColor, Pa, Pb);
                                Pa = Pb;
                            }
                        }
                        TComtrade.AChannel Channel = Line.I3F[0];
                        DateTime D = TimeCursor.Cursor;
                        long EndSamp = (long)(Channel.BaseComtrade.endsamp[Channel.BaseComtrade.samp.Count() - 1]);
                        long n = EndSamp * (D - Channel.BaseComtrade.start_time).Ticks / (Channel.BaseComtrade.end_time - Channel.BaseComtrade.start_time).Ticks;
                        if (n >= 0 && n <= EndSamp)
                        {
                            Complex Zp = fZ((ulong)n);
                            SizeF Pp2 = new SizeF(Max * 2 / (12 * 5), -Max * 2 / (12 * 5));
                            PointF Pp1 = new PointF((float)Zp.Real - Pp2.Width / 2, -(float)Zp.Imaginary - Pp2.Height / 2);
                            e.Graphics.FillEllipse(PColor.Brush,new RectangleF(Pp1, Pp2));
                            Circle = Zp;
                        }
                    }
                    public delegate Complex TfZ(ulong n);
                    public TfZ fZ;
                    public Complex Circle = 1 / Complex.Zero;
                }
                protected TInstance.TNODE.TLINE Line;
                public float[] Zones = new float[] { 0.8f, 1.2f, 2f };
                public float Zlinha;
                protected TTimeCursor TimeCursor;
                Pen PLine, PSquare;
                public List<Curve> Curves = new List<Curve>();
                float Max;
                ToolTip Tip = new ToolTip();
                public TImpedanceControl(TInstance.TNODE.TLINE Line)
                {
                    Complex[] Zn = new Complex[Line.I3F[0].BaseComtrade.endsamp[0]];
                    TPhasor
                    Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                    Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                    Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                    Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                    Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                    Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                    k0I0 = Line.I3F.Sequence[0].Reader(TPhasor.TReaderMode.PU)*Line.K0;
                    Curve.TfZ fab = delegate(ulong n)
                    {
                        if ((Va[n] - Vb[n]).Magnitude > 0.05)
                        {
                            return (Va[n] - Vb[n]) / (Ia[n] - Ib[n]);
                        }
                        else
                        {
                            return 1 / Complex.Zero;
                        }
                    },
                    fbc = delegate(ulong n)
                    {
                        if ((Vb[n] - Vc[n]).Magnitude > 0.05)
                        {
                            return (Vb[n] - Vc[n]) / (Ib[n] - Ic[n]);
                        }
                        else
                        {
                            return 1 / Complex.Zero;
                        }
                    },
                    fca = delegate(ulong n)
                    {
                        if ((Vc[n] - Va[n]).Magnitude > 0.05)
                        {
                            return (Vc[n] - Va[n]) / (Ic[n] - Ia[n]);
                        }
                        else
                        {
                            return 1 / Complex.Zero;
                        }
                    };
                    Curves.Add(new Curve("zAB", Line, Color.FromArgb((Line.I3F.A.ph.Color.ToArgb() + Line.I3F.B.ph.Color.ToArgb()) / 2), fab));
                    Curves.Add(new Curve("zBC", Line, Color.FromArgb((Line.I3F.B.ph.Color.ToArgb() + Line.I3F.C.ph.Color.ToArgb()) / 2), fbc));
                    Curves.Add(new Curve("zCA", Line, Color.FromArgb((Line.I3F.C.ph.Color.ToArgb() + Line.I3F.A.ph.Color.ToArgb()) / 2), fca));
                    Curves.Add(new Curve("zAn", Line, Line.I3F.A.ph.Color, delegate(ulong n)
                    {

                        if ((Va[n]).Magnitude > 0.05)
                        {
                            return Va[n] / (Ia[n] + k0I0[n]);
                        }
                        else
                        {
                            return 1 / Complex.Zero;
                        }
                    }));
                    Curves.Add(new Curve("zBn", Line, Line.I3F.B.ph.Color, delegate(ulong n)
                    {
                        if ((Vb[n]).Magnitude > 0.05)
                        {
                            return Vb[n] / (Ib[n] + k0I0[n]);
                        }
                        else
                        {
                            return 1 / Complex.Zero;
                        }
                    }));
                    Curves.Add(new Curve("zCn", Line, Line.I3F.C.ph.Color, delegate(ulong n)
                    {
                        if ((Vc[n]).Magnitude > 0.05)
                        {
                            return Vc[n] / (Ic[n] + k0I0[n]);
                        }
                        else
                        {
                            return 1 / Complex.Zero;
                        }
                    }));
                    this.Line = Line;
                    TimeCursor = Line.DE.Instance.Cursor;
                    TimeCursor.Change += (TTimeCursor Sender, DateTime Old) =>
                    {
                        this.Invalidate();
                    };
                    PLine = new Pen(Color.Black, -1);
                    PLine.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    PLine.Color = Color.Green;
                    PSquare = new Pen(Color.Black, -1);
                    PSquare.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
                    Max = 3 * (float)Line.Z1.Magnitude * 2;
                    Zlinha = (float)Line.Z1.Magnitude;
                }
                protected override void OnResize(EventArgs e)
                {
                    Invalidate();
                    base.OnResize(e);
                }
                protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
                {
                    string S="";
                    float fx = e.X - Width / 2, fy = Height / 2 - e.Y;
                    fx *= Max / Width;
                    fy *= Max / Height;
                    Complex Cu=new Complex(fx,fy);
                    double r = Max * 2 / (12 * 5);
                    foreach (Curve C in Curves)
                    {
                        if ((C.Circle - Cu).Magnitude < r)
                        {
                            S = C.Name + " - " + S;
                        }
                    }
                    if (S != "")
                    {
                        if (!Tip.Active)
                        {
                            Tip.Show(S.Remove(S.Length-3), this, e.X, e.Y - 20);
                            Tip.Active = true;
                        }
                    }
                    else
                    {
                        Tip.Active = false;
                    }
                    base.OnMouseMove(e);
                }
                protected override void OnPaint(PaintEventArgs e)
                {
                    e.Graphics.Clear(SystemColors.ButtonFace);
                    e.Graphics.TranslateTransform(Width / 2, Height / 2);
                    e.Graphics.ScaleTransform(Width / Max, Height / Max);
                    e.Graphics.FillEllipse(Brushes.White, -Zlinha * Times, -Zlinha * Times, 2 * Zlinha * Times, 2 * Zlinha * Times);
                    e.Graphics.DrawEllipse(PLine, -Zlinha * Times, -Zlinha * Times, 2 * Zlinha * Times, 2 * Zlinha * Times);
                    System.Drawing.Drawing2D.Matrix M = e.Graphics.Transform;
                    e.Graphics.ResetTransform();
                    e.Graphics.DrawLine(PSquare, 0, Height / 2, Width, Height / 2);
                    e.Graphics.DrawLine(PSquare, Width / 2, 0, Width / 2, Height);
                    e.Graphics.Transform=M;
                    PointF Pa, Pb;
                    PLine.Color = Color.Red;
                    Pa = new PointF(0, 0);
                    Pb = new PointF((float)Line.Z1.Real, -(float)Line.Z1.Imaginary);
                    e.Graphics.DrawLine(PLine, Pa, Pb);
                    PLine.Color = Color.Gray;
                    foreach(TInstance.TNODE.TLINE SubLines in Line.PARA.LINES)
                    {
                        if (SubLines != Line.Oposite)
                        {
                            Complex ZsL = SubLines.Z1 + Line.Z1;
                            PointF Pc = new PointF((float)ZsL.Real, -(float)ZsL.Imaginary);
                            e.Graphics.DrawLine(PLine, Pb, Pc);
                        }
                    }
                    PLine.Color = Color.Red;
                    foreach (float Zone in Zones)
                    {
                        e.Graphics.DrawEllipse(PLine, -Zone * Zlinha, -Zone * Zlinha, 2 * Zone * Zlinha, 2 * Zone * Zlinha);
                    }
                    foreach (Curve C in Curves)
                    {
                        C.OnPaint(e, TimeCursor, Max);
                    }
                    base.OnPaint(e);
                }
                protected override void OnMouseWheel(MouseEventArgs e)
                {
                    Max *= (1 + e.Delta / 1200f);
                    Max = Math.Min(Max, 10*Zlinha * Times);
                    Max = Math.Max(Max, Zlinha / 10);
                    Invalidate();
                    base.OnMouseWheel(e);
                }
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x216 || m.Msg == 0x214)
                { // WM_MOVING || WM_SIZING
                    // Keep the window square
                    RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                    int w = rc.Right - rc.Left;
                    int h = rc.Bottom - rc.Top - SystemInformation.CaptionHeight;
                    int z = w > h ? w : h;
                    rc.Bottom = rc.Top + z + SystemInformation.CaptionHeight;
                    rc.Right = rc.Left + z;
                    Marshal.StructureToPtr(rc, m.LParam, false);
                    m.Result = (IntPtr)1;
                    //return;
                }
                base.WndProc(ref m);
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }
            protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
            {
                base.OnClosing(e);
            }
    }
}
