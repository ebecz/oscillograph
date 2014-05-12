using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Drawing;

using PowerSystem.IEEEComtrade;

using PowerSystem.NetWork;

namespace PowerSystem
{
    public class T3F : List<TComtrade.AChannel>
    {
        private CMath.TPhasor[] _Sequence;
        public CMath.TPhasor[] Sequence
        {
            get
            {
                if (_Sequence == null)
                {
                    _Sequence = CMath.TPhasor.Sequence(A, B, C, N);
                }
                return _Sequence;
            }
        }
        public TComtrade.AChannel A
        {
            get
            {
                foreach (TComtrade.AChannel Channel in this)
                {
                    if (((string)Channel.ph).ToUpper() == "A")
                    {
                        return Channel;
                    }
                }
                return null;
            }
        }
        public TComtrade.AChannel B
        {
            get
            {
                foreach (TComtrade.AChannel Channel in this)
                {
                    if (((string)Channel.ph).ToUpper() == "B")
                    {
                        return Channel;
                    }
                }
                return null;
            }
        }
        public TComtrade.AChannel C
        {
            get
            {
                foreach (TComtrade.AChannel Channel in this)
                {
                    if (((string)Channel.ph).ToUpper() == "C")
                    {
                        return Channel;
                    }
                }
                return null;
            }
        }
        public TComtrade.AChannel N
        {
            get
            {
                foreach (TComtrade.AChannel Channel in this)
                {
                    if (((string)Channel.ph).ToUpper() == "N")
                    {
                        return Channel;
                    }
                }
                return null;
            }
        }
        public string ccbm
        {
            get
            {
                return this[0].ccbm;
            }
        }
        public TComtrade Comtrade
        {
            get
            {
                return this[0].BaseComtrade;
            }
        }
    }
    public class TInstance
    {
        public ANA Ana;
        public class TNODE
        {
            internal ANA.BAR BAR;
            public class TLINE
            {
                internal ANA.CIR CIR;
                public TLINE Oposite;
                public Complex K0
                {
                    get
                    {
                        return (Z0 - Z1) / (3 * Z1);
                    }
                }
                internal TLINE(ANA.CIR CIR, TNODE DE, TNODE PARA)
                {
                    this.CIR = CIR;
                    this.DE = DE;
                    this.PARA = PARA;
                }
                public Complex Z0
                {
                    get
                    {
                        return new Complex(CIR.R0, CIR.X0);
                    }
                }
                public Complex Z1
                {
                    get
                    {
                        return new Complex(CIR.R1, CIR.X1);
                    }
                }
                public TNODE DE, PARA;
                public string Name
                {
                    get
                    {
                        if (I3F.Count == 0)
                        {
                            return CIR.Circuit_Name;
                        }
                        else
                        {
                            return I3F[0].ccbm;
                        }
                    }
                }
                private T3F _I3F = new T3F();
                public T3F I3F
                {
                    get
                    {
                        return _I3F;
                    }
                    set
                    {
                        _I3F = value;
                        foreach (IEEEComtrade.TComtrade.AChannel Channel in _I3F)
                        {
                            //Lembrar de desnvincular???
                            //if (Channel.Barra != null)
                            //{

                            //}
                            Channel.Barra = DE;
                        }
                    }
                }
                public List<TComtrade.DChannel> D = new List<TComtrade.DChannel>();
                public float L;
                public override string ToString()
                {
                    return Name;
                }
            }
            public List<TLINE> LINES = new List<TLINE>();
            public T3F _V3F = new T3F();
            public T3F V3F
            {
                get
                {
                    return _V3F;
                }
                set
                {
                    _V3F = value;
                    foreach (IEEEComtrade.TComtrade.AChannel Channel in _V3F)
                    {
                        //Lembrar de desnvincular???
                        //if (Channel.Barra != null)
                        //{

                        //}
                        Channel.Barra = this;
                    }
                }
            }
            internal TNODE(ANA.BAR BAR, TInstance Instance)
            {
                this.BAR = BAR;
                this.Instance = Instance;
            }
            public float VBASE
            {
                get
                {
                    return BAR.VBASE;
                }
            }
            public string Name
            {
                get
                {
                    return BAR.BarName;
                }
            }
            public TInstance Instance;
            public override string ToString()
            {
                return Name;
            }
            public string SE
            {
                get
                {
                    return V3F[0].BaseComtrade.rec_dev_id;
                }
            }
        }
        public List<TNODE> LNode = new List<TNODE>();
        public class ListWithEvents<T>:List<T>{
            public delegate void OnAdd(T Item);
            public event OnAdd Added;
            public new void Add(T item)
            {
                if (Added != null)
                    Added(item);
                base.Add(item);
            }
            public new void AddRange(IEnumerable<T> Collection)
            {
                if (Added != null)
                    foreach (T Item in Collection)
                        Added(Item);
                base.AddRange(Collection);
            }
        }
        public ListWithEvents<TNODE> LNode_Using = new ListWithEvents<TNODE>();
        public ListWithEvents<TNODE.TLINE> LLine_Using = new ListWithEvents<TNODE.TLINE>();

        public TInstance(ANA AnaFile, DateTime TimeReference)
        {
            Cursor = new TTimeCursor(Pens.Black);
            Anchor = new TTimeCursor(new Pen(Color.Black, 2));
            Anchor.pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            Load(AnaFile, TimeReference);
        }
        public TInstance()
        {
            Ana = new ANA();
        }
        public bool Load(ANA Ana, DateTime TimeReference)
        {
            this.Ana = Ana;
            Dictionary<uint, TNODE> DBARRA = new Dictionary<uint, TNODE>();
            uint AREA = 0;
            foreach (ANA.DARE DARE in Ana.DARES)
            {
                if (DARE.Nome.ToLower() == Config.DefaultZone.ToLower())
                {
                    AREA = DARE.NUN;
                }
            }
            foreach (ANA.BAR BAR in Ana.BARS)
            {
                if (BAR.SubSistema == AREA || AREA == 999)
                {
                    if (BAR.DATA_I < TimeReference && BAR.DATA_F > TimeReference)
                    {
                        TNODE BARRA = new TNODE(BAR, this);
                        LNode.Add(BARRA);
                        DBARRA.Add(BAR.NB, BARRA);
                    }
                }
            }
            foreach (ANA.CIR CIR in Ana.CIRS)
            {
                if (CIR.DATA_I < TimeReference && CIR.DATA_F > TimeReference)
                {
                    TNODE A, B;
                    if (DBARRA.TryGetValue(CIR.BF, out A) & DBARRA.TryGetValue(CIR.BT, out B))
                    {
                        TNODE.TLINE L1 = new TNODE.TLINE(CIR, A, B);
                        TNODE.TLINE L2 = new TNODE.TLINE(CIR, B, A);
                        L1.Oposite = L2;
                        L2.Oposite = L1;
                        A.LINES.Add(L1);
                        B.LINES.Add(L2);
                    }
                }
            }
            return true;
        }
        public TTimeCursor Cursor, Anchor;
        public TTime Time = new TTime();
    }
}
