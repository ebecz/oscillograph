using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerSystem.IEEEComtrade;
using System.Numerics;
//using PowerSystem.NetWork;

namespace PowerSystem.CMath
{
    public class TPhasor
    {
        public Complex M=1;
        public Complex[] Values;
        public Complex this[DateTime D]
        {
            get
            {
                long EndSamp = (long)(Channel.BaseComtrade.endsamp[Channel.BaseComtrade.samp.Count() - 1]);
                long n = EndSamp * (D - Channel.BaseComtrade.start_time).Ticks / (Channel.BaseComtrade.end_time - Channel.BaseComtrade.start_time).Ticks;
                if (n >= 0 && n <= EndSamp)
                {
                    return M * Values[n] / Channel.BaseComtrade.Reference.Values[n];
                }
                else
                {
                    return Complex.Zero;
                }
            }
        }
        public Complex this[ulong n]
        {
            get
            {
                return M * Values[n] / Channel.BaseComtrade.Reference.Values[n];
            }
        }
        public TPhasor PhaseReference{
            get{
                TPhasor P2 = new TPhasor(this);
                for (int i = 0; i < P2.Values.Length; i++)
                {
                    P2.Values[i] = Values[i] / Values[i].Magnitude;
                }
                return P2;
            }
        }
        static public TPhasor[] Sequence(TComtrade.AChannel A, TComtrade.AChannel B, TComtrade.AChannel C,TComtrade.AChannel N=null)
        {
            if (A == null || B == null || C == null) return null;
            Complex a = new Complex(Math.Cos(2 * Math.PI / 3), Math.Sin(2 * Math.PI / 3));
            TPhasor[] S = new TPhasor[3];
            if (N == null)
            {
                S[0] = (A.Phasor + B.Phasor + C.Phasor) * (1 / 3f);
            }
            else
            {
                S[0] = N.Phasor;
            }
            S[1] = (A.Phasor + B.Phasor * a + C.Phasor * (a * a)) * (1 / 3f);
            S[2] = (A.Phasor + B.Phasor * (a * a) + C.Phasor * a) * (1 / 3f);
            S[0].Channel.ph = "S0";
            S[1].Channel.ph = "S1";
            S[2].Channel.ph = "S2";
            return S;
        }
        public static TPhasor operator *(TPhasor X, Complex N)
        {
            TPhasor Y = new TPhasor(X, false);
            Y.M *= N;
            return Y;
        }
        public static TPhasor operator +(TPhasor X, TPhasor Y)
        {
            if (X.Channel.BaseComtrade == Y.Channel.BaseComtrade)
            {
                TPhasor Z = new TPhasor(X, true);
                Z.Channel.ph.Color = System.Drawing.Color.FromArgb((X.Channel.ph.Color.ToArgb() + Y.Channel.ph.Color.ToArgb()) / 2);
                for (ulong i = 0; i < X.Channel.BaseComtrade.endsamp[0]; i++)
                {
                    Z.Values[i] = X.M * X.Values[i] + Y.M * Y.Values[i];
                }
                return Z;
            }
            else
            {
                return null;
            }
        }        
        private TPhasor(TPhasor Copy, bool Hard=true)
        {
            if (Hard)
            {
                Values = new Complex[Copy.Channel.BaseComtrade.endsamp[0]];
                Channel = new TComtrade.AChannel(Copy.Channel.BaseComtrade);
                Channel.a = Copy.Channel.a;
                Channel.n = 0;
                Channel.b = Copy.Channel.b;
                Channel.ccbm = Copy.Channel.ccbm;
                Channel.ch_id = Copy.Channel.ccbm;
                Channel.max = Copy.Channel.max;
                Channel.min = Copy.Channel.min;
                Channel.ph = Copy.Channel.ph;
                Channel.primary = Copy.Channel.primary;
                Channel.PS = Copy.Channel.PS;
                Channel.secondary = Copy.Channel.secondary;
                Channel.skew = 0;
                Channel.uu = Copy.Channel.uu;
                Channel.Values = null;
                Channel.Barra = Copy.Channel.Barra;
            }
            else
            {
                Channel = Copy.Channel;
                Values = Copy.Values;
                this.M = Copy.M;
            }
        }
        public TComtrade.AChannel Channel;
        public TPhasor(TComtrade.AChannel Channel)
        {
            this.Channel = Channel;

            uint T = (uint)(Channel.BaseComtrade.samp[0] / Channel.BaseComtrade.lf);

            double sqrt2 = Math.Sqrt(2);
            double[] vA = new double[T];
            double[] vB = new double[T];
            double[] MSin = new double[T];
            double[] MCos = new double[T];
            double B = 0, A = 0;
            double w = 2 * Math.PI / T;
            Values = new Complex[Channel.BaseComtrade.endsamp[0]];
            for (ulong n = 0; n < T; n++)
            {
                MSin[n] = Math.Sin(w * n);
                MCos[n] = Math.Cos(w * n);
                vA[n] = Channel.Values[n] * MSin[n];
                A += vA[n];
                vB[n] = Channel.Values[n] * MCos[n];
                B += vB[n];
            }
            for (ulong n = 0; n < T / 2; n++)
            {
                Values[n] = (new Complex(B * sqrt2 / T, -A * sqrt2 / T)) * Channel.a;
            }
            for (ulong n = T; n < Channel.BaseComtrade.endsamp[0]; n++)
            {
                A -= vA[n % T];
                vA[n % T] = Channel.Values[n] * Math.Sin(w * (n % T));
                A += vA[n % T];
                B -= vB[n % T];
                vB[n % T] = Channel.Values[n] * Math.Cos(w * (n % T));
                B += vB[n % T];
                Values[n - T / 2] = (new Complex(B * sqrt2 / T, -A * sqrt2 / T)) * Channel.a;
            }
            for (ulong n = Channel.BaseComtrade.endsamp[0] - T / 2; n < Channel.BaseComtrade.endsamp[0]; n++)
            {
                Values[n] = Values[Channel.BaseComtrade.endsamp[0] - T / 2 - 1];
            }
        }
        public enum TReaderMode { PU, Primary, Secondary };
        public TPhasor Reader(TReaderMode Mode)
        {
            TPhasor R = new TPhasor(this, false);
            switch (Mode)
            {
                case TReaderMode.PU:
                    if (Channel.Barra == null)
                    {
                        throw (new Exception("Cannot Create a PU Reader without Node information"));
                    }
                    else
                    {
                        switch (this.Channel.uu.uu)
                        {
                            case TComtrade.AChannel.Tuu.Tu.V:
                                R.M *= (this.Channel.uu.M / 1000) / Channel.Barra.BAR.VBASE;
                                break;
                            case TComtrade.AChannel.Tuu.Tu.A:
                                R.M *= Channel.Barra.BAR.VBASE * (this.Channel.uu.M / 1000) / Channel.Barra.Instance.Ana.SBASE;
                                break;
                        }
                    }
                    break;
                case TReaderMode.Primary:
                    if (this.Channel.PS == TComtrade.AChannel.TPS.P)
                    {
                        R.M*= 1;
                    }
                    else
                    {
                        R.M *= this.Channel.primary / this.Channel.secondary;
                    }
                    break;
                case TReaderMode.Secondary:
                    if (this.Channel.PS == TComtrade.AChannel.TPS.S)
                    {
                        R.M *= 1;
                    }
                    else
                    {
                        R.M *= this.Channel.secondary / this.Channel.primary;
                    }
                    break;
            }
            return R;
        }
    }
}