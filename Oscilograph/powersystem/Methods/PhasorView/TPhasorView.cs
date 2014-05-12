using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

using PowerSystem.IEEEComtrade;
using PowerSystem.WaveView;
using PowerSystem.CMath;
using PowerSystem.NetWork;

namespace PowerSystem.Methods.PhasorView
{
    public partial class TPhasorView : TMethod<TInstance.TNODE.TLINE>
    {
        public System.Windows.Forms.ToolStrip MenuTool
        {
            get
            {
                return ToolStrip;
            }
        }
        public string Text
        {
            get
            {
                return "Show Phasors";
            }
        }
        public string Description
        {
            get
            {
                return Text;
            }
        }
        private class TPhasorViewForm:Form
        {
            TPhasorControl F, S;
            Label lF, lS;
            public void CopyToClipboard()
            {
                MessageBox.Show("Conteúdo Copiado para a Área de transferência");
                Clipboard.SetText(lF.Text + Environment.NewLine + F.ToString("F3") + Environment.NewLine + lS.Text + Environment.NewLine + S.ToString("F3"));
            }
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    CopyToClipboard();
                }
            }
            public TPhasorViewForm(TInstance.TNODE.TLINE Line)
            {
                S = new TPhasorControl(Line.DE.V3F.Sequence, Line.I3F.Sequence, Line.DE.Instance.Cursor);
                F = new TPhasorControl(Phasors(Line.DE.V3F), Phasors(Line.I3F), Line.DE.Instance.Cursor);
                F.Parent = this;
                S.Parent = this;
                lF = new Label();
                lF.Parent = this;
                lS = new Label();
                lS.Parent = this;

                S.BorderStyle = F.BorderStyle = BorderStyle.FixedSingle;

                Text = Line.Name;
                lF.Text = "Phasores";
                lS.Text = "Sequences";
                lS.TextAlign = lF.TextAlign = ContentAlignment.MiddleCenter;
                this.Width = 300 * 2;
                this.Height = 300 + lS.Height;
                base.Layout += (object sender, LayoutEventArgs e) =>
                {
                    lF.Top = 0;
                    lF.Width = Width / 2;
                    lF.Left = 0;

                    F.Top = lF.Height;
                    F.Left = 0;
                    F.Height = Height - lF.Height;
                    F.Width = Width / 2;

                    lS.Top = 0;
                    lS.Width = Width / 2;
                    lS.Left = Width / 2;

                    S.Top = lS.Height;
                    S.Left = Width / 2;
                    S.Height = Height - lS.Height;
                    S.Width = Width / 2;
                };
            }
            public partial class TPhasorControl : UserControl
            {
                protected CMath.TPhasor[] V, I;
                protected TTimeCursor TimeCursor;
                Pen PV, PI;
                public TPhasorControl(CMath.TPhasor[] V, CMath.TPhasor[] I, TTimeCursor Cursor)
                {
                    this.V = V;
                    this.I = I;
                    this.TimeCursor = Cursor;
                    Cursor.Change += (TTimeCursor Sender, DateTime Old) =>
                    {
                        this.Invalidate();
                    };
                    PI = new Pen(Color.Black, -1);
                    PI.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    PV = (Pen)PI.Clone();
                    PV.Width = 2;
                    PV.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
                }
                protected override void OnResize(EventArgs e)
                {
                    Invalidate();
                    base.OnResize(e);
                }
                protected override void OnPaint(PaintEventArgs e)
                {
                    e.Graphics.Clear(Color.White);
                    DateTime C = TimeCursor.Cursor;
                    e.Graphics.TranslateTransform(Width / 2, Height / 2);
                    double Max = 0;
                    double Cr = 0;
                    if (I != null)
                    {
                        foreach (CMath.TPhasor Phasor in I)
                        {
                            Max = Math.Max(Phasor[C].Magnitude, Max);
                        }
                        e.Graphics.DrawString("I:" + Max.ToString("F2"), Font, Brushes.Black, -Width / 3, -Height / 2);
                        Cr = Max / (Math.Min(Height, Width) / 3);
                        foreach (CMath.TPhasor Phasor in I)
                        {
                            Complex X = (Phasor[C] / Cr);
                            PI.Color = Phasor.Channel.ph.Color;
                            e.Graphics.DrawLine(PI, 0, 0, (float)X.Real, -(float)X.Imaginary);
                        }
                    }
                    if (V != null)
                    {
                        Max = 0;
                        foreach (CMath.TPhasor Phasor in V)
                        {
                            Max = Math.Max(Phasor[C].Magnitude, Max);
                        }
                        e.Graphics.DrawString("V:" + Max.ToString("F2"), Font, Brushes.Black, Width / 3, -Height / 2, new StringFormat(StringFormatFlags.DirectionRightToLeft));
                        Cr = Max / (Math.Min(Height, Width) / 3);
                        foreach (CMath.TPhasor Phasor in V)
                        {
                            Complex X = (Phasor[C] / Cr);
                            PV.Color = Phasor.Channel.ph.Color;
                            e.Graphics.DrawLine(PV, 0, 0, (float)X.Real, -(float)X.Imaginary);
                        }
                    }
                    int L = Math.Min(Height, Width);
                    e.Graphics.DrawEllipse(Pens.Aquamarine, -L / 3, -L / 3, L * 2 / 3, L * 2 / 3);
                    base.OnPaint(e);
                }
                public string ToString(string format = "")
                {
                    DateTime C = TimeCursor.Cursor;
                    string S = "";
                    if (V != null)
                    {
                        S = "V:" + Environment.NewLine;
                        foreach (CMath.TPhasor Phasor in V)
                        {
                            S = S + Phasor.Channel.ph + '\t';
                            S = S + (Phasor[C]).Real.ToString(format) + '\t' + (Phasor[C]).Imaginary.ToString(format) + Environment.NewLine;
                        }
                    }
                    if (I != null)
                    {
                        S = S + "I:" + Environment.NewLine;
                        foreach (CMath.TPhasor Phasor in I)
                        {
                            S = S + Phasor.Channel.ph + '\t';
                            S = S + (Phasor[C]).Real.ToString(format) + '\t' + (Phasor[C]).Imaginary.ToString(format) + Environment.NewLine;
                        }
                    }
                    return S;
                }
                protected override void OnGotFocus(EventArgs e)
                {
                    base.OnGotFocus(e);
                    Parent.Focus();
                }
            }
            internal static CMath.TPhasor[] Phasors(List<TComtrade.AChannel> Channels)
            {
                CMath.TPhasor[] P = new TPhasor[Channels.Count];
                for (int i = 0; i < Channels.Count; i++)
                {
                    P[i] = Channels[i].Phasor;
                }
                return P;
            }
        }
        public TPhasorView()
        {
            ToolStripButton FaultLocationButton = new System.Windows.Forms.ToolStripButton();

            // 
            // CursorStripButton
            // 
            FaultLocationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            FaultLocationButton.Image = Resource.copyToolStripMenuItem_Image;
            FaultLocationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            FaultLocationButton.Name = "CursorStripButton";
            FaultLocationButton.Size = new System.Drawing.Size(23, 22);
            FaultLocationButton.Text = "Cursor";
            FaultLocationButton.ToolTipText = "Set Cursor on time line";
            FaultLocationButton.Click += (object sender, EventArgs e) =>
            {
                TPhasorViewForm Form_iga = (TPhasorViewForm)Active;
                Form_iga.CopyToClipboard();
            };
            ToolStrip.Text = "ToolStrip";
            ToolStrip.Enabled = false;
            ToolStrip.AllowDrop = true;
            ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { FaultLocationButton });
        }
        private ToolStrip ToolStrip = new ToolStrip();
        private class TMyResult : TResult
        {
            TPhasorViewForm MyForm;
            public TMyResult(TPhasorViewForm MyForm)
            {
                this.MyForm =MyForm;
            }
            public void Show(Form ParentForm)
            {
                MyForm.Show(ParentForm);
            }
            public object Data
            {
                get
                {
                    return this;
                }
            }
        }
        static TPhasorViewForm Active;
        public TResult Execute(TInstance.TNODE.TLINE Linha)
        {
            TPhasorViewForm MyForm = new TPhasorViewForm(Linha);
            MyForm.Activated += (object sender, EventArgs e) =>
            {
                MenuTool.Enabled = true;
                Active = (TPhasorViewForm)sender;
            };
            MyForm.Deactivate += (object sender, EventArgs e) =>
            {
                MenuTool.Enabled = false;
            };
            return new TMyResult(MyForm);
        }
    }
}