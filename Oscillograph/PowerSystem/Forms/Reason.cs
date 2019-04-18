using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using PowerSystem.NetWork;
using PowerSystem.IEEEComtrade;

namespace PowerSystem.Reason
{
    public static class Reason
    {
        public static bool LoadReasonFile(this TInstance Instance, TComtrade Comtrade, Stream File)
        {
            if (Instance.Time.Start > Instance.Time.End)
            {
                Instance.Time.Start = Comtrade.start_time;
                Instance.Time.End = Comtrade.end_time;
            }
            if (Comtrade.start_time <= Instance.Time.End && Comtrade.end_time >= Instance.Time.Start)
            {
                Instance.Time.Start = Comtrade.start_time < Instance.Time.Start ? Comtrade.start_time : Instance.Time.Start;
                Instance.Time.A = Instance.Time.Start > Instance.Time.A ? Instance.Time.Start : Instance.Time.A;
                Instance.Time.End = Comtrade.end_time > Instance.Time.End ? Comtrade.end_time : Instance.Time.End;
                Instance.Time.B = Instance.Time.End < Instance.Time.B ? Instance.Time.End : Instance.Time.B;
                using (StreamReader F = new StreamReader(File))
                {
                    try
                    {
                        string[] S = F.ReadLine().Split(',');
                        int iend = Convert.ToInt32(S[0]);
                        bool[] D = new bool[Comtrade.DigitalChannels.Length];
                        for (int i = 0; i < iend; i++)
                        {
                            string st = F.ReadLine();
                            S = st.Split(',');
                            TInstance.TNODE.TLINE Linha = new TInstance.TNODE.TLINE(new ANA.CIR(new ANA.ANAFAS_Reader("")), null, null);
                            Linha.CIR.Nome = S[0];
                            Linha.CIR.Circuit_Name = S[0];
                            int jend = Convert.ToInt32(S[1]);
                            st = F.ReadLine();
                            S = st.Split(',');
                            float Vbase = Convert.ToSingle(S[0], System.Globalization.CultureInfo.InvariantCulture);
                            //float RTC = Convert.ToSingle(S[1]);
                            float L = Convert.ToSingle(S[2], System.Globalization.CultureInfo.InvariantCulture);

                            float a = Convert.ToSingle(S[4], System.Globalization.CultureInfo.InvariantCulture) * (Instance.Ana.SBASE / 100f),
                            b = Convert.ToSingle(S[3], System.Globalization.CultureInfo.InvariantCulture) * (Instance.Ana.SBASE / 100f);
                            Linha.CIR.R0 = Math.Min(a, b);
                            Linha.CIR.X0 = Math.Max(a, b);
                            a = Convert.ToSingle(S[6], System.Globalization.CultureInfo.InvariantCulture) * (Instance.Ana.SBASE / 100f);
                            b = Convert.ToSingle(S[5], System.Globalization.CultureInfo.InvariantCulture) * (Instance.Ana.SBASE / 100f);
                            Linha.CIR.R1 = Math.Min(a, b);
                            Linha.CIR.X1 = Math.Max(a, b);

                            Linha.L = L;

                            TInstance.TNODE Barra = null;
                            for (int j = 0; j < jend; j++)
                            {
                                st = F.ReadLine();
                                S = st.Split(',');
                                switch (S[0])
                                {
                                    case "Tensoes":
                                        foreach (TInstance.TNODE B2 in Instance.LNode_Using)
                                        {
                                            if (B2.Name == Comtrade.AnalogChannels[Convert.ToInt32(S[1]) - 1].ccbm)
                                            {
                                                Barra = B2;
                                                break;
                                            }
                                        }
                                        if (Barra == null)
                                        {
                                            Barra = new TInstance.TNODE(new ANA.BAR(new ANA.ANAFAS_Reader("")), Instance);
                                            for (uint k = 0; k < 4; k++)
                                            {
                                                int n = Convert.ToInt32(S[k + 1]);
                                                if (n > 0)
                                                {
                                                    Barra.V3F.Add(Comtrade.AnalogChannels[n - 1]);
                                                    Comtrade.AnalogChannels[n - 1].Barra = Barra;
                                                }
                                            }
                                            Barra.BAR.BarName = Barra.V3F[0].ccbm;
                                            Barra.BAR.VBASE = Vbase;
                                            //Barra.SE = Comtrade.rec_dev_id;
                                            Instance.LNode_Using.Add(Barra);
                                            Instance.LNode.Add(Barra);
                                        }
                                        if (Comtrade.Reference == null)
                                        {
                                            if (Barra.V3F.Sequence != null)
                                            {
                                                Comtrade.Reference = Barra.V3F.Sequence[1].PhaseReference;
                                            }
                                        }
                                        Linha.DE = Barra;
                                        //Adicionar barras virtuais de saída:
                                        Linha.Oposite = Linha;
                                        Linha.PARA = new TInstance.TNODE(new ANA.BAR(new ANA.ANAFAS_Reader("")), Instance);
                                        Linha.PARA.BAR.BarName = Linha.Name.ToUpper().Replace("LT_", "SE_");
                                        Linha.PARA.BAR.VBASE = Vbase;
                                        Linha.PARA.LINES.Add(Linha);

                                        Barra.LINES.Add(Linha);
                                        break;
                                    case "Correntes":
                                        for (uint k = 0; k < 4; k++)
                                        {
                                            int n = Convert.ToInt32(S[k + 1]);
                                            if (n > 0)
                                            {
                                                Linha.I3F.Add(Comtrade.AnalogChannels[n - 1]);
                                                Comtrade.AnalogChannels[n - 1].Barra = Barra;
                                            }
                                        }
                                        break;
                                    case "Digitais":
                                        for (int k = 1; k < S.Length - 1; k++)
                                        {
                                            Linha.D.Add(Comtrade.DigitalChannels[Convert.ToInt32(S[k + 1]) - 1]);
                                            D[Convert.ToInt32(S[k + 1]) - 1] = true;
                                        }
                                        break;
                                }
                            }
                            Instance.LLine_Using.Add(Linha);
                        }
                        TInstance.TNODE.TLINE LinhaD = null;
                        for (int i = 0; i < Comtrade.DigitalChannels.Length; i++)
                        {
                            if (!D[i])
                            {
                                //TBARRA.TLINHA LinhaD = FLinha(Comtrade.DigitalChannels[i], null, LLINHA);
                                if (LinhaD == null)
                                {
                                    LinhaD = new TInstance.TNODE.TLINE(new ANA.CIR(new ANA.ANAFAS_Reader("")), null, null);
                                    //LinhaD.CIR.Circuit_Name = "Remained Digitals";
                                    Instance.LLine_Using.Add(LinhaD);
                                    Instance.LNode_Using[0].LINES.Add(LinhaD);
                                    LinhaD.DE = Instance.LNode_Using[0];

                                    LinhaD.DE = Instance.LNode_Using[0];
                                    //Adicionar barras virtuais de saída:
                                    LinhaD.Oposite = LinhaD;
                                    LinhaD.PARA = new TInstance.TNODE(new ANA.BAR(new ANA.ANAFAS_Reader("")), Instance);
                                    LinhaD.PARA.BAR.BarName = "Barra Virtual para Criar uma linha para as digitais";
                                    LinhaD.PARA.LINES.Add(LinhaD);
                                }
                                LinhaD.D.Add(Comtrade.DigitalChannels[i]);
                            }
                        }
                        //foreach (var V in Instance.LNode_Using)
                        //{
                        //    V.BAR.BarName = Comtrade.rec_dev_id + " - " + V.BAR.BarName;
                        //}
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}