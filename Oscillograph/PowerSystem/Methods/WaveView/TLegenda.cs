using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PowerSystem.NetWork;

namespace PowerSystem.WaveView
{
    public class TLegenda : UserControl
    {
        public enum TLegendMode { None, Fundamental, Instantaneus, Secondary, PU };
        private TLegendMode _LegendMode;
        public TLegendMode LegendMode
        {
            get
            {
                return _LegendMode;
            }
            set
            {
                if (value != TLegendMode.None)
                {
                    Grafico.Instance.Cursor.Change += new TTimeCursor.OnChange(Cursor_Change);
                }
                else if (_LegendMode != TLegendMode.None)
                {
                    Grafico.Instance.Cursor.Change -= new TTimeCursor.OnChange(Cursor_Change);
                }
                _LegendMode = value;
                Invalidate();
            }
        }
        void Cursor_Change(TTimeCursor Sender, DateTime OldValue)
        {
            Invalidate();
        }
        TGrafico Grafico;
        public TLegenda(TGrafico Grafico)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.Grafico = Grafico;
            Grafico.Instance.Time.Change += new TTime.OnChange(Time_Change);
            Grafico.Instance.Time.Reference.Change += new TTime.OnChange(Time_ChangeReference);
            Grafico.Layout += (object sender, LayoutEventArgs e) =>
            {
                Invalidate();
            };
            MouseUp += new MouseEventHandler(Grafico_MouseUp);
            LegendMode = TLegendMode.Instantaneus;
        }
        void Grafico_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip myMenu = new ContextMenuStrip();
                if (e.Location.X > Font.Height * 3 / 2)
                {
                    ToolStripMenuItem Exhibicion = new ToolStripMenuItem("Exhibicion");

                    ToolStripMenuItem None = new ToolStripMenuItem("None");
                    None.Click += (object sender2, EventArgs e2) => LegendMode = TLegendMode.None;
                    None.Checked = LegendMode == TLegendMode.None;
                    Exhibicion.DropDownItems.Add(None);

                    ToolStripMenuItem Fundamental = new ToolStripMenuItem("Fundamental");
                    Fundamental.Click += (object sender2, EventArgs e2) => LegendMode = TLegendMode.Fundamental;
                    Fundamental.Checked = LegendMode == TLegendMode.Fundamental;
                    Exhibicion.DropDownItems.Add(Fundamental);

                    ToolStripMenuItem Primary = new ToolStripMenuItem("Primária Instantâneo");
                    Primary.Click += (object sender2, EventArgs e2) => LegendMode = TLegendMode.Instantaneus;
                    Primary.Checked = LegendMode == TLegendMode.Instantaneus;
                    Exhibicion.DropDownItems.Add(Primary);

                    ToolStripMenuItem Secondary = new ToolStripMenuItem("Secondario Fundamental");
                    Secondary.Click += (object sender2, EventArgs e2) => LegendMode = TLegendMode.Secondary;
                    Secondary.Checked = LegendMode == TLegendMode.Secondary;
                    Exhibicion.DropDownItems.Add(Secondary);

                    ToolStripMenuItem PU = new ToolStripMenuItem("PU Fundamental");
                    PU.Click += (object sender2, EventArgs e2) => LegendMode = TLegendMode.PU;
                    PU.Checked = LegendMode == TLegendMode.PU;
                    Exhibicion.DropDownItems.Add(PU);

                    myMenu.Items.Add(Exhibicion);
                    myMenu.Items.Add(new ToolStripSeparator());

                    foreach(TBaseGroup Group in Grafico.Grupos){
                        myMenu.Items.Add(Group.MenuItens[1]);
                    }
                }
                //foreach (TBaseGroup Item in Grafico.Groups)
                //{
                //    if (Item.Visible)
                //    {
                //        if ((Item.Top) <= e.Y && (Item.Top + Item.Height) >= e.Y)
                //        {
                //            if (e.Location.X < Font.Height * 3 / 2)
                //            {
                //                foreach (TBaseGroup Item2 in Grafico.Groups)
                //                {
                //                    if (Item2.Comtrade == Item.Comtrade)
                //                    {
                //                        myMenu.Items.Add(Item2.Menu);
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                if (Item.Line != null)
                //                {
                //                    myMenu.Items.Add("Show Phasors").Click += (object sender2, EventArgs e2) =>
                //                    {
                //                        (new PowerSystem.Methods.TPhasorForm(Item.Line)).Show(Parent);
                //                    };
                //                    myMenu.Items.Add(new ToolStripSeparator());
                //                }
                //                myMenu.Items.Add(Item.Menu);
                //            }
                //        }
                //    }
                //}
                myMenu.Show(this, e.Location);
            }
        }
        void Time_ChangeReference(TTime Sender)
        {
            using (System.Drawing.Graphics g = this.CreateGraphics())
            {
                g.Clip = new Region(new Rectangle(0, Height - Font.Height, Width, Font.Height));
                g.Clear(BackColor);
                DrawTime(g);
            }
        }
        void Time_Change(TTime Sender)
        {
            if (!Grafico.Instance.Time.Reference.Fixed)
            {
                using (System.Drawing.Graphics g = this.CreateGraphics())
                {
                    g.Clip = new Region(new Rectangle(0, Height - Font.Height, Width, Font.Height));
                    g.Clear(BackColor);
                    DrawTime(g);
                }
            }
        }
        private void DrawTime(System.Drawing.Graphics g)
        {
            using (StringFormat F = new StringFormat(StringFormatFlags.DirectionRightToLeft))
            {
                if (Grafico.Instance.Time.Reference.Fixed)
                {
                    g.DrawString("+" + Grafico.Instance.Time.Reference.Value.ToLongTimeString() + " " + Grafico.Instance.Time.Reference.Value.ToShortDateString(), Font, Brushes.Black, new Point(Width, Height - Font.Height), F);
                }
                else
                {
                    g.DrawString(":" + Grafico.Instance.Time.A.ToShortTimeString() + " " + Grafico.Instance.Time.A.ToShortDateString(), Font, Brushes.Black, new Point(Width, Height - Font.Height), F);
                    //g.DrawString(Grafico.Time.A.ToShortDateString(), Font, Brushes.Black, new Point(Width, Height - 2*Font.Height), F);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            using (StringFormat F = new StringFormat(StringFormatFlags.DirectionVertical))
            {
                F.Alignment = StringAlignment.Center;
                foreach (TBaseGroup Group in Grafico.Grupos)
                {
                    int n = 0;
                    if ((Group.AItems.VisibleItens > 0 || Group.DItems.VisibleItens > 0) && Group.Visible)
                    {
                        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(Font.Height * 3 / 2, Group.Top, Width - Font.Height * 3 / 2, Group.Height));
                        if (e.Graphics.MeasureString(Group.ccbm, Font).Width < Group.Height)
                        {
                            foreach (TDItem DItem in Group.DItems)
                            {
                                if (DItem.Visible)
                                {
                                    e.Graphics.DrawString(DItem.Channel.ch_id, Font, Brushes.Black, Font.Height * 7 / 2, Group.Top + Group.DItems.Top + Grafico.DigitalWidth * n);
                                    n = n + 1;
                                }
                            }
                            n = 0;
                            if (Font.Height * Group.AItems.VisibleItens * 3 / 2 < Group.AItems.Height)
                            {
                                foreach (TAItem AItem in Group.AItems)
                                {
                                    if (AItem.Visible)
                                    {
                                        string L;
                                        if (LegendMode != TLegendMode.None)
                                        {
                                            double V=0;
                                            if (LegendMode == TLegendMode.Instantaneus)
                                            {
                                                V = AItem.Channel[Grafico.Instance.Cursor.Cursor];
                                            }
                                            else if (LegendMode == TLegendMode.Fundamental)
                                            {
                                                V = AItem.Channel.Phasor[Grafico.Instance.Cursor.Cursor].Magnitude;
                                            }
                                            else if (LegendMode == TLegendMode.Secondary)
                                            {
                                                V = AItem.Channel.Phasor.Reader(CMath.TPhasor.TReaderMode.Secondary)[Grafico.Instance.Cursor.Cursor].Magnitude;
                                                    //AItem.Channel[Grafico.Instance.Cursor.Cursor] * (AItem.Channel.PS == IEEEComtrade.TComtrade.AChannel.TPS.P ? AItem.Channel.secondary / AItem.Channel.primary : 1);
                                            }
                                            else if (LegendMode == TLegendMode.PU)
                                            {
                                                //Ainda não implementado o Channel Reader
                                                V = AItem.Channel.Phasor.Reader(CMath.TPhasor.TReaderMode.PU)[Grafico.Instance.Cursor.Cursor].Magnitude;
                                            }
                                            else if (LegendMode == TLegendMode.Fundamental)
                                            {
                                                V = AItem.Channel.Phasor[Grafico.Instance.Cursor.Cursor].Magnitude;
                                            }

                                            if (V < 0)
                                            {
                                                L = (V).ToString("000.00") + ' ' + AItem.Channel.uu + '\t';
                                            }
                                            else
                                            {
                                                L = (V).ToString(" 000.00") + ' ' + AItem.Channel.uu + '\t';
                                            }
                                        }
                                        else
                                        {
                                            L = "";
                                        }
                                        e.Graphics.DrawString(L + AItem.Channel.ch_id, Font, AItem.pen.Brush, Font.Height * 7 / 2, Group.Top + Group.AItems.Top + Font.Height * n + (Group.AItems.Height / 2 - Group.AItems.VisibleItens * Font.Height / 2));
                                        n = n + 1;
                                    }
                                }
                            }
                            else if (Group.AItems.VisibleItens > 0)
                            {
                                e.Graphics.DrawString("ANALOGS", Font, Brushes.Red, Font.Height * 7 / 2, Group.Top + Group.AItems.Top + (Group.AItems.Height - Font.Height) / 2);
                            }
                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(Font.Height * 3 / 2, Group.Top, Font.Height * 3 / 2, Group.Height));
                            e.Graphics.DrawString(Group.ccbm, Font, Brushes.Black, new Point(Font.Height * 3 / 2, Group.Top + Group.Height / 2), F);
                        }
                        else if (Group.Height > Font.Height)
                        {
                            e.Graphics.DrawString(Group.ccbm, Font, Brushes.Black, new Point(Font.Height * 5 / 2, Group.Top + (Group.Height - Font.Height) / 2));
                        }
                    }
                }
                if (Grafico.Grupos.Count > 0)
                {
                    int SHeight = 0, i = 0;
                    for (int j = 0; j < Grafico.Grupos.Count; j++)
                    {
                        if ((Grafico.Grupos[j].AItems.VisibleItens > 0 || Grafico.Grupos[j].DItems.VisibleItens > 0) && Grafico.Grupos[j].Visible)
                        {
                            if (Grafico.Grupos[i].RefBarra.Name != Grafico.Grupos[j].RefBarra.Name)
                            {
                                e.Graphics.DrawString(Grafico.Grupos[i].RefBarra.Name, Font, Brushes.Black, new Point(0, Grafico.Grupos[i].Top + SHeight / 2), F);
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, Grafico.Grupos[i].Top, Font.Height * 3 / 2, SHeight));
                                i = j;
                                SHeight = 0;
                            }
                            SHeight += Grafico.Grupos[j].Height;
                        }
                    }
                    e.Graphics.DrawString(Grafico.Grupos[i].RefBarra.Name, Font, Brushes.Black, new Point(0, Grafico.Grupos[i].Top + SHeight / 2), F);
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, Grafico.Grupos[i].Top, Font.Height * 3 / 2, SHeight));
                }
                DrawTime(e.Graphics);
            }
            base.OnPaint(e);
        }
    }
}
