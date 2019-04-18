using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using PowerSystem.IEEEComtrade;
using PowerSystem.NetWork;

namespace PowerSystem.WaveView
{
    public class TGrafico : UserControl
    {
        private TInstance _Instance;
        public TInstance Instance
        {
            get
            {
                return _Instance;
            }
            set
            {
                if (value != _Instance)
                {
                    _Instance = value;
                    _Instance.LNode_Using.Added += new TInstance.ListWithEvents<TInstance.TNODE>.OnAdd(LNode_Using_Added);
                    _Instance.LLine_Using.Added += new TInstance.ListWithEvents<TInstance.TNODE.TLINE>.OnAdd(LLine_Using_Added);
                    _Instance.LNode_Using.ForEach((TInstance.TNODE Node) => LNode_Using_Added(Node));
                    _Instance.LLine_Using.ForEach((TInstance.TNODE.TLINE Line) => LLine_Using_Added(Line));
                }
            }
        }
        void LLine_Using_Added(TInstance.TNODE.TLINE Item)
        {
            SuspendLayout();
            TLinhaGroup Group = new WaveView.TLinhaGroup(Item.I3F, Item.D, this, Item);
            this.Linhas.Add(Group);
            this.Grupos.Add(Group);
            if (Group.Visible)
            {
                foreach (var Barra in this.Barras)
                {
                    if (Group.RefBarra == Barra.RefBarra)
                    {
                        Barra.Visible = true;
                        break;
                    }
                }
            }
            ResumeLayout();
        }
        void LNode_Using_Added(TInstance.TNODE Item)
        {
            SuspendLayout();
            TBarraGroup Group = new TBarraGroup(Item.V3F, new List<TComtrade.DChannel>(), this, Item);
            this.Barras.Add(Group);
            this.Grupos.Add(Group);
            Group.Visible = false;
            foreach (var Linha in this.Linhas)
            {
                if (Group.RefBarra == Linha.RefBarra)
                {
                    Group.Visible = true;
                    break;
                }
            }
            ResumeLayout();
        }
        public TTimeCursor ActiveCursor;
        public List<TLinhaGroup> Linhas = new List<TLinhaGroup>();
        public List<TBarraGroup> Barras = new List<TBarraGroup>();
        public List<TBaseGroup> Grupos = new List<TBaseGroup>();

        public int DigitalWidth = 10;
        public int TimeRuleWidht = 30;

        KeyEventArgs KeyBoard = new KeyEventArgs(Keys.None);
        MouseEventArgs Mouse = new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0);

        private System.Drawing.Size MSize = new Size();

        public TGrafico(TInstance Instance)
        {
            this.Instance = Instance;
            //Define as propriedades de Desenho
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            //Adiciona eventos ao mexer com o tempo mostrado
            Instance.Time.Change += new TTime.OnChange(Time_Change);
            Instance.Time.Reference.Change += new TTime.OnChange(Time_ChangeReference);
            Instance.Anchor.Change += new TTimeCursor.OnChange(TimeAnchor_Change);
            Instance.Cursor.Change += new TTimeCursor.OnChange(RepaintCursores);
            Instance.Anchor.Change += new TTimeCursor.OnChange(RepaintCursores);
        }
        void RepaintCursores(TTimeCursor Sender, DateTime Old)
        {
            if (Sender==ActiveCursor)
            {
                int x = (int)(Parent.Width * (Sender.Cursor - Instance.Time.A).Ticks / (Instance.Time.B - Instance.Time.A).Ticks);
                Invalidate(new Rectangle(x - (int)Sender.pen.Width - 1, 0, (int)Sender.pen.Width + 2, Parent.Height));
                x = (int)(Parent.Width * (Old - Instance.Time.A).Ticks / (Instance.Time.B - Instance.Time.A).Ticks);
                Invalidate(new Rectangle(x - (int)Sender.pen.Width - 1, 0, (int)Sender.pen.Width + 2, Parent.Height));
            }
        }
        void PaintCursores(TTimeCursor Sender, Graphics g)
        {
            if (Sender.Enabled)
            {
                int x = (int)(Parent.Width * (Sender.Cursor - Instance.Time.A).Ticks / (Instance.Time.B - Instance.Time.A).Ticks);
                g.DrawLine(Sender.pen, x, 0, x, Parent.Height - (int)(Parent.Font.Height * 1.5f));
            }
        }
        void TimeAnchor_Change(TTimeCursor Sender, DateTime Old)
        {
            _Instance.Time.Reference.Value = _Instance.Anchor.Cursor;
        }
        void Time_ChangeReference(TTime Sender)
        {
            //Invalida a área que a régua está ocupando.
            Invalidate(new Rectangle(0, Grupos[Grupos.Count - 1].Top + Grupos[Grupos.Count - 1].Height, Width, Height - Grupos[Grupos.Count - 1].Top + Grupos[Grupos.Count - 1].Height));
        }
        void Time_Change(TTime Sender)
        {
            UpdateMemory();
            Invalidate();
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                DigitalWidth = (int)(1.1f * base.Font.Height);
                TimeRuleWidht = (int)(2.5f * base.Font.Height);
            }
        }
        private MouseButtons MButtons;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Grupos.Count > 0)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Middle || (MButtons == (MouseButtons.Left | MouseButtons.Right)))
                {
                    if (Math.Abs((e.X - Mouse.X)) * 100 > Width)
                    {
                        long ticks = (_Instance.Time.B - _Instance.Time.A).Ticks * (Mouse.X - e.X) / Width;
                        _Instance.Time.SetAandB(_Instance.Time.A.AddTicks(ticks), _Instance.Time.B.AddTicks(ticks));
                        Mouse = e;
                    }
                }
                else if (e.Button == MouseButtons.Left)
                {
                    if (ActiveCursor != null)
                    {
                        ActiveCursor.Cursor = _Instance.Time.A.AddTicks((_Instance.Time.B - _Instance.Time.A).Ticks * e.X / Width);
                    }
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MButtons = MButtons | e.Button;
            Mouse = e;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Grupos.Count > 0)
            {
                if (MButtons == MouseButtons.Left)
                {
                    if (ActiveCursor != null)
                    {
                        ActiveCursor.Cursor = _Instance.Time.A.AddTicks((_Instance.Time.B - _Instance.Time.A).Ticks * e.X / Width);
                        ActiveCursor.Enabled = true;
                    }
                }
                else if (MButtons == MouseButtons.Right)
                {
                    ContextMenuStrip myMenu = new ContextMenuStrip();
                    ToolStripMenuItem Mi2 = new ToolStripMenuItem("Expand");
                    foreach (TBarraGroup Item in Barras)
                    {
                        if (Item.Visible)
                        {
                            if ((Item.AItems.Top + Item.Top) <= e.Y && (Item.Top + Item.AItems.Height + Item.AItems.Top) >= e.Y)
                            {
                                myMenu.Items.AddRange(Item.MenuItens);
                            }
                        }
                    }
                    foreach (TLinhaGroup Item in Linhas)
                    {
                        if (Item.Visible)
                        {
                            if ((Item.AItems.Top + Item.Top) <= e.Y && (Item.Top + Item.AItems.Height + Item.AItems.Top) >= e.Y)
                            {
                                myMenu.Items.AddRange(Item.MenuItens);
                                myMenu.Items.Add(new ToolStripSeparator());
                                foreach (Methods.TMethod<TInstance.TNODE.TLINE> Method in Methods.TMethodManager.Method_Line)
                                {
                                    myMenu.Items.Add(Method.Text).Click += new EventHandler(
                                            (object sender, EventArgs e2) =>
                                            {
                                                foreach (Methods.TMethod<TInstance.TNODE.TLINE> Methodin in  Methods.TMethodManager.Method_Line)
                                                {
                                                    if (Methodin.Text == ((ToolStripItem)sender).Text)
                                                    {
                                                        Methodin.Execute(Item.Line).Show(this.ParentForm);
                                                    }
                                                }
                                            }
                                        );
                                }
                                break;
                            }
                        }
                    }
                    myMenu.Show(this, e.Location);
                }
                MButtons = MButtons ^ e.Button;
            }
            base.OnMouseUp(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            KeyBoard = e;
            base.OnKeyUp(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyBoard = e;
            base.OnKeyDown(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (KeyBoard.Control)
            {
                foreach (TBaseGroup Item in Grupos)
                {
                    if (Item.Visible)
                    {
                        if ((Item.AItems.Top + Item.Top) <= e.Y && (Item.Top + Item.AItems.Height + Item.AItems.Top) >= e.Y)
                        {
                            Item.A *= 1 + e.Delta / 1200.0f;
                            //Redesenha apenas a região do Item a ser escalonado
                            Invalidate(new Rectangle(Item.Left, Item.Top, Item.Width, Item.Height));
                            break;
                        }
                    }
                }
            }
            else
            {
                float k = 1 / (1 - Math.Min(e.Delta / 1200.0f,0.8f));
                long A = (_Instance.Time.A - _Instance.Time.Start).Ticks;
                long B = (_Instance.Time.B - _Instance.Time.Start).Ticks;
                if (B - A > 40 || k < 1)
                {
                    double C = A + (B - A) * e.X / Width;
                    A = (long)(((k - 1) * C + A * (1 - k)) / k);
                    B = (long)(((k - 1) * C + B * (1 - k)) / k);
                    _Instance.Time.SetAorB(_Instance.Time.A.AddTicks(A), _Instance.Time.B.AddTicks(B));
                }
            }
            base.OnMouseWheel(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                System.Drawing.Drawing2D.GraphicsState X = e.Graphics.Save();
                foreach (TBaseGroup Group in Grupos)
                {
                    if (Group.Visible)
                    {
                        System.Drawing.Drawing2D.GraphicsState M = e.Graphics.Save();
                        e.Graphics.SetClip(new Rectangle(Group.Left, Group.Top, Group.Width, Group.Height), System.Drawing.Drawing2D.CombineMode.Intersect);
                        if (!e.Graphics.IsClipEmpty)
                        {
                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(Group.Left, Group.Top, Group.Width, Group.Height - 1));
                            e.Graphics.TranslateTransform(0, Group.Top);
                            Group.Paint(this, e.Graphics);
                            e.Graphics.TranslateTransform(0, -Group.Top);
                        }
                        e.Graphics.Restore(M);
                    }
                }
                //Recalcula a régua para ter as dimensões que sobraram (isso se deve aos arredondamentos feitos na altura dos gráficos)
                int TimeRuleWidht;
                if (Grupos.Count > 0)
                {
                    TimeRuleWidht = Height - (Grupos[Grupos.Count - 1].Top + Grupos[Grupos.Count - 1].Height) - 1;
                }
                else
                {
                    TimeRuleWidht = Height / 4;
                }
                e.Graphics.SetClip(new Rectangle(0, Height - TimeRuleWidht, Width, TimeRuleWidht), System.Drawing.Drawing2D.CombineMode.Intersect);
                if (!e.Graphics.IsClipEmpty)
                {
                    using (StringFormat F = new StringFormat())
                    {
                        //e.Graphics.Clip.Intersect(new Rectangle(0, Height - TimeRuleWidht, Width, TimeRuleWidht));
                        long ticks = (_Instance.Time.B - _Instance.Time.A).Ticks;
                        F.Alignment = StringAlignment.Center;
                        e.Graphics.Clear(SystemColors.Control);
                        if (_Instance.Time.Reference.Fixed)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                DateTime T2 = _Instance.Time.A.AddTicks(ticks * (2 * i + 1) / 12);
                                e.Graphics.DrawString((T2 - _Instance.Time.Reference.Value).TotalSeconds.ToString("F3"), Font, Brushes.Black, Width * (2 * i + 1) / 12, Height - Font.Height, F);
                                e.Graphics.DrawLine(Pens.Black, new Point(Width * (2 * i + 1) / 12, Height - TimeRuleWidht + (int)Font.Height / 2), new Point(Width * (2 * i + 1) / 12, Height - (int)Font.Height * 3 / 2));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                DateTime T2 = _Instance.Time.A.AddTicks(ticks * (2 * i + 1) / 12);
                                e.Graphics.DrawString(T2.Second + "." + T2.Millisecond.ToString("D3"), Font, Brushes.Black, Width * (2 * i + 1) / 12, Height - Font.Height, F);
                                e.Graphics.DrawLine(Pens.Black, new Point(Width * (2 * i + 1) / 12, Height - TimeRuleWidht + (int)Font.Height / 2), new Point(Width * (2 * i + 1) / 12, Height - (int)Font.Height * 3 / 2));
                            }
                        }
                    }
                }
                e.Graphics.Restore(X);
                PaintCursores(Instance.Cursor, e.Graphics);
                PaintCursores(Instance.Anchor, e.Graphics);
            }
            catch (Exception eX)
            {
                e.Graphics.DrawString(eX.Message, Font, Brushes.Black, 0, 0);
            }
            base.OnPaint(e);
        }
        public void UpdateMemory(bool AutoScale = false)
        {
            foreach (TBaseGroup Group in Grupos)
            {
                if (Group.Visible)
                {
                    Group.UpdateBuffer(_Instance.Time, Width);
                    if (this.AutoScale | AutoScale)
                    {
                        Group.Rescale();
                    }
                }
            }
        }
        internal void Rescale()
        {
            foreach (TBaseGroup Group in Grupos)
            {
                if (Group.Visible)
                {
                    Group.Rescale();
                }
            }
            Invalidate();
        }
        internal void IsoScale()
        {
            MessageBox.Show("IsoScale: Ainda não implementado, deve ser implementado após resolver a questão com unidades que deve ser resolvida após a implementaçõa do sistema PU");
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            int dI = 0, aI = 0;
            foreach (TBaseGroup Group in Grupos)
            {
                if (Group.Visible)
                {
                    dI += Group.DItems.VisibleItens;
                    aI += Group.Expand ? Group.AItems.VisibleItens : (Group.AItems.VisibleItens > 0 ? 1 : 0);
                }
            }
            int T = 0, H = aI == 0 ? 0 : (Height - TimeRuleWidht) / aI, L = aI == 0 ? 0 : (Height - TimeRuleWidht - dI * DigitalWidth) / aI;
            //Calculo do Posicionamento dos Desenhos
            foreach (TBaseGroup Group in Grupos)
            {
                Group.AItems.Top = 0;
                Group.AItems.Height = Group.Expand ? Group.AItems.VisibleItens * L : (((Group.AItems.VisibleItens > 0) & Group.Visible) ? L : 0);
                Group.AItems.Left = 0;
                Group.AItems.Widht = Width;

                Group.DItems.Top = Group.AItems.Height;
                Group.DItems.Height = Group.Visible ? Group.DItems.VisibleItens * DigitalWidth : 0;
                Group.DItems.Left = 0;
                Group.DItems.Widht = Width;

                Group.Top = T;
                Group.Height = Group.AItems.Height + Group.DItems.Height;
                Group.Left = 0;
                Group.Width = Width;

                T += Group.Height;
            }
            //if (MSize.Width != Size.Width) 
            UpdateMemory();
            MSize = Size;
            Invalidate();
            base.OnLayout(e);
        }
        public bool AutoScale;
    }
}
