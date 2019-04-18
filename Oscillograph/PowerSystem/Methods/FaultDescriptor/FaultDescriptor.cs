using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerSystem.CMath;
using System.Numerics;

namespace PowerSystem.Methods
{
    class FaultDescriptor : TMethod<TInstance.TNODE.TLINE>
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
                return "Identificador do Tipo de falta";
            }
        }
        public string Description
        {
            get
            {
                return "Identificador do Tipo de falta";
            }
        }
        public class TMyResult : TResult
        {
            public bool A, B, C, N;
            public bool Reverse;
            public void Show(System.Windows.Forms.Form ParentForm)
            {
                
            }
            public object Data{
                get
                {
                    return this;
                }
            }
        }
        public TResult Execute(TInstance.TNODE.TLINE Linha)
        {
            TMyResult R = new TMyResult();
            DateTime D = Linha.DE.Instance.Cursor.Cursor;
            TPhasor
                Va = Linha.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                Vb = Linha.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                Vc = Linha.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ia = Linha.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ib = Linha.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ic = Linha.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                k0I0 = Linha.I3F.Sequence[0].Reader(TPhasor.TReaderMode.PU) * Linha.K0;

            double N = (Linha.I3F.Sequence[0][D] / Linha.I3F.Sequence[1][0]).Magnitude;

            Complex[]Z=new Complex[6];

            Z[0] = (Va[D] - Vb[D]) / (Ia[D] - Ib[D]);
            Z[1] = (Vb[D] - Vc[D]) / (Ib[D] - Ic[D]);
            Z[2] = (Vc[D] - Va[D]) / (Ic[D] - Ia[D]);
            Z[3] = Va[D] / (Ia[D] + k0I0[D]);
            Z[4] = Vb[D] / (Ib[D] + k0I0[D]);
            Z[5] = Vc[D] / (Ic[D] + k0I0[D]);

            double M = double.PositiveInfinity;
            double Mm=0;
            int n = 0;
            for (int k = 0; k < 6;k++)
            {
                M = Math.Min(Z[k].Magnitude, M);
                if (M == Z[k].Magnitude)
                {
                    n = k;
                }
                Mm += Z[k].Magnitude;
            }
            if (Mm / (6 * M) < 1.2f)
            {
                R.A = true;
                R.B = true;
                R.C = true;
                R.N = false;
            }
            else
            {
                if ((Z[n] / Linha.Z1).Phase > Math.PI)
                {
                    R.Reverse = true;
                }
                else if (n < 3)
                {
                    if (Math.Max(Z[0].Magnitude - M, Math.Max(Z[1].Magnitude - M, Z[2].Magnitude - M)) / M < 0.01)
                    {
                        R.A = true;
                        R.B = true;
                        R.C = true;
                    }
                    else
                    {
                        switch (n)
                        {
                            case 0:
                                R.A = true;
                                R.B = true;
                                break;
                            case 1:
                                R.B = true;
                                R.C = true;
                                break;
                            case 2:
                                R.C = true;
                                R.A = true;
                                break;
                        }
                    }
                }
                else
                {
                    switch (n)
                    {
                        case 3:
                            R.A = true; break;
                        case 4:
                            R.B = true; break;
                        case 5:
                            R.C = true; break;
                    }
                    R.N = true;
                }
            }
            return R;
        }
    }
}
