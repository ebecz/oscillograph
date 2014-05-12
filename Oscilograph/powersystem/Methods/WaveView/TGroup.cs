using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PowerSystem.IEEEComtrade;
using PowerSystem;

namespace PowerSystem.WaveView
{
    public class TBaseGroup
    {
        public TGrafico Parent;
        public float A = 0f;
        public int Top, Height, Width, Left;
        public abstract class MList<T> : List<T>
        {
            public int Top, Height, Widht, Left;
            abstract public int VisibleItens { get; }
        }
        public class MListAItem : MList<TAItem>
        {
            public override int VisibleItens
            {
                get
                {
                    int c = 0;
                    foreach (TAItem Item in this)
                    {
                        if (Item.Visible)
                        {
                            c++;
                        }
                    }
                    return c;
                }
            }
        }
        public class MListDItem : MList<TDItem>
        {
            internal void Paint(object sender, System.Drawing.Graphics g)
            {
                foreach (TDItem D in this)
                {
                    if (D.Visible)
                    {
                        D.Paint(sender, g, Height / VisibleItens);
                        g.TranslateTransform(0, Height / VisibleItens);
                    }
                }
            }
            public override int VisibleItens
            {
                get
                {
                    int c = 0;
                    foreach (TDItem Item in this)
                    {
                        if (Item.Visible)
                        {
                            c++;
                        }
                    }
                    return c;
                }
            }
        }
        public MListAItem AItems = new MListAItem();
        public MListDItem DItems = new MListDItem();
        internal void UpdateBuffer(TTime Time, int Witdh)
        {
            foreach (TAItem Item in AItems)
            {
                if (Item.Visible)
                {
                    Item.UpdateMemory(Time, Witdh);
                }
            }
            foreach (TDItem Item in DItems)
            {
                if (Item.Visible)
                {
                    Item.UpdateMemory(Time, Witdh);
                }
            }
            if (A == 0)
            {
                Rescale();
            }
        }
        internal void Paint(TGrafico sender, System.Drawing.Graphics g)
        {
            if (!float.IsInfinity(A) & AItems.VisibleItens > 0)
            {
                System.Drawing.Drawing2D.GraphicsState Y = g.Save();
                if (Expand)
                {
                    g.SetClip(new Rectangle(AItems.Left, AItems.Top, AItems.Widht, AItems.Height), System.Drawing.Drawing2D.CombineMode.Intersect);
                    //g.SetClip(new Rectangle(AItems.Left, AItems.Top, AItems.Widht / AItems.CountVisible, AItems.Height), System.Drawing.Drawing2D.CombineMode.Intersect);
                    g.TranslateTransform(0, AItems.Top + AItems.Height / (2 * AItems.VisibleItens));
                    foreach (TAItem Item in AItems)
                    {
                        if (Item.Visible)
                        {
                            //g.TranslateClip(0, AItems.Height / AItems.CountVisible);
                            System.Drawing.Drawing2D.GraphicsState G2 = g.Save();
                            g.ScaleTransform(1, A * AItems.Height / AItems.VisibleItens);
                            Item.Paint(sender, g);
                            g.Restore(G2);
                            g.TranslateTransform(0, AItems.Height / AItems.VisibleItens);
                        }
                    }
                }
                else
                {
                    g.SetClip(new Rectangle(AItems.Left, AItems.Top, AItems.Widht, AItems.Height), System.Drawing.Drawing2D.CombineMode.Intersect);
                    g.TranslateTransform(0, AItems.Top + AItems.Height / 2);
                    g.ScaleTransform(1, A * AItems.Height);
                    foreach (TAItem Item in AItems)
                    {
                        if (Item.Visible)
                        {
                            Item.Paint(sender, g);
                        }
                    }
                }
                g.Restore(Y);
            }
            if (DItems.VisibleItens > 0)
            {
                System.Drawing.Drawing2D.GraphicsState X = g.Save();
                g.SetClip(new Rectangle(DItems.Left, DItems.Top, DItems.Widht, DItems.Height), System.Drawing.Drawing2D.CombineMode.Intersect);
                g.TranslateTransform(0, DItems.Top);
                DItems.Paint(sender, g);
                g.Restore(X);
            }
        }
        private bool _Expand;
        public bool Expand
        {
            get
            {
                return _Expand;
            }
            set
            {
                if (value != _Expand)
                {
                    _Expand = value;
                    Parent.PerformLayout();
                }
            }
        }
        public void Rescale()
        {
            float fMax = 0;
            foreach (TAItem Item in AItems)
            {
                if (Item.Visible)
                {
                    fMax = Math.Max(Item.Max * Item.Channel.a + Item.Channel.b, fMax);
                }
            }
            A = 1 / (2.5f * fMax);
        }
        private bool _Visible;
        public string ccbm;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    Parent.PerformLayout();
                }
            }
        }
        public void Add(TAItem Item)
        {
            AItems.Add(Item);
        }
        public void Add(TDItem Item)
        {
            DItems.Add(Item);
        }
        public TBaseGroup(List<TComtrade.AChannel> AChannels, List<TComtrade.DChannel> DChannels, TGrafico Parent,TInstance.TNODE Barra)
        {
            this.Parent = Parent;
            this.RefBarra = Barra;
            foreach (TComtrade.AChannel AChannel in AChannels)
            {
                TAItem AItem = new TAItem(AChannel, this);
                AItem.Visible = true;
                AItems.Add(AItem);
            }
            foreach (TComtrade.DChannel DChannel in DChannels)
            {
                TDItem DItem = new TDItem(DChannel, this);
                DItems.Add(DItem);
                DItem.Visible = !DChannel.Static;
            }
            Visible = AItems.VisibleItens != 0 || DItems.VisibleItens != 0;
        }
        public ToolStripItem[] MenuItens
        {
            get
            {
                ToolStripItem[]ToolStripMenu=new ToolStripItem[2];
                ToolStripMenuItem M0 = new ToolStripMenuItem("Expand");
                M0.CheckOnClick = true;
                M0.Checked = Expand;
                M0.Click += (object sender, EventArgs e2) =>
                {
                    Expand = ((ToolStripMenuItem)sender).Checked;
                };
                ToolStripMenuItem M1 = new ToolStripMenuItem(this.ccbm);
                M1.Checked = this.Visible;
                M1.CheckOnClick = true;
                M1.CheckedChanged += (object sender, EventArgs e) =>
                {
                    this.Visible = ((ToolStripMenuItem)sender).Checked;
                };
                foreach (TAItem AItem in AItems)
                {
                    M1.DropDownItems.Add(AItem.Menu);
                }
                if (DItems.Count != 0 && AItems.Count != 0)
                {
                    M1.DropDownItems.Add(new ToolStripSeparator());
                }
                foreach (TDItem DItem in DItems)
                {
                    M1.DropDownItems.Add(DItem.Menu);
                }
                ToolStripMenu[0] = M0;
                ToolStripMenu[1] = M1;
                return ToolStripMenu;
            }
        }
        public TInstance.TNODE RefBarra;
    }
    public class TLinhaGroup : TBaseGroup
    {
        public new ToolStripItem[] MenuItens
        {
            get
            {
                return base.MenuItens;
            }
        }
        public TInstance.TNODE.TLINE Line;
        public TLinhaGroup(List<TComtrade.AChannel> AChannels, List<TComtrade.DChannel> DChannels, TGrafico Parent, TInstance.TNODE.TLINE Line)
            : base(AChannels, DChannels, Parent,Line.DE)
        {
            this.Visible = false;
            foreach (TDItem Ditem in this.DItems)
            {
                this.Visible |= Ditem.Visible;
            }
            this.Line = Line;
            ccbm = Line.Name;
        }
    }
    public class TBarraGroup : TBaseGroup
    {
        public string Name
        {
            get
            {
                return RefBarra.Name;
            }
        }
        public new ToolStripItem[] MenuItens
        {
            get
            {
                return base.MenuItens;
            }
        }
        public TBarraGroup(List<TComtrade.AChannel> AChannels, List<TComtrade.DChannel> DChannels, TGrafico Parent, TInstance.TNODE Barra)
            : base(AChannels, DChannels, Parent, Barra)
        {
            this.RefBarra = Barra;
            ccbm = Barra.Name;
        }
    }
    public abstract class TBaseItem
    {
        public ToolStripMenuItem Menu
        {
            get
            {
                ToolStripMenuItem M = new ToolStripMenuItem(ch_id);
                M.Checked = Visible;
                M.CheckedChanged += (object sender, EventArgs e) =>
                {
                    ToolStripMenuItem thisMenu=(ToolStripMenuItem)sender;
                    this.Visible = thisMenu.Checked;
                };
                M.DropDownItems.Add("Funções").Enabled = false;
                M.CheckOnClick = true;
                return M;
            }
        }
        public TBaseGroup Parent;
        private bool _Visible = false;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    Parent.Parent.PerformLayout();
                }
            }
        }
        internal abstract void UpdateMemory(TTime Time, int Widht);
        public TBaseItem(TComtrade.TBaseChannel BaseChannel,TBaseGroup Parent)
        {
            this.Parent = Parent;
            this.BaseChannel = BaseChannel;
        }
        public TComtrade.TBaseChannel BaseChannel;
        public string ch_id
        {
            get
            {
                return BaseChannel.ch_id;
            }
        }
    }
    public class TAItem : TBaseItem
    {
        public TAItem(TComtrade.AChannel T, TBaseGroup Parent)
            : base(T, Parent)
        {
            Channel = T;
        }
        public TComtrade.AChannel Channel;
        private Point[] Points;
        public int Max;
        internal override void UpdateMemory(TTime Time, int Witdh)
        {
            double Rate = Witdh / (Time.B - Time.A).TotalSeconds;
            //Encontra o intervalo de tempo das amostras disponíveis
            DateTime A = Time.A > Channel.BaseComtrade.start_time ? (Time.A < Channel.BaseComtrade.end_time ? Time.A : Channel.BaseComtrade.end_time) : Channel.BaseComtrade.start_time;
            DateTime B = Time.B > Channel.BaseComtrade.start_time ? (Time.B < Channel.BaseComtrade.end_time ? Time.B : Channel.BaseComtrade.end_time) : Channel.BaseComtrade.start_time;
            //Em amostras em relação ao início do arquivo
            long a = (long)(Channel.BaseComtrade.samp[0] * (A - Channel.BaseComtrade.start_time).TotalSeconds);
            long b = (long)(Channel.BaseComtrade.samp[0] * (B - Channel.BaseComtrade.start_time).TotalSeconds);
            //Recalcula a largura do buffer proporcional a amostra disponível
            long n = (long)((B - A).TotalSeconds * Rate);
            if (n > 0)
            {
                //Aloca espaço para o buffer de imagem
                Points = new Point[n];
                //Calcula a distância em pixels das amostras puladas
                int d = (int)((A - Time.A).TotalSeconds * Rate);
                //Percorre preenchendo o buffer de imagem
                Max = 0;
                for (int k = 0; k < n; k++)
                {
                    Points[k].X = k + d;
                    Points[k].Y = -(int)(Channel.Values[a + (b - a) * k / n]);
                    Max = Math.Max(Max, Math.Abs(Points[k].Y));
                }
            }
            else
            {
                Points = null;
            }
        }
        internal void Paint(object sender, System.Drawing.Graphics g)
        {
            if (Points != null && Channel.a != 0)
            {
                System.Drawing.Drawing2D.GraphicsState Y = g.Save();
                {
                    pen.Color = Channel.ph.Color;
                    g.TranslateTransform(0, Channel.b);
                    g.ScaleTransform(1, Channel.a);
                    g.DrawLines(pen, Points);
                } g.Restore(Y);
            }
        }
        Pen ipen;
        public Pen pen
        {
            get
            {
                if (ipen == null)
                {
                    ipen = new Pen(this.Channel.ph.Color);
                }
                return ipen;
            }
        }
    }
    public class TDItem : TBaseItem
    {
        private class DStatus
        {
            public bool value;
            public int a, b;
            public DStatus(bool value, int a, int b)
            {
                this.value = value;
                this.a = a;
                this.b = b;
            }
        }
        private List<DStatus> LDS = new List<DStatus>();
        public TDItem(TComtrade.DChannel T, TBaseGroup Parent)
            : base(T, Parent)
        {
            Channel = T;
        }
        public TComtrade.DChannel Channel;
        internal override void UpdateMemory(TTime Time, int Width)
        {
            double Rate = Width / (Time.B - Time.A).TotalSeconds;
            LDS.Clear();
            //Encontra o intervalo de tempo das amostras disponíveis
            DateTime A = Time.A > Channel.BaseComtrade.start_time ? (Time.A < Channel.BaseComtrade.end_time ? Time.A : Channel.BaseComtrade.end_time) : Channel.BaseComtrade.start_time;
            DateTime B = Time.B > Channel.BaseComtrade.start_time ? (Time.B < Channel.BaseComtrade.end_time ? Time.B : Channel.BaseComtrade.end_time) : Channel.BaseComtrade.start_time;
            //Em amostras em relação ao início do arquivo (Adiciona uma amostra extra para garantir a visualização sem cortes)
            long a = (long)(Channel.BaseComtrade.samp[0] * (A - Channel.BaseComtrade.start_time).TotalSeconds);
            long b = (long)(1 + Channel.BaseComtrade.samp[0] * (B - Channel.BaseComtrade.start_time).TotalSeconds);
            b = b > (long)Channel.BaseComtrade.endsamp[0] ? (long)Channel.BaseComtrade.endsamp[0] : b;
            //Índice da primeira e última amostra dispónível
            if (a < b)
            {
                double R2 = Width / ((Time.B - Time.A).TotalSeconds * Channel.BaseComtrade.samp[0]);
                double i = (Width * ((A - Time.A).TotalSeconds / (Time.B - Time.A).TotalSeconds));
                long p = a;
                for (long k = a; k < b - 1; k++)
                {
                    if (Channel.Values[k] != Channel.Values[k + 1])
                    {
                        double W = (R2 * (k - p));
                        LDS.Add(new DStatus(Channel.Values[k], (int)i, (int)(W)));
                        i += W;
                        if (Channel.Values[p] != Channel.Values[k])
                        {
                            throw (new Exception("see:TDItem.UpdateBuffer, p!=k"));
                        }
                        p = k + 1;
                    }
                }
                LDS.Add(new DStatus(Channel.Values[p], (int)i, (int)(R2 * (b - p))));
            }
        }
        internal void Paint(object sender, System.Drawing.Graphics e, int Widht)
        {
            foreach (DStatus C in LDS)
            {
                e.FillRectangle(C.value ? Config.On: Config.Off, C.a, 0, C.b, Widht);
                e.DrawRectangle(Pens.Black, C.a, 0, C.b, Widht);
            }
        }
    }
}
