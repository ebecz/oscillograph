using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Numerics;

using PowerSystem.CMath;

namespace PowerSystem.Tools.Comtrade_Snooper
{
    static class Takagi
    {
        public static double Calcular(TInstance.TNODE.TLINE Line, ulong D,out string Tp)
        {
            double R = 0;
            FaultDescriptor Descriptor = new FaultDescriptor(Line, D);
            Tp = (Descriptor.A ? "A" : "") +
                (Descriptor.B ? "B" : "") +
                (Descriptor.C ? "C" : "") +
                (Descriptor.G ? "G" : "");
            if (Descriptor.A && Descriptor.B && Descriptor.C)
            {
                Complex V1 = Line.DE.V3F.Sequence[1].Reader(TPhasor.TReaderMode.PU)[D];
                Complex I1 = Line.I3F.Sequence[1].Reader(TPhasor.TReaderMode.PU)[D];
                R = (V1 / I1).Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            if (Descriptor.A && Descriptor.B && !Descriptor.C)
            {
                Complex Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Z1 = (Va - Vb) / (Ia - Ib);
                R = Z1.Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            if (!Descriptor.A && Descriptor.B && Descriptor.C)
            {
                Complex Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D];

                Complex Z1 = (Vb - Vc) / (Ib - Ic);
                R = Z1.Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            if (Descriptor.A && !Descriptor.B && Descriptor.C)
            {
                Complex Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Z1 = (Vc - Va) / (Ic - Ia);
                R = Z1.Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            Complex Lz1 = Line.Z1;
            Complex Lz0 = Line.Z0;
            Complex K = Line.K0;
            Complex Io = Line.I3F.Sequence[0].Reader(TPhasor.TReaderMode.PU)[D];
            if (Descriptor.A && !Descriptor.B && !Descriptor.C)
            {
                Complex Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                R = (Va * Complex.Conjugate(Io)).Imaginary / (Line.Z1 * (Ia + K * Io) * Complex.Conjugate(Io)).Imaginary;
                return R;
            }
            if (!Descriptor.A && Descriptor.B && !Descriptor.C)
            {
                Complex Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                R = (Vb * Complex.Conjugate(Io)).Imaginary / (Line.Z1 * (Ib + K * Io) * Complex.Conjugate(Io)).Imaginary;
                return R;
            }
            if (!Descriptor.A && !Descriptor.B && Descriptor.C)
            {
                Complex Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                Complex Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D];
                R = (Vc * Complex.Conjugate(Io)).Imaginary / (Line.Z1 * (Ic + K * Io) * Complex.Conjugate(Io)).Imaginary;
                return R;
            }
            return R;
        }
    }
    class FaultDescriptor
    {
        public bool A, B, C, G;
        public bool R;
        public FaultDescriptor(TInstance.TNODE.TLINE Linha, ulong D)
        {
            A = false;
            B = false;
            C = false;
            this.G = false;
            R = false;
            TPhasor
                Va = Linha.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                Vb = Linha.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                Vc = Linha.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ia = Linha.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ib = Linha.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ic = Linha.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                k0I0 = Linha.I3F.Sequence[0].Reader(TPhasor.TReaderMode.PU) * Linha.K0;

            double N = (Linha.I3F.Sequence[0][D] / Linha.I3F.Sequence[1][0]).Magnitude;

            Complex[] Z = new Complex[6];

            Z[0] = (Va[D] - Vb[D]) / (Ia[D] - Ib[D]);
            Z[1] = (Vb[D] - Vc[D]) / (Ib[D] - Ic[D]);
            Z[2] = (Vc[D] - Va[D]) / (Ic[D] - Ia[D]);
            Z[3] = Va[D] / (Ia[D] + k0I0[D]);
            Z[4] = Vb[D] / (Ib[D] + k0I0[D]);
            Z[5] = Vc[D] / (Ic[D] + k0I0[D]);

            double M = double.PositiveInfinity;
            double Mm = 0;
            int n = 0;
            for (int k = 0; k < 6; k++)
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
                A = true;
                B = true;
                C = true;
                this.G = false;
            }
            else
            {
                if ((Z[n] / Linha.Z1).Phase > Math.PI)
                {
                    R = true;
                }
                else if (n < 3)
                {
                    if (Math.Max(Z[0].Magnitude - M, Math.Max(Z[1].Magnitude - M, Z[2].Magnitude - M)) / M < 0.01)
                    {
                        A = true;
                        B = true;
                        C = true;
                    }
                    else
                    {
                        switch (n)
                        {
                            case 0:
                                A = true;
                                B = true;
                                break;
                            case 1:
                                B = true;
                                C = true;
                                break;
                            case 2:
                                C = true;
                                A = true;
                                break;
                        }
                    }
                }
                else
                {
                    switch (n)
                    {
                        case 3:
                            A = true; break;
                        case 4:
                            B = true; break;
                        case 5:
                            C = true; break;
                    }
                    this.G = true;
                }
            }
        }
    }
}
