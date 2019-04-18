using System;
using System.IO;
using System.Globalization;
using PowerSystem.IEEEComtrade;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using PowerSystem.CMath;

namespace PowerSystem.IEEEComtrade
{
    public class TComtrade
    {
        public class TBaseChannel
        {
            public struct TPh
            {
                public string ph;
                private System.Drawing.Color pColor;
                public System.Drawing.Color Color
                {
                    get
                    {
                        if (pColor.IsEmpty)
                        {
                            return this.GetRegistryColor();
                        }
                        else
                        {
                            return pColor;
                        }
                    }
                    set
                    {
                        pColor = value;
                    }
                }
                public static implicit operator TPh(string s)
                {
                    TPh V = new TPh();
                    V.ph = s;
                    return V;
                }
                public static implicit operator string(TPh uu)
                {
                    return uu.ph;
                }
            }
            /// <summary>
            /// Nome do canal
            /// </summary>     
            public string ch_id;
            /// <summary>
            /// Fase do canal
            /// </summary>     
            public TPh ph;
            /// <summary>
            /// Equipamento monitorado
            /// </summary>     
            public string ccbm;
            /// <summary>
            /// Numero do canal
            /// </summary>         
            public uint n;
            /// <summary>
            /// Comtrade Original
            /// </summary>     
            public TComtrade BaseComtrade;
            public TBaseChannel(TComtrade BaseComtrade)
            {
                this.BaseComtrade = BaseComtrade;
            }
            public override string ToString()
            {
                if (ccbm == string.Empty)
                {
                    return ch_id;
                }
                else
                {
                    return ccbm;
                }
            }
        }
        public class AChannel : TBaseChannel
        {
            public struct Tuu
            {
                public enum Tu { V, A , Unknow};
                public double M;
                public string uu_original;
                public Tu uu;
                public static implicit operator Tuu(string s)
                {
                    Tuu V=new Tuu();
                    V.uu_original = s;
                    V.M = 1;
                    foreach (char c in s.ToLower())
                    {
                        switch (c)
                        {
                            case 'k':
                                V.M = 1000;
                                break;
                            case 'm':
                                V.M = 1000000;
                                break;
                            case 'v':
                                V.uu = Tu.V;
                                return V;
                            case 'a':
                                V.uu = Tu.A;
                                return V;
                            default:
                                V.uu = Tu.Unknow;
                                V.M = 1;
                                return V;
                        }
                    }
                    return V;
                }
                public static implicit operator string(Tuu uu)
                {
                    return uu.uu_original;
                }
            }
            /// <summary>
            /// Relação do Primário.
            /// </summary>
            public float primary;
            /// <summary>
            /// Relação do Secundário.
            /// </summary>
            public float secondary;
            public enum TPS { P, S };
            /// <summary>
            /// Valores em relação ao primário ou secundário. P ou S
            /// </summary>
            public TPS PS;
            /// <summary>
            /// Unidade.
            /// </summary>
            public Tuu uu;
            /// <summary>
            /// Multiplicador y[n]=V[n].a+b
            /// </summary>
            public float a;
            /// <summary>
            /// Offset y[n]=V[n].a+b
            /// </summary>
            public float b;
            /// <summary>
            /// Atraso de amostragem x[n]=skew+n/rate
            /// </summary>
            public float skew;
            /// <summary>
            /// Valor mínimo dos dados.
            /// </summary>
            public int min;
            /// <summary>
            /// Valor máximo dos dados.
            /// </summary>
            public int max;
            /// <summary>
            /// Vetor com os dados.
            /// </summary>
            public short[] Values;
            public AChannel(TComtrade BaseComtrade):base(BaseComtrade)
            {
                //Nothing ToDo here!
            }
            /// <summary>
            /// Retorna o valor real a uma determinada amostra
            /// </summary>
            public double this[ulong n]
            {
                get
                {
                    return Values[n] * a + b;
                }
            }
            /// <summary>
            /// Retorna o valor real em ms em relação ao início do registro contrade
            /// Esta função não deve ser usada repetidamente devido a limitação de desempenho.
            /// </summary>
            public double this[DateTime T]
            {
                get
                {
                    if (T < BaseComtrade.start_time || T > BaseComtrade.end_time)
                    {
                        return 0;
                    }
                    else
                    {
                        long n = ((T - BaseComtrade.start_time).Ticks * (long)BaseComtrade.endsamp[0] / (BaseComtrade.end_time - BaseComtrade.start_time).Ticks);
                        return Values[n] * a + b;
                    }
                }
            }
            /// <summary>
            /// Retorna o valor real em ms em relação ao início do registro contrade
            /// Esta função não deve ser usada repetidamente devido a limitação de desempenho.
            /// </summary>
            public CMath.TPhasor Phasor
            {
                get
                {
                    if (_Phasor == null)
                    {
                        _Phasor = new TPhasor(this);
                    }
                    return _Phasor;
                }
            }
            CMath.TPhasor _Phasor;
            /// <summary>
            /// Barra na qual a medição está pendurada.
            /// Esta informação é necessára para Fazer As comparações no Sistema PU
            /// </summary>     
            public TInstance.TNODE Barra;
        }
        public class DChannel : TBaseChannel
        {
            /// <summary>
            /// Valor padrão
            /// </summary>     
            public bool y;
            /// <summary>
            /// Valores[n]
            /// </summary>  
            public bool[] Values;
            /// <summary>
            /// Flags que indica se houve mudança no canal
            /// </summary>
            public bool Static=true;

            public DChannel(TComtrade BaseComtrade):base(BaseComtrade)
            {
                //Nothing ToDo Here!
            }
            public DateTime ChangeTime;
        }
        /// <summary>
        /// Estação de medição
        /// </summary>
        public string station_name;
        /// <summary>
        /// Identificador do aparelho
        /// </summary>
        public string rec_dev_id;          
        /// <summary>
        /// Padrão do Arquivo
        /// </summary>
        public int rev_year;
        /// <summary>
        /// Numero de canais
        /// </summary>
        public uint nT;
        /// <summary>
        /// Numero de canais Analogicos
        /// </summary>
        public uint nA;                     
        /// <summary>
        /// Numero de canais Digitais
        /// </summary>
        public uint nD;
        /// <summary>
        /// Lista dos canais analogicos
        /// </summary>
        public AChannel[] AnalogChannels;
        /// <summary>
        /// Lista com os canais digitais
        /// </summary>
        public DChannel[] DigitalChannels;
        /// <summary>
        /// Lista com os tempos
        /// </summary>
        public uint[] timestamp;
        /// <summary>
        /// Frequencia nominal
        /// </summary>
        public float lf;
        /// <summary>
        /// Quantidade de taxas
        /// </summary>
        public uint nrates;
        /// <summary>
        /// Lista com as taxas de amostragens;
        /// </summary>
        public float[] samp;
        /// <summary>
        /// Lita com os limites das taxas
        /// </summary>
        public ulong[] endsamp;
        /// <summary>
        /// Data de inicio da oscilografia
        /// </summary>
        public DateTime start_time;
        /// <summary>
        /// Data de do trigger
        /// </summary>
        public DateTime triger_time;
        /// <summary>
        /// Data de final
        /// </summary>
        public DateTime end_time;
        /// <summary>
        /// ???? - Rever na norma
        /// </summary>
        public string ft;
        /// <summary>
        /// Multiplo do tempo - para longa duração
        /// </summary>
        public float timemult;
        /// <summary>
        /// Constructor principal
        /// </summary>
        public TPhasor Reference;
        /// <summary>
        /// Constructor principal
        /// </summary>
        public TComtrade(Stream cfg, Stream dat)
        {
            IFormatProvider Format =  new NumberFormatInfo();
            using (StreamReader F = new StreamReader(cfg))
            {
                string[] S;
                S = F.ReadLine().Split(',');
                station_name = S[0];
                rec_dev_id = S[1];

                if (S.Length == 3)
                {
                    if (S[2] == "1999" || S[2] == "1991" || S[2] == "2001")
                    {
                        rev_year = Convert.ToInt32(S[2]);
                    }
                    else
                    {
                        throw (new Exception("Versão não suportada:" + S[2]));
                    }
                }
                else
                {
                    rev_year = 1991;
                }

                S = F.ReadLine().Split(',');
                nT = Convert.ToUInt32(S[0]);

                for (int nn = 1; nn < S.Length; nn++)
                {
                    switch (S[nn].Substring(S[nn].Length - 1, 1))
                    {
                        case "A":
                            nA = Convert.ToUInt32(S[nn].Substring(0, S[nn].Length - 1));
                            break;
                        case "D":
                            nD = Convert.ToUInt32(S[nn].Substring(0, S[nn].Length - 1));
                            break;
                    }
                }

                AnalogChannels = new AChannel[nA];
                for (int kc = 0; kc < nA; kc++)
                {
                    S = F.ReadLine().Split(',');
                    uint k = Convert.ToUInt32(S[0], Format) - 1;
                    AnalogChannels[k] = new AChannel(this);
                    AnalogChannels[k].n = k + 1;
                    AnalogChannels[k].ch_id = S[1];
                    AnalogChannels[k].ph = S[2];
                    AnalogChannels[k].ccbm = S[3];
                    AnalogChannels[k].uu = S[4];
                    AnalogChannels[k].a = Convert.ToSingle(S[5], Format);
                    AnalogChannels[k].b = Convert.ToSingle(S[6], Format);
                    AnalogChannels[k].skew = Convert.ToSingle(S[7], Format);
                    AnalogChannels[k].min = Convert.ToInt32(S[8], Format);
                    AnalogChannels[k].max = Convert.ToInt32(S[9], Format);
                    if (rev_year != 1991)
                    {
                        AnalogChannels[k].primary = Convert.ToSingle(S[10], Format);
                        AnalogChannels[k].secondary = Convert.ToSingle(S[11], Format);
                        char c = Convert.ToChar(S[12].ToUpper(), Format);
                        if (c == 'S')
                        {
                            AnalogChannels[k].PS = AChannel.TPS.S;
                        }
                        else
                        {
                            AnalogChannels[k].PS = AChannel.TPS.P;
                        }
                    }
                    else
                    {
                        AnalogChannels[k].PS = AChannel.TPS.P;
                        AnalogChannels[k].primary = 1;
                        AnalogChannels[k].secondary = 1;
                    }
                }

                DigitalChannels = new DChannel[nD];
                for (int kc = 0; kc < nD; kc++)
                {
                    S = F.ReadLine().Split(',');
                    uint k = Convert.ToUInt32(S[0], Format) - 1;
                    DigitalChannels[k] = new DChannel(this);
                    DigitalChannels[k].n = k + 1;
                    DigitalChannels[k].ch_id = S[1];
                    if (rev_year != 1991)
                    {
                        DigitalChannels[k].ph = S[2];
                        DigitalChannels[k].ccbm = S[3];
                        if (S[4] != "")
                        {
                            DigitalChannels[k].y = Convert.ToInt16(S[4]) == 1;
                        }
                        else
                        {
                            Console.WriteLine("Problema com a identificação do canal digital:" + DigitalChannels[k].ch_id);
                        }
                    }
                    else if (rev_year == 1991)
                    {
                        DigitalChannels[k].ccbm = "Unknow";
                        DigitalChannels[k].y = Convert.ToInt16(S[2]) == 1;
                    }
                }

                lf = Convert.ToSingle(F.ReadLine(), Format);

                nrates = Convert.ToUInt32(F.ReadLine());
                if (nrates !=1)
                {
                    throw (new Exception("Ainda não há suporte para multitaxa"));
                }
                samp = new float[nrates];
                endsamp = new ulong[nrates];
                for (uint k = 0; k < nrates; k++)
                {
                    S = F.ReadLine().Split(',');
                    samp[k] = Convert.ToSingle(S[0], Format);
                    endsamp[k] = Convert.ToUInt64(S[1]);
                }
                Regex X = new Regex(" *?([\\d]+)/ *?([\\d]+)/ *?([\\d]+), *?([\\d]+): *?([\\d]+): *?([\\d]+)\\. *?([\\d]+)");
                Match M1;
                M1 = X.Match(F.ReadLine());
                if (M1.Groups.Count == 8)
                {
                    start_time = new DateTime(
                        Convert.ToInt32(M1.Groups[3].ToString()) + (M1.Groups[3].ToString().Length == 2 ? 2000 : 0),
                        Convert.ToInt32(M1.Groups[rev_year == 1991 ? 1 : 2].ToString()),
                        Convert.ToInt32(M1.Groups[rev_year == 1991 ? 2 : 1].ToString()),
                        Convert.ToInt32(M1.Groups[4].ToString()),
                        Convert.ToInt32(M1.Groups[5].ToString()),
                        Convert.ToInt32(M1.Groups[6].ToString()),
                        (int)(Convert.ToInt32(M1.Groups[7].ToString()) * Math.Pow(10, (3 - 6)))
                        );
                    end_time = start_time;
                    for (int i = 0; i < nrates; i++)
                    {
                        end_time = end_time.AddSeconds(endsamp[i] / samp[i]);
                    }
                }else{
                    throw (new Exception("Problema com a leitura da data de inicio da Oscilografia"));
                }
                M1 = X.Match(F.ReadLine());
                if (M1.Groups.Count == 8)
                {
                    triger_time = new DateTime(
                        Convert.ToInt32(M1.Groups[3].ToString()) + (M1.Groups[3].ToString().Length == 2 ? 2000 : 0),
                        Convert.ToInt32(M1.Groups[rev_year == 1991 ? 1 : 2].ToString()),
                        Convert.ToInt32(M1.Groups[rev_year == 1991 ? 2 : 1].ToString()),
                        Convert.ToInt32(M1.Groups[4].ToString()),
                        Convert.ToInt32(M1.Groups[5].ToString()),
                        Convert.ToInt32(M1.Groups[6].ToString()),
                        (int)(Convert.ToInt32(M1.Groups[7].ToString()) * Math.Pow(10, (3 - 6)))
                        );
                }
                else
                {
                    throw (new Exception("Problema com a leitura da data de gatilho da Oscilografia"));
                }
                //Lê o tipo de dado do arquivo de dados
                ft = F.ReadLine();

                //Multiplo de tempo para registros de longa duração.
                if (rev_year != 1991)
                {
                    timemult = Convert.ToSingle(F.ReadLine());
                }
                else
                {
                    timemult = 1;
                }
                F.Close();
            }
            //Inicio da leitura dos dados
            for (uint k = 0; k < nA; k++)
            {
                AnalogChannels[k].Values = new short[endsamp[endsamp.Length - 1]];
            }
            for (uint k = 0; k < nD; k++)
            {
                DigitalChannels[k].Values = new bool[endsamp[endsamp.Length - 1]];
            }
            timestamp = new uint[endsamp[endsamp.Length - 1]];
            ulong SIndex = 0; float Ssum = 0;
            UInt32 N = 0;
            if (ft == "BINARY" || ft == "binary")
            {
                using (BinaryReader F = new BinaryReader(dat))
                {
                    {
                        for (uint k = 0; k < endsamp[endsamp.Length - 1]; k++)
                        {
                            if (endsamp[SIndex] < k)
                            {
                                Ssum += endsamp[SIndex] * timemult / samp[SIndex];
                                SIndex++;
                            }
                            //Some files in example have a corrupted sample index, so a replaced it by a sequential sample index
                            //Anyway, we must read this line, because we need jump this value
                            N = F.ReadUInt32() - 1;
                            timestamp[k] = F.ReadUInt32();
                            if (timestamp[k] == 0)
                            {
                                if (SIndex == 0)
                                {
                                    timestamp[k] = (uint)(k * 1000 / (samp[SIndex] * timemult));
                                }
                                else
                                {
                                    timestamp[k] = (uint)(Ssum + (k - endsamp[SIndex - 1]) / (samp[SIndex] * timemult));
                                }
                            }
                            for (uint kc2 = 0; kc2 < nA; kc2++)
                            {
                                AnalogChannels[kc2].Values[N] = F.ReadInt16();
                            }
                            {
                                ushort Vt = 0;
                                for (uint k2 = 0; k2 < nD; k2++)
                                {
                                    if (k2 % 16 == 0)
                                    {
                                        Vt = F.ReadUInt16();
                                    }
                                    DigitalChannels[k2].Values[N] = (Vt & (1 << (int)(k2 % 16))) != 0;
                                    if (N > 1)
                                    {
                                        if (DigitalChannels[k2].Values[N] != DigitalChannels[k2].Values[N-1])
                                        {
                                            DigitalChannels[k2].Static = false;
                                            if (DigitalChannels[k2].ChangeTime == DateTime.MinValue)
                                            {
                                                DigitalChannels[k2].ChangeTime = start_time.AddSeconds(N / this.samp[0]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //(ulong)F.BaseStream.Length == ((4 + 4 + nA * 2 + 2 * ((nD+(16 - nD)%16) / 16)) * endsamp[endsamp.Length - 1])
                        F.Close();
                    }
                }
            }
            else if (ft == "ASCII" || ft == "ascii")
            {
                using (StreamReader Fdat = new StreamReader(dat))
                {
                    //Estou tendo problemas com isso, rever o código para das suporte para casos estranhos
                    //if ((ulong)F.BaseStream.Length == ((4 + 4 + nA * 2 + 2 * ((15 + nD) / 16)) * endsamp[endsamp.Length - 1]))
                    {
                        for (uint k = 0; k < endsamp[endsamp.Length - 1]; k++)
                        {
                            string[] S = Fdat.ReadLine().Split(',');
                            if (endsamp[SIndex] < k)
                            {
                                Ssum += endsamp[SIndex] * timemult / samp[SIndex];
                                SIndex++;
                            }
                            //Some files in example have a corrupted sample index, so a replaced it by a sequential sample index
                            //Anyway, we must read this line, because we need jump this value
                            N = Convert.ToUInt32(S[0]) - 1;
                            timestamp[k] = Convert.ToUInt32(S[1]);
                            if (timestamp[k] == 0)
                            {
                                if (SIndex == 0)
                                {
                                    timestamp[k] = (uint)(k * 1000 / (samp[SIndex] * timemult));
                                }
                                else
                                {
                                    timestamp[k] = (uint)(Ssum + (k - endsamp[SIndex - 1]) / (samp[SIndex] * timemult));
                                }
                            }
                            for (uint kc2 = 0; kc2 < nA; kc2++)
                            {
                                AnalogChannels[kc2].Values[N] = Convert.ToInt16(S[2 + kc2]);
                            }
                            for (uint kc2 = 0; kc2 < nD; kc2++)
                            {
                                DigitalChannels[kc2].Values[N] = S[2 + nA + kc2] == "1";
                            }
                        }
                        Fdat.Close();
                    }
                }
            }
            else
            {
                throw (new Exception("There is no support for " + ft + " file types"));
            }
        }
        public void Write(Stream cfg, Stream dat){
            IFormatProvider FormatEng = new CultureInfo("en-us");
            //cfg.SetLength(288 + 342 * nA + 138 * nD + 45 * nrates);
            StreamWriter F = new StreamWriter(cfg);
            {
                if (rev_year == 1991)
                {
                    F.WriteLine(station_name + ',' + rec_dev_id);
                }
                else
                {
                    F.WriteLine(station_name + ',' + rec_dev_id + ',' + rev_year.ToString());
                }
                F.WriteLine(nT.ToString() + ',' + nA.ToString() + "A," + nD.ToString() + 'D');

                for (int k = 0; k < nA; k++)
                {
                    F.WriteLine(
                        AnalogChannels[k].n + "," +
                        AnalogChannels[k].ch_id + ',' +
                        AnalogChannels[k].ph + ',' +
                        AnalogChannels[k].ccbm + ',' +
                        AnalogChannels[k].uu + ',' +
                        AnalogChannels[k].a.ToString(FormatEng) + ',' +
                        AnalogChannels[k].b.ToString(FormatEng) + ',' +
                        AnalogChannels[k].skew.ToString(FormatEng) + ',' +
                        AnalogChannels[k].min + ',' +
                        AnalogChannels[k].max +
                        (rev_year != 1991 ?
                                "," + AnalogChannels[k].primary.ToString(FormatEng) + ',' + AnalogChannels[k].secondary.ToString(FormatEng) + ',' + AnalogChannels[k].PS
                            :
                                ""
                        )
                   );
                }
                for (int k = 0; k < nD; k++)
                {
                    F.WriteLine(
                        DigitalChannels[k].n + "," +
                        DigitalChannels[k].ch_id + "," +
                        (rev_year != 1991 ?
                            DigitalChannels[k].ph + "," +
                            DigitalChannels[k].ccbm + ","
                            :
                            ""
                        ) + (DigitalChannels[k].y ? 1 : 0)
                    );
                }
                F.WriteLine(lf.ToString(FormatEng));
                F.WriteLine(nrates);

                for (uint k = 0; k < nrates; k++)
                {
                    F.WriteLine(samp[k].ToString(FormatEng) + "," + endsamp[k]);
                }
                {
                    if (rev_year != 1991)
                    {
                        F.WriteLine(start_time.Day + "/" + start_time.Month + "/" + start_time.Year + "," + start_time.ToLongTimeString() + "." + start_time.Millisecond);
                        F.WriteLine(triger_time.Day + "/" + triger_time.Month + "/" + triger_time.Year + "," + triger_time.ToLongTimeString() + "." + triger_time.Millisecond);
                    }
                    else
                    {
                        F.WriteLine(start_time.Month + "/" + start_time.Day + "/" + start_time.Year + "," + start_time.ToLongTimeString() + "." + start_time.Millisecond);
                        F.WriteLine(triger_time.Month + "/" + triger_time.Day + "/" + triger_time.Year + "," + triger_time.ToLongTimeString() + "." + triger_time.Millisecond);
                    }
                }
                F.WriteLine(ft, FormatEng);
                if (rev_year != 1991)
                {
                    F.WriteLine(timemult);
                }
                if (ft == "BINARY" || ft == "binary"){
                    dat.SetLength((long)((4 + 4 + nA * 2 + 2 * ((15 + nD) / 16)) * endsamp[endsamp.Length - 1]));
                    BinaryWriter G = new BinaryWriter(dat);
                    {
                        for (UInt32 k = 0; k < endsamp[this.nrates-1]; k++)
                        {
                            G.Write(k+1);
                            G.Write(timestamp[k]);
                            for (int i = 0; i < nA; i++)
                            {
                                G.Write(AnalogChannels[i].Values[k]);
                            }
                            for (int i = 0; i < nD; i+=16)
                            {
                                UInt16 B = 0x8000, V = 0;
                                for (int j = i; j-i < 16 && j<nD; j++)
                                {
                                    V |= (DigitalChannels[j].Values[k] ? B : (UInt16)0);
                                    B >>= 1;
                                }
                                G.Write(V);
                            }
                        }
                    }
                    G.Flush();
                }
                else if (ft == "ASCII" || ft == "ascii")
                {
                    //dat.SetLength((long)((1 + 4 + 4 + nA * 2 + 2 * ((15 + nD) / 16)) * endsamp[endsamp.Length - 1]) * 2);
                    StreamWriter G = new StreamWriter(dat);
                    {
                        for (UInt32 k = 0; k < endsamp[this.nrates - 1]; k++)
                        {
                            G.Write(k + 1); 
                            G.Write(',');
                            G.Write(timestamp[k]);
                            for (int i = 0; i < nA; i++)
                            {
                                G.Write(',');
                                G.Write(AnalogChannels[i].Values[k]);
                            }
                            for (int i = 0; i < nD; i ++)
                            {
                                G.Write(',');
                                G.Write((DigitalChannels[i].Values[k] ? 1 :0));
                            }
                            G.WriteLine();
                        }
                    }
                    G.Flush();
                }
                else
                {
                    throw (new Exception("Go home write mode:" + ft + "/n You are drunk!"));
                }
                F.Flush();
            }
            cfg.Position = 0;
            dat.Position = 0;
        }
    }
}