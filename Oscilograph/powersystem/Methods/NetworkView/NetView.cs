using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PowerSystem;

namespace PowerSystem.Methods.NetWorkView
{
    public class NetView : UserControl
    {
        ToolTip Tip = new ToolTip();
        public NetView():base()
        {
            Nodes = new NodeList(this);
            DoubleBuffered = true;
            Tip.AutomaticDelay = 500;
        }
        public class NodeList : List<NodeControl>
        {
            internal NetView Parent;
            public List<NodeControl> Systens = new List<NodeControl>();
            public NodeList(NetView Parent) : base()
            {
                this.Parent = Parent;
            }
            public new void Add(NodeControl Item)
            {
                bool Isolated = true;
                if (this.Exists((LookUpNode) => { return LookUpNode.BaseNode == Item.BaseNode; }))
                {
                    return;
                }
                else
                {
                    foreach (NodeControl Node in this)
                    {
                        foreach (TInstance.TNODE.TLINE Line in Node.BaseNode.LINES)
                        {
                            if (Line.PARA == Item.BaseNode)
                            {
                                Connection.assign(new Connection(Node, Item, Line));
                                Isolated = false;
                            }
                        }
                    }
                    if (Isolated)
                    {
                        Systens.Add(Item);
                    }
                    base.Add(Item);
                    Item.FlagRecursivity = Parent.FlagRecursivity;
                }
            }
            public void AddRange(NodeControl[] Itens)
            {
                foreach (NodeControl Node in Itens)
                {
                    Add(Node);
                }
            }
        }
        public NodeList Nodes;
        bool FlagRecursivity = false;
        NodeControl Dragging, Highlight;
        public int X=0, Y=0;
        float dW=1;
        KeyEventArgs None = new KeyEventArgs(Keys.None), Keyboard = new KeyEventArgs(Keys.None);
        MouseEventArgs Mouse = new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0);
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(X, Y);
            e.Graphics.ScaleTransform(dW, dW);
            FlagRecursivity = !FlagRecursivity;
            foreach (NodeControl Node in Nodes.Systens)
            {
                Node.Paint(e, this, FlagRecursivity);
            }
            base.OnPaint(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keyboard = e;
            if (e.KeyCode == Keys.T)
            {
                if (Highlight != null)
                {
                    AutoOrganize(Highlight);
                    Invalidate();
                }
                else
                {
                    Auto();
                }
            }
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            Keyboard = None;
            base.OnKeyUp(e);
        }
        bool Middle_Simulated = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button == System.Windows.Forms.MouseButtons.Left && Mouse.Button == System.Windows.Forms.MouseButtons.Right) || (e.Button == System.Windows.Forms.MouseButtons.Right && Mouse.Button == System.Windows.Forms.MouseButtons.Left))
            {
                Middle_Simulated = true;
            }
            else
            {
                Middle_Simulated = false;
            }
            Mouse = e;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!Keyboard.Control)
                {
                    NodeControl Node = GetNodeAt(Rescale(e.Location));
                    if (Node != null)
                    {
                        Dragging = Node;
                    }
                }
                else
                {
                    NodeControl Node = GetNodeAt(Rescale(e.Location));
                    if (Node != null)
                    {
                        List<NodeControl> SubNodes = new List<NodeControl>();
                        foreach (TInstance.TNODE.TLINE Line in Node.BaseNode.LINES)
                        {
                            SubNodes.Add(new NodeControl(Line.PARA));
                        }
                        Nodes.AddRange(SubNodes.ToArray());
                        AutoOrganize(Node);
                        Invalidate();
                    }
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            PointF p = Rescale(e.Location);
            dW *= (1 + e.Delta / 1200f);
            X = (int)(-p.X * dW + e.X);
            Y = (int)(-p.Y * dW + e.Y);
            Invalidate();
            base.OnMouseWheel(e);
        }      
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Dragging = null;
            Middle_Simulated = false;
            NodeControl Node = GetNodeAt(Rescale(e.Location));
            if (Node != null)
            {
                switch (Keyboard.KeyCode)
                {
                    case Keys.F:
                        if (Node.Orientation == Orientation.Horizontal)
                        {
                            Node.Orientation = Orientation.Vertical;
                        }
                        else
                        {
                            Node.Orientation = Orientation.Horizontal;
                        }
                        break;
                }
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (Highlight != null)
                    {
                        Highlight.Highlight = false;
                    }
                    Node.Highlight = true;
                    Highlight = Node;
                }
                Invalidate();
            }
            else
            {
                if (Highlight != null)
                {
                    Highlight.Highlight = false;
                    Highlight = null;
                    Invalidate();
                }
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle || Middle_Simulated)
            {
                X += e.X - Mouse.X;
                Y += e.Y - Mouse.Y;
                Invalidate();
            }
            else if (Dragging != null)
            {
                if (Keyboard.Shift)
                {
                    FlagRecursivity = !FlagRecursivity;
                    RecursiveDrag(Dragging, (int)((e.X - Mouse.X) / dW), (int)((e.Y - Mouse.Y) / dW));
                    FlagRecursivity = !FlagRecursivity;
                    ClearRecursivity(Dragging);
                }
                else
                {
                    Dragging.Top += (int)((e.Y - Mouse.Y) / dW);
                    Dragging.Left += (int)((e.X - Mouse.X) / dW);
                }
                Invalidate();
            }
            else
            {
                NodeControl Node = GetNodeAt(Rescale(e.Location));
                if (Node != null)
                {
                    if (LastTip != Node)
                    {
                        LastTip = Node;
                        Tip.Show(Node.BaseNode.BAR.FullDescription, this);
                    }
                }
                else
                {
                    if (LastTip != null)
                    {
                        LastTip = null;
                        //Tip.Hide(this);
                    }
                    if(Highlight!=null){
                        Connection Conn = GetConnectionAt(Highlight, Rescale(e.Location));
                        if (Conn != null)
                        {
                            Tip.Show(Conn.Source.Line.CIR.FullDescription, this);
                        }
                    }
                }
            }
            Mouse = e;
            base.OnMouseMove(e);
        }
        private Connection GetConnectionAt(NodeControl Node, PointF P)
        {
            foreach (Connection Conn in Node.Output.Union(Node.Input))
            {
                for (int i = 0; i < Conn.Path.Length - 1; i++)
                {
                    Point A = Conn.Path[i], B = Conn.Path[i + 1];
                    float t = -((B.Y - A.Y) * (P.Y) + (B.X - A.X) * (P.X) - (B.Y) * (B.Y) + (A.Y) * (B.Y) - (B.X) * (B.X) + (A.X) * (B.X)) / ((B.Y) * (B.Y) - 2 * (A.Y) * (B.Y) + (B.X) * (B.X) - 2 * (A.X) * (B.X) + (A.Y) * (A.Y) + (A.X) * (A.X));
                    if (t > 0 && t < 1)
                    {
                        float X = A.X * t + B.X * (1 - t);
                        float Y = A.Y * t + B.Y * (1 - t);
                        double d = Math.Sqrt((X - P.X) * (X - P.X) + (Y - P.Y) * (Y - P.Y));
                        if (d < 5)
                        {
                            return Conn;
                        }
                    }
                }
            }
            return null;
        }
        NodeControl LastTip; 
        private void RecursiveDrag(NodeControl Node,int dX,int dY){
            if (Node.FlagRecursivity != FlagRecursivity)
            {
                Node.FlagRecursivity = FlagRecursivity;
                Node.Top += dY;
                Node.Left += dX;
                foreach (Connection Conn in Node.Output)
                {
                    RecursiveDrag(Conn.Destiny.Node, dX, dY);
                }
            }
        }
        private void RecursiveOrganize(NodeControl Node)
        {
            //for (int i = 0; i < SubNodes.Count; i++)
            //{
            //    SubNodes[i].Top = Node.Top + Node.Height * (3 + SubNodes.Count - Math.Abs(2 * i - SubNodes.Count + 1));
            //    SubNodes[i].Left = Node.Left + Node.Width / 2 + 2 * Node.Height * (2 * i - SubNodes.Count + 1) - SubNodes[i].Width / 2;
            //}
            if (Node.FlagRecursivity != FlagRecursivity)
            {
                Node.FlagRecursivity = FlagRecursivity;
                int SWidht = -30, VWidht;
                foreach (Connection Conn in Node.Output)
                {
                    SWidht += Conn.Destiny.Node.Width + 30;
                }
                VWidht = SWidht / 2;
                SWidht = -SWidht / 2;
                foreach (Connection Conn in Node.Output)
                {
                    if (Conn.Destiny.Node.FlagRecursivity != Node.FlagRecursivity)
                    {
                        if (Conn.Destiny.Node.Orientation == Orientation.Horizontal)
                        {
                            Conn.Destiny.Node.Top = Node.Top + 50;
                        }
                        else
                        {
                            Conn.Destiny.Node.Top = Node.Top + 130;
                        }
                        Conn.Destiny.Node.Left = Node.Left + Node.Width / 2 + SWidht;
                        SWidht += Conn.Destiny.Node.Width + 30;
                        Conn.Destiny.Node.Top += (VWidht - Math.Abs(SWidht - Conn.Destiny.Node.Width / 2));
                        RecursiveOrganize(Conn.Destiny.Node);
                    }
                }
            }
        }
        private void ClearRecursivity(NodeControl Node)
        {
            Node.FlagRecursivity = FlagRecursivity;
            foreach (Connection Conn in Node.Output)
            {
                ClearRecursivity(Conn.Destiny.Node);
            }
        }
        private PointF Rescale(Point p)
        {
            return new PointF((p.X - X) / dW, (p.Y - Y) / dW);
        }
        private NodeControl GetNodeAt(PointF P)
        {
            foreach (NodeControl Node in Nodes)
            {
                if ((Node.Top < P.Y && Node.Height + Node.Top > P.Y) && (Node.Left < P.X && Node.Width + Node.Left > P.X))
                {
                    return Node;
                }
            }
            return null;
        }
        public TInstance.TNODE GetNodeAtPoint(Point P)
        {
            NodeControl Node = GetNodeAt(Rescale(P));
            return Node == null ? null : Node.BaseNode;
        }
        public TInstance.TNODE.TLINE GetConnectionAtPoint(Point P)
        {
            if (Highlight != null)
            {
                Connection Conn=GetConnectionAt(Highlight, Rescale(P));
                if (Conn == null)
                {
                    return null;
                }
                else
                {
                    return Conn.Source.Node == Highlight ? Conn.Source.Line : Conn.Destiny.Line;
                }
            }
            else
            {
                return null;
            }
        }
        internal class Connection
        {
            internal class TPlug
            {
                internal bool Updated=false;
                internal NodeControl Node;
                private ArrowDirection _Direction = ArrowDirection.Up;
                internal ArrowDirection Direction
                {
                    set
                    {
                        Updated = false;
                        if (_Direction != value)
                        {
                            Node.Updated = false;
                        }
                        _Direction = value;
                    }
                    get
                    {
                        return _Direction;
                    }
                }
                internal TInstance.TNODE.TLINE Line;
                public Point GetConnectionPoint()
                {
                    int n, Count;
                    n= Node.Output.Count((Connection Conn) => Conn.Source.index < index && Conn.Source.Direction == Direction) + Node.Input.Count((Connection Conn) => Conn.Destiny.index < index && Conn.Destiny.Direction == Direction);
                    Count = Node.Output.Count((Connection Conn) => { return Conn.Source.Direction == Direction; }) + Node.Input.Count((Connection Conn) => { return Conn.Destiny.Direction == Direction; });
                    int Y, X;
                    if (Direction == ArrowDirection.Up || Direction == ArrowDirection.Down)
                    {
                        X = Node.Left + Node.Width * (1 + 2 * n) / (2 * Count);
                        if (Direction == ArrowDirection.Up)
                        {
                            Y = Node.Top;
                        }
                        else
                        {
                            Y = Node.Top + Node.Height;
                        }
                    }
                    else
                    {
                        Y = Node.Top + Node.Height * (1 + 2 * n) / (2 * Count);
                        if (Direction == ArrowDirection.Left)
                        {
                            X = Node.Left;
                        }
                        else
                        {
                            X = Node.Left + Node.Width;
                        }
                    }
                    return new Point(X, Y);
                }
                public int index;
                public bool ClearIntersection(Connection X)
                {
                    foreach (Connection Conn in Node.Output)
                    {
                        if (Conn != X)
                        {
                            if (Conn.CheckIntersection(X))
                            {
                                int index = Conn.Source.index;
                                Conn.Source.index = this.index;
                                Conn.Source.Updated = false;
                                this.index = index;
                                this.Updated = false;
                                //Conn.Update();
                                return false;
                            }
                        }
                    }
                    foreach (Connection Conn in Node.Input)
                    {
                        if (Conn != X)
                        {
                            if (Conn.CheckIntersection(X))
                            {
                                int index = Conn.Destiny.index;
                                Conn.Destiny.index = this.index;
                                Conn.Source.Updated = false;
                                this.index = index;
                                this.Updated = false;
                                //Conn.Update();
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            internal Point[] _Path;
            internal bool ClearIntersection(){
                return Source.ClearIntersection(this) && Destiny.ClearIntersection(this);
            }
            internal Point[] Path
            {
                get
                {
                    if (!Source.Updated || !Destiny.Updated)
                    {
                        Update();
                        if (!ClearIntersection())
                        {
                            Update();
                            ClearIntersection();
                            Update();
                        }
                        Source.Updated = true;
                        Destiny.Updated = true;
                    }
                    return _Path;
                }
            }
            internal TPlug Source = new TPlug(), Destiny = new TPlug();
            public Connection(NodeControl Source, NodeControl Destiny, TInstance.TNODE.TLINE Line)
            {
                this.Source.Node = Source;
                this.Destiny.Node= Destiny;
                this.Source.Line = Line;
                this.Destiny.Line = Line.Oposite;
                this.Destiny.Node.Updated = false;
                this.Source.Node.Updated = false;
            }
            internal static void assign(Connection Conn)
            {
                Conn.Source.Node.Output.Add(Conn);
                Conn.Source.index = Conn.Source.Node.Count;
                Conn.Destiny.Node.Input.Add(Conn);
                Conn.Destiny.index = Conn.Destiny.Node.Count;
            }          
            private void Update()
            {
                NodeControl NodeA = this.Source.Node;
                NodeControl NodeB = this.Destiny.Node;
                //TODO: Criar um algorítmo para refazer estas ligações visando a melhor disposição
                //int nA = this.Source.Node.Output.Count((Connection Conn) => Conn.Source.index < this.Source.index && Conn.Source.Direction == this.Source.Direction) + this.Source.Node.Input.Count((Connection Conn) => Conn.Destiny.index < this.Source.index && Conn.Destiny.Direction == this.Destiny.Direction);
                //int nB = this.Destiny.Node.Output.Count((Connection Conn) => Conn.Source.index < this.Destiny.index && Conn.Source.Direction == this.Destiny.Direction) + this.Destiny.Node.Input.Count((Connection Conn) => Conn.Destiny.index < this.Destiny.index && Conn.Destiny.Direction == this.Destiny.Direction);
                ////int nA = this.Source.Node.Output.FindAll((Connection Conn) => Conn.Source.Direction == this.Source.Direction).IndexOf(this) + this.Source.Node.Input.FindAll((Connection Conn) => Conn.Destiny.Direction == this.Source.Direction).Count;
                ////int nB = this.Destiny.Node.Input.FindAll((Connection Conn) => Conn.Destiny.Direction == this.Destiny.Direction).IndexOf(this);// +this.Destiny.Node.Output.FindAll((Connection Conn) => Conn.Destiny.Direction == this.Destiny.Direction).Count;
                //int nB = this.Destiny.Node.Output.Count((Connection Conn) => Conn.Source.index < this.Destiny.index && Conn.Source.Direction == this.Destiny.Direction) + this.Destiny.Node.Input.Count((Connection Conn) => Conn.Destiny.index < this.Destiny.index && Conn.Destiny.Direction == this.Destiny.Direction);
                //Point A = this.Source.GetConnectionPoint(nA, this.Source.Node.Output.Count((Connection Conn) => { return Conn.Source.Direction == this.Source.Direction; }) + this.Source.Node.Input.Count((Connection Conn) => { return Conn.Destiny.Direction == this.Source.Direction; }));
                //Point B = this.Destiny.GetConnectionPoint(nB, this.Destiny.Node.Input.Count((Connection Conn) => { return Conn.Destiny.Direction == this.Destiny.Direction; }) + this.Destiny.Node.Output.Count((Connection Conn) => { return Conn.Source.Direction == this.Destiny.Direction; }));
                Point A = this.Source.GetConnectionPoint();
                Point B = this.Destiny.GetConnectionPoint();
                //Partindo dos Pontos A e B, criar as direções
                if ((Destiny.Direction == ArrowDirection.Up && Source.Direction == ArrowDirection.Down) || (Destiny.Direction == ArrowDirection.Down && Source.Direction == ArrowDirection.Up))
                {
                    _Path = new Point[4];
                    _Path[0] = A;
                    _Path[1] = new Point(A.X, (B.Y + A.Y) / 2);
                    _Path[2] = new Point(B.X, (B.Y + A.Y) / 2);
                    _Path[3] = B;
                }
                else if ((Destiny.Direction == ArrowDirection.Left && Source.Direction == ArrowDirection.Right) || (Destiny.Direction == ArrowDirection.Right && Source.Direction == ArrowDirection.Left))
                {
                    _Path = new Point[4];
                    _Path[0] = A;
                    _Path[1] = new Point((B.X + A.X) / 2, A.Y);
                    _Path[2] = new Point((B.X + A.X) / 2, B.Y);
                    _Path[3] = B;
                }
                else
                {
                    _Path = new Point[3];
                    _Path[0] = A;
                    if (Destiny.Direction == ArrowDirection.Right || Destiny.Direction == ArrowDirection.Left)
                    {
                        _Path[1] = new Point(A.X, B.Y); 
                    }
                    else
                    {
                        _Path[1] = new Point(B.X, A.Y);
                    }
                    _Path[2] = B;
                }
            }
            internal bool CheckIntersection(Connection X)
            {
                if (X._Path != null && this._Path != null)
                {
                    for (int i = 1; i < this._Path.Length; i++)
                    {
                        for (int j = 1; j < X._Path.Length; j++)
                        {
                            PointF Ya = this._Path[i - 1];
                            PointF Yb = this._Path[i];
                            PointF Xa = X._Path[j - 1];
                            PointF Xb = X._Path[j];
                            System.Drawing.Drawing2D.Matrix M;
                            M = new System.Drawing.Drawing2D.Matrix(Ya.X - Yb.X, Ya.Y - Yb.Y, Xb.X - Xa.X, Xb.Y - Xa.Y, 0, 0);
                            if (M.IsInvertible)
                            {
                                PointF C = new PointF(Xb.X - Yb.X, Xb.Y - Yb.Y);
                                M.Invert();
                                PointF[] A = new PointF[] { C };
                                M.TransformPoints(A);
                                C = A[0];
                                if (C.X < 1 && C.X > 0 && C.Y < 1 && C.Y > 0)
                                {
                                    return true;
                                }
                            }
                            else if (Xb.X - Yb.X == 0 || Xb.Y - Yb.Y == 0)
                            {
                                //return fa;
                            }
                        }
                    }
                }
                return false;
            }
        }
        public class NodeControl
        {
            internal class TEdgeConnections
            {
                internal int Top = 0, Left = 0, Botton = 0, Right = 0;
            }
            internal TEdgeConnections Edge = new TEdgeConnections();
            internal bool Highlight = false, Updated = false;
            internal List<Connection> Input = new List<Connection>();
            internal List<Connection> Output = new List<Connection>();
            internal int Count
            {
                get
                {
                    return Input.Count + Output.Count;
                }
            }
            public TInstance.TNODE BaseNode;
            internal NodeControl(TInstance.TNODE Node): base()
            {
                this.BaseNode = Node;
                Orientation = System.Windows.Forms.Orientation.Horizontal;
            }
            private Orientation _Orientation;
            public Orientation Orientation
            {
                set
                {
                    _Orientation = value;
                    Updated = false;
                }
                get
                {
                    return _Orientation;
                }
            }
            public int Tickness = 20;
            public void UpdateSize()
            {
                if (BaseNode.BAR.mp == NetWork.ANA.BAR.MP.MidPoint)
                {
                    _Width = _Height = 2 * Tickness;
                }
                else
                {
                    if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        _Height = Tickness;
                        _Width = 2 * Tickness * (Count + 1);
                    }
                    else
                    {
                        _Height = 2 * Tickness * (Count + 1);
                        _Width = Tickness;
                    }
                }
            }
            private int _Top = 0, _Left = 0, _Height, _Width;
            private ArrowDirection GetDirections(NodeControl Node)
            {
                if (BaseNode.BAR.mp == NetWork.ANA.BAR.MP.MidPoint)
                {
                    if (Top - Node.Top > Left - Node.Left)
                    {
                        if (Top - Node.Top > -(Left - Node.Left))
                        {
                            return ArrowDirection.Up;
                        }
                        else
                        {
                            return  ArrowDirection.Right;
                        }
                    }
                    else
                    {
                        if (Top - Node.Top > -(Left - Node.Left))
                        {
                            return  ArrowDirection.Left;
                        }
                        else
                        {
                            return  ArrowDirection.Down;
                        }
                    }
                }
                else
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (Top + Height < Node.Top + Node.Height / 2)
                        {
                            return ArrowDirection.Down;
                        }
                        else
                        {
                            return ArrowDirection.Up;
                        }
                    }
                    else
                    {
                        if (Left + Width < Node.Left + Node.Width / 2)
                        {
                            return ArrowDirection.Right;
                        }
                        else
                        {
                            return ArrowDirection.Left;
                        }
                    }
                }
                return ArrowDirection.Down;
            }
            private void UpdateConnections()
            {
                foreach (Connection Conn in Output){
                    Conn.Source.Direction = GetDirections(Conn.Destiny.Node);
                    Conn.Destiny.Direction = Conn.Destiny.Node.GetDirections(this);
                }
                foreach (Connection Conn in Input){
                    Conn.Destiny.Direction = GetDirections(Conn.Source.Node);
                    Conn.Source.Direction = Conn.Source.Node.GetDirections(this);
                }
            }
            public int Top
            {
                set
                {
                    _Top = value;
                    Updated = false;
                }
                get
                {
                    return _Top;
                }
            }
            public int Left
            {
                set
                {
                    _Left = value;
                    Updated = false;
                }
                get
                {
                    return _Left;
                }
            }
            public int Height
            {
                set
                {
                    _Height = value;
                    Updated = false;
                }
                get
                {
                    return _Height;
                }
            }
            public int Width
            {
                set
                {
                    _Width = value;
                    Updated = false;
                }
                get
                {
                    return _Width;
                }
            }
            public bool FlagRecursivity = false;
            internal void Paint(PaintEventArgs e, NetView Parent, bool CheckPaint)
            {
                if (!Updated)
                {
                    UpdateSize();
                    UpdateConnections();
                    Updated = true;
                }
                if (CheckPaint != this.FlagRecursivity)
                {
                    this.FlagRecursivity = CheckPaint;
                    if (BaseNode.BAR.mp == NetWork.ANA.BAR.MP.MidPoint)
                    {
                        e.Graphics.DrawImage(Methods.NetWorkView.Resource.TT, _Left + 5, _Top + 5, _Width - 10, _Height - 10);
                    }
                    else
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            float M = e.Graphics.MeasureString(BaseNode.Name, DefaultFont).Width;
                            int dW = Math.Max(_Width, (int)M) - _Width;
                            if (dW != 0)
                            {
                                //_Left -= dW / 2;
                                _Width += dW;
                            }
                            e.Graphics.DrawString(BaseNode.Name, DefaultFont, Brushes.Black, _Left + (_Width - M) / 2, _Top + (_Height - DefaultFont.Height) / 2);
                        }
                        else
                        {
                            e.Graphics.DrawString(BaseNode.Name, DefaultFont, Brushes.Black, _Left + _Width, _Top + _Height - DefaultFont.Height);
                        }
                    }
                    if (Highlight)
                    {
                        Pen MyPen = new Pen(Brushes.Black);
                        MyPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        e.Graphics.DrawRectangle(MyPen, _Left, _Top, _Width, _Height);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(Pens.Black, _Left, _Top, _Width, _Height);
                    }
                    foreach (Connection Conn in Input)
                    {
                        Conn.Source.Node.Paint(e, Parent, CheckPaint);
                    }
                    foreach (Connection Conn in Output)
                    {
                        Conn.Destiny.Node.Paint(e, Parent, CheckPaint);
                    }
                    foreach (Connection Conn in Output)
                    {
                        if (Conn.Destiny.Node.Highlight || Conn.Source.Node.Highlight)
                        {
                            Pen MyPen = new Pen(Brushes.Black);
                            MyPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            MyPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                            e.Graphics.DrawLines(MyPen, Conn.Path);
                        }
                        else
                        {
                            e.Graphics.DrawLines(Pens.Black, Conn.Path);
                        }
                    }
                }
            }
        }
        public void AutoOrganize(NodeControl Node)
        {
            FlagRecursivity = !FlagRecursivity;
            RecursiveOrganize(Node);
            FlagRecursivity = !FlagRecursivity;
            ClearRecursivity(Node);
        }
        public void Auto()
        {
            FlagRecursivity = !FlagRecursivity;
            foreach (var v in Nodes.Systens)
            {
                RecursiveOrganize(v);
            }
            FlagRecursivity = !FlagRecursivity;
            foreach (var v in Nodes.Systens)
            {
                ClearRecursivity(v);
            }
            Invalidate();
        }
    }
}
