using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PowerSystem.IEEEComtrade;

namespace PowerSystem.Methods
{
    class DjTime : TMethod<TInstance.TNODE.TLINE>
    {
        public System.Windows.Forms.ToolStrip MenuTool
        {
            get
            {
                return null;
            }
        }
        public string Text
        {
            get
            {
                return "Circuit breaker opening time";
            }
        }
        public string Description
        {
            get
            {
                return "Circuit breaker opening time";
            }
        }
        private class TMyResult : TResult
        {
            public double R;
            public bool b;
            public object Data
            {
                get
                {
                    return R;
                }
            }
            public void Show(System.Windows.Forms.Form ParentForm)
            {
                if (b)
                {
                    System.Windows.Forms.MessageBox.Show(ParentForm, R.ToString("F2"), "Estimação de tempo de abertura de Disjuntor");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(ParentForm, "Não foi possível identificar", "Estimação de tempo de abertura de Disjuntor");
                }
            }
        }
        public TResult Execute(TInstance.TNODE.TLINE Linha)
        {
            TMyResult Result = new TMyResult();
            DateTime A = DateTime.MaxValue, B = DateTime.MinValue;
            foreach (TComtrade.DChannel Channel in Linha.D)
            {
                if (Channel.ch_id.ToLower().IndexOf("trip") != -1)
                {
                    if (A == DateTime.MaxValue)
                    {
                        A = Linha.DE.Instance.Anchor.Cursor = Channel.ChangeTime;
                        Linha.DE.Instance.Anchor.Enabled = true;
                    }
                }
            }
            foreach (TComtrade.AChannel Channel in Linha.I3F)
            {
                short ALimit = (short)(20 / Channel.a);
                ulong C = 0, TLimit = 3 * (ulong)(Channel.BaseComtrade.samp[0] / 60);
                for (ulong n = 0, N = 0; n < Channel.BaseComtrade.endsamp[0]; n++)
                {
                    C++;
                    if (Linha.I3F.A.Values[n] > ALimit || Linha.I3F.A.Values[n] < -ALimit)
                    {
                        C = 0;
                        N = n;
                    }
                    if (C > TLimit)
                    {
                        DateTime D = Channel.BaseComtrade.start_time.AddSeconds(N / Channel.BaseComtrade.samp[0]);
                        B = B > D ? B : D;
                        break;
                    }
                }
            }
            if (A != DateTime.MaxValue && B != DateTime.MinValue)
            {
                Linha.DE.Instance.Cursor.Cursor = B;
                Linha.DE.Instance.Cursor.Enabled = true;
                float t = (B - A).Milliseconds;
                Result.R = t;
                Result.b = true;
            }
            else
            {
                Result.R = 0;
                Result.b = false;
            }
            return Result;
        }
    }
}
