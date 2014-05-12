using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

using PowerSystem.IEEEComtrade;

namespace PowerSystem.CMath
{
    static class Fc
    {
        static private double[] Effective(this TComtrade.AChannel Channel)
        {
            var iEffective = new double[Channel.BaseComtrade.endsamp[Channel.BaseComtrade.endsamp.Length - 1]];
            if (Channel.a != 0)
            {
                if (Channel.BaseComtrade.samp.Length != 1)
                {
                    throw (new Exception("Problema com multitaxa na função Effective"));
                }
                ulong T = (ulong)(Channel.BaseComtrade.samp[0] / (Channel.BaseComtrade.lf));
                double[] vV = new double[T];
                double V = 0;
                for (ulong n = 0; n < T; n++)
                {
                    vV[n] = Math.Pow(Channel.Values[n] + Channel.b / Channel.a, 2);
                    V += vV[n];
                }
                for (ulong n = T; n < Channel.BaseComtrade.endsamp[Channel.BaseComtrade.endsamp.Length - 1]; n++)
                {
                    V -= vV[n % T];
                    vV[n % T] = Math.Pow(Channel.Values[n] + Channel.b / Channel.a, 2);
                    V += vV[n % T];
                    iEffective[n - T / 2] = Math.Sqrt(V / T);
                }
                for (ulong n = 0; n < T / 2; n++)
                {
                    iEffective[n] = iEffective[T / 2];
                    iEffective[Channel.BaseComtrade.endsamp[Channel.BaseComtrade.endsamp.Length - 1] - n - 1] = iEffective[Channel.BaseComtrade.endsamp[Channel.BaseComtrade.endsamp.Length - 1] - T / 2 - 1];
                }
            }
            return iEffective;
        }
        public class TSteadyDescriptor : List<TSteadyDescriptor.TSteady>
        {
            public TInstance.TNODE.TLINE Line;
            public class TSteady
            {
                public enum TRegime { Permanente = 0, Transitório = 1 };
                public DateTime start = DateTime.MinValue, end = DateTime.MaxValue;
                public TRegime Regime;
                public Complex Value;
            }
            public TSteadyDescriptor(TInstance.TNODE.TLINE Line)
            {
                this.Line = Line;

                //const double puAmpSteady = 0.05f;
                //const ulong nCyclesSteady = 2;
                //ulong TimeSteady = (ulong)(nCyclesSteady * Channel.BaseComtrade.samp[Channel.BaseComtrade.samp.Length - 1] / Channel.BaseComtrade.lf);

                //List<TSteadyDescriptor> iCollectionSteadyDescriptor = new List<TSteadyDescriptor>();
                //double nT = 1, nP = 1;
                //bool End = false;
                //for (ulong n = 0; !End; n++)
                //{
                //    T.Value = Channel.Phasor[n] / nT + T.Value * ((nT - 1) / nT);
                //    P.Value = Channel.Phasor[n] / nP + P.Value * ((nP - 1) / nP);
                //    End = ((1 + n) == Channel.BaseComtrade.endsamp[Channel.BaseComtrade.endsamp.Length - 1]);
                //    if (((1 - Channel.Phasor[n] / P.Value).Magnitude > puAmpSteady) || End)
                //    {
                //        if (nP > TimeSteady)
                //        {
                //            if (nT - nP > 0)
                //            {
                //                T.end = (ulong)(nT - nP) + T.start - 1;
                //                T.Value = T.Value * (nT / (nT - nP)) - P.Value * (nP / (nT - nP));
                //                T.Regime = TSteadyDescriptor.TRegime.Transitório;

                //                T.BaseChannel = Channel;
                //                iCollectionSteadyDescriptor.Add(T);
                //            }
                //            P.end = (ulong)nP + P.start - 1;
                //            P.Regime = TSteadyDescriptor.TRegime.Permanente;
                //            P.BaseChannel = Channel;
                //            iCollectionSteadyDescriptor.Add(P);

                //            P = new TSteadyDescriptor();
                //            P.start = n;

                //            T = new TSteadyDescriptor();
                //            T.start = n;

                //            nT = nP = 0;
                //        }
                //        else if (End)
                //        {
                //            T.end = (ulong)(nT) + T.start - 1;
                //            T.Regime = TSteadyDescriptor.TRegime.Transitório;
                //            T.BaseChannel = Channel;
                //            iCollectionSteadyDescriptor.Add(T);
                //        }
                //        else
                //        {
                //            P.Value = 0;
                //            P.start = n;
                //            nP = 0;
                //        }
                //    }
                //    nT++;
                //    nP++;
                //};
            }
        }
        static public TSteadyDescriptor SteadyDescriptor(this TInstance.TNODE.TLINE Line)
        {
            return new TSteadyDescriptor(Line);
        }
    }
}
