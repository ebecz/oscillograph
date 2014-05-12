using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PowerSystem;

namespace PowerSystem
{
    public class TTime
    {
        public delegate void OnChange(TTime Sender);
        public event OnChange Change;
        private DateTime Ai = DateTime.MinValue, Bi = DateTime.MaxValue;
        public DateTime A
        {
            get
            {
                return Ai;
            }
            set
            {
                if (value < Start)
                {
                    value = Start;
                }
                if (Ai != value && Change != null)
                {
                    Change(this);
                }
                Ai = value;
            }
        }
        public DateTime B
        {
            get
            {
                return Bi;
            }
            set
            {
                if (value > End)
                {
                    value = End;
                }
                if (Bi != value && Change != null)
                {
                    Change(this);
                }
                Bi = value;
            }
        }
        public class TReference{
            public event OnChange Change;
            private bool iFixed = false;
            private DateTime iValue;
            private TTime Parent;
            public TReference(TTime Parent)
            {
                this.Parent = Parent;
            }
            public bool Fixed
            {
                get
                {
                    return iFixed;
                }
                set
                {
                    if (iFixed != value)
                    {
                        iFixed = value;
                        if (Change != null)
                        {
                            Change(Parent);
                        }
                    }
                }
            }
            public DateTime Value
            {
                get
                {
                    if (Fixed)
                    {
                        return iValue;
                    }
                    else
                    {
                        return Parent.Ai;
                    }
                }
                set
                {
                    if (value > Parent.End)
                    {
                        iValue = Parent.End;
                    }
                    else if (value < Parent.Start)
                    {
                        iValue = Parent.Start;
                    }
                    else
                    {
                        iValue = value;
                    }
                    Fixed = true;
                    if (Change != null)
                    {
                        Change(Parent);
                    }
                }
            }
        }
        public TReference Reference;
        public DateTime Start = DateTime.MaxValue, End = DateTime.MinValue;
        public void SetAandB(DateTime A, DateTime B)
        {
            if (B > A)
            {
                if (A >= Start && B <= End)
                {
                    if (Ai != A || Bi != B)
                    {
                        Ai = A; Bi = B;
                        if (Change != null)
                        {
                            Change(this);
                        }
                    }
                }
                else if (A < Start)
                {
                    Ai = Start;
                    if (Change != null)
                    {
                        Change(this);
                    }
                }
                else if (B > End)
                {
                    Bi = End;
                    Change(this);
                }
            }
        }
        public void SetAorB(DateTime A, DateTime B)
        {
            if (B > A)
            {
                if (A >= Start)
                {
                    Ai = A;
                }
                else
                {
                    Ai = Start;
                }
                if (B <= End)
                {
                    Bi = B;
                }
                else
                {
                    Bi = End;
                }
                Change(this);
            }
        }
        public TTime()
        {
            Reference = new TReference(this);
        }
    }
    public class TTimeCursor
    {
        public delegate void OnChange(TTimeCursor Sender, DateTime OldValue);
        public event OnChange Change;
        private DateTime iCursor;
        public TInstance Parent;
        public Pen pen;
        public DateTime Cursor
        {
            get
            {
                return iCursor;
            }
            set
            {
                DateTime Old = iCursor;
                iCursor = value;
                if (Change != null) { Change(this, Old); }
            }
        }
        private bool iEnabled = false;
        public bool Enabled
        {
            get { return iEnabled; }
            set
            {
                if (iEnabled != value)
                {
                    DateTime Old = iCursor;
                    iEnabled = value;
                    if (Change != null) { Change(this, Old); }
                }
            }
        }      
        public TTimeCursor(Pen pen)
        {
            this.pen = pen;
        }
    };
}
