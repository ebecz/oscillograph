using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Windows.Forms;

namespace PowerSystem.NetWork
{
    public class ANA
    {
        /// <summary>
        /// Classe auxiliar com funções de leitura e conversão
        /// </summary>
        internal class ANAFAS_Reader
        {
            internal string str;
            internal ANAFAS_Reader(string str)
            {
                this.str = str;
            }
            internal string Read(int Start, int End)
            {
                return Substring(Start, End).Trim();
            }
            private string Substring(int Start, int End)
            {
                if (Start < str.Length)
                {
                    End = Math.Min(End, str.Length);
                    return str.Substring(Start - 1, End - (Start - 1));
                }
                else
                {
                    return "";
                }
            }
            internal char GetChar(int p)
            {
                if (str.Length > p)
                {
                    return str[p - 1];
                }
                else
                {
                    return ' ';
                }
            }
            internal T Read<T>(int Start, int End, T Default)
            {
                string V = Substring(Start, End).Trim();
                if (V == "")
                {
                    return Default;
                }
                else
                {
                    return (T)Convert.ChangeType(V, typeof(T), CultureInfo.InvariantCulture);
                }
            }
            internal DateTime Read(int Start, int End, DateTime Default)
            {
                int day = Read(Start, Start + 1, 1);
                int month = Read(Start + 2, Start + 3, 1);
                int year = Read(Start + 4, Start + 7, 0);
                string V = Substring(Start, End).Trim();
                if (year == 0)
                {
                    return Default;
                }
                else
                {
                    return new DateTime(year, month, day);
                }
            }
        }
        /// <summary>
        ///  TD: Tipo de Dado
        /// </summary>
        internal enum TD
        {
            /// <summary>
            ///  Dado Planejado
            /// </summary>
            PL,
            /// <summary>
            ///  Dado de Projeto
            /// </summary>
            PR,
            /// <summary>
            ///  Dado de Comissionamento
            /// </summary>
            CO
        }
        /// <summary>
        /// Tabela de Códigos de Atualização
        /// </summary>
        internal enum CHNG { Include, Exclude, Modificate };
        /// <summary>
        /// Classe que descreve Barra
        /// </summary>
        internal class BAR
        {
            /// <summary>
            /// Tabela de Tipos de Barra
            /// </summary>
            internal enum MP { Normal, MidPoint, Derivation };
            /// <summary>
            /// Numero da Barra
            /// </summary>
            internal uint NB;
            /// <summary>
            /// Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            /// Flag indicando se a barra está ativa, se não estiver todos os ramos também não estarão
            /// </summary>
            internal bool enable;
            /// <summary>
            /// Tipo de Barra
            /// </summary>
            internal MP mp;
            /// <summary>
            /// Nome da Barra
            /// </summary>
            internal string BarName;
            /// <summary>
            /// Tensão Basse da Barra
            /// </summary>
            internal float VBASE;
            /// <summary>
            /// Fasor que indica o Fasor de Pré-Falta
            /// </summary>
            internal Complex VPRE;
            /// <summary>
            /// Corrente limite
            /// </summary>
            internal float DISJUN;
            /// <summary>
            /// Data de Entrada de operação da Linha
            /// </summary>
            internal DateTime DATA_I;
            /// <summary>
            /// Data de Saída de operação da Linha
            /// </summary>
            internal DateTime DATA_F;
            /// <summary>
            /// Número do SubSistema
            /// </summary>
            internal int SubSistema;
            /// <summary>
            /// Número da SubÁrea
            /// </summary>
            internal int SubÁrea;
            /// <summary>
            /// Constructor para o Leitor
            /// </summary>
            internal BAR(ANAFAS_Reader S)
            {
                NB = S.Read(1, 5, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'A':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                switch (S.GetChar(7))
                {
                    case 'D':
                    case 'd':
                        enable = false;
                        break;
                    case 'L':
                    case 'l':
                    case ' ':
                        enable = true;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                switch (S.GetChar(8))
                {
                    case ' ':
                    case '0':
                        mp = MP.Normal;
                        break;
                    case '1':
                        mp = MP.MidPoint;
                        break;
                    case '2':
                        mp = MP.Derivation;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                BarName = S.Read(10, 21);
                {
                    double magnitude, phase;
                    magnitude = S.Read(23, 26, (double)0);
                    phase = S.Read(27, 30, (double)0);
                    VPRE = Complex.FromPolarCoordinates(magnitude, phase * Math.PI / 180);
                }
                VBASE = S.Read(32, 35, float.NaN);
                DISJUN = S.Read(37, 42, float.PositiveInfinity);
                DATA_I = S.Read(53, 60, DateTime.MinValue);
                DATA_F = S.Read(61, 68, DateTime.MaxValue);
                SubSistema = S.Read(70, 72, 0);
                SubÁrea = S.Read(74, 75, 0);
            }

            public string FullDescription
            {
                get
                {
                    string S = string.Empty;
                    foreach (var v in this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                    {
                        S = S + v.Name + ":" + v.GetValue(this) + Environment.NewLine;
                    }
                    return S;
                    //return "Node Name:" + this.BarName + Environment.NewLine +
                    //    "Entrada:" + this.DATA_I.ToLongDateString() + Environment.NewLine +
                    //    "Saída:" + this.DATA_F.ToLongDateString() + Environment.NewLine +
                    //    "Tensão:" + this.VBASE + Environment.NewLine +
                    //    "Tipo de Barra:" + this.mp.ToString();
                }
            }
        }
        /// <summary>
        /// Classe que descreve Circuito de Ramos
        /// </summary>
        internal class CIR
        {
            /// <summary>
            /// Tabela com Tipos de Circuito
            /// </summary>
            internal enum TIPC
            {
                /// <summary>
                /// Gerador
                /// </summary>
                G,
                /// <summary>
                /// Linha
                /// </summary>
                L,
                /// <summary>
                /// Transformador
                /// </summary>
                T,
                /// <summary>
                /// Carga com impedância Constante
                /// </summary>
                C,
                /// <summary>
                /// Reator ou Capacitor Shunt
                /// </summary>
                H,
                /// <summary>
                /// Capacitor Série
                /// </summary>
                S,
                /// <summary>
                /// Transformador de Aterramento
                /// </summary>
                Z
            };
            /// <summary>
            /// Barra de Origem, "DE"
            /// </summary>
            internal uint BF;
            /// <summary>
            /// Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            /// Estado Operativo do Circuito
            /// </summary>
            internal bool enabled;
            /// <summary>
            /// Barra de Destino, "PARA"
            /// </summary>
            internal uint BT;
            /// <summary>
            /// Identificador de Circuito em Paralelo, Número do Circuito
            /// </summary>
            internal uint NC;
            /// <summary>
            /// Tipo do Circuito
            /// </summary>
            public TIPC tipc;
            /// <summary>
            /// Resistência e Reatância de sequencias positivas e zero
            /// </summary>
            internal float R1, X1, R0, X0;
            /// <summary>
            /// Nome do circuito
            /// </summary>
            internal string Circuit_Name;
            /// <summary>
            /// Susceptância total da Linha
            /// </summary>
            internal float S1, S0;
            /// <summary>
            /// Potências Geradas
            /// </summary>
            internal float Pg, Qg;
            /// <summary>
            /// Relação de transformação TAP:1
            /// </summary>
            internal float TAP;
            /// <summary>
            ///  n. da barra do trafo delta-estrela onde se encontra olado delta
            /// </summary>
            internal uint TB;
            /// <summary>
            ///  n. do circuito do ramo série associado ao ramo shunt*, que está sendo especificado
            /// </summary>
            internal uint TC;
            /// <summary>
            ///  Número da Área do circuito
            /// </summary>
            internal uint IA;
            /// <summary>
            ///  defasagem de trafo  ∆-Y. Opcional. Valor, em graus, de quanto as tensões da barra “para” estão adiantadas em relaçãoàs tensões da barra “de”.
            /// </summary>
            internal float DEF;
            /// <summary>
            ///  Indica que o defasamento fornecido no campo DEF é explícito, caso contrário não.
            /// </summary>
            internal bool IE;
            /// <summary>
            ///  Número da Área do circuito
            /// </summary>
            internal uint SA;
            /// <summary>
            ///  Número Total de unidades idênticas que compõe o grupo.
            /// </summary>
            internal uint NUN;
            /// <summary>
            ///  Número de unidades do grupo que estão em operação
            /// </summary>
            internal uint NOP;
            /// <summary>
            ///  Capacidade de interrupção do disjuntor localizado no terminal DE do circuito
            /// </summary>
            internal float DJBF;
            /// <summary>
            ///  Capacidade de interrupção do disjuntor localizado no terminal PARA do circuito
            /// </summary>
            internal float DJBT;
            /* <summary>
                Data de entrada em operação do circuito/grupo. Opcional. Define 
                o dia e/ou mês e/ou ano a partir do qual o circuitopassa a ser considerado 
                eletricamente no sistema. Em datas anteriores à data informada neste 
                campo, tanto ele próprio quanto os seus eventuais acoplamentos mútuos de 
                seqüência zero e shunts de linha (caso seja um circuito tipo linha de 
                transmissão) são desconsiderados eletricamente. Seas posições 
                correspondentes ao número do dia forem deixadas em  branco, é assumido 
                o dia número 01 (primeiro dia do mês). Se as posições correspondentes ao 
                número do mês forem deixadas em branco, é assumido  o mês número 01 
                (primeiro mês do ano). Se as posições correspondentes ao número do ano 
                forem deixadas em branco, é assumido que a data nãofoi fornecida e 
                considerado que o circuito sempre existiu (data de  entrada igual a 
                00000000). 
            </summary>*/
            public DateTime DATA_I;
            /// <summary>
            ///  Data de saída
            /// </summary>
            public DateTime DATA_F;
            /// <summary>
            ///  Nome por extenço
            /// </summary>
            internal string Nome;
            internal CIR(ANAFAS_Reader S)
            {
                BF = S.Read(1, 5, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'A':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                switch (S.GetChar(7))
                {
                    case 'D':
                    case 'd':
                        enabled = false;
                        break;
                    case 'L':
                    case 'l':
                    case ' ':
                        enabled = true;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                BT = S.Read(8, 12, (uint)0);
                NC = S.Read(15, 16, (uint)0);
                switch (S.GetChar(17))
                {
                    case 'C':
                    case 'c':
                        tipc = TIPC.C;
                        break;
                    case 'G':
                    case 'g':
                        tipc = TIPC.G;
                        break;
                    case 'H':
                    case 'h':
                        tipc = TIPC.H;
                        break;
                    case 'L':
                    case 'l':
                        tipc = TIPC.L;
                        break;
                    case 'S':
                    case 's':
                        tipc = TIPC.S;
                        break;
                    case 'T':
                    case 't':
                        tipc = TIPC.T;
                        break;
                    case 'Z':
                    case 'z':
                        tipc = TIPC.Z;
                        break;
                    default:
                        tipc = TIPC.L;
                        //throw (new Exception("Problema com uma barra. linha:"));
                        break;
                }
                R1 = S.Read(18, 23, (float)0) / 10000;
                X1 = S.Read(24, 29, (float)0) / 10000;
                R0 = S.Read(30, 35, (float)0) / 10000;
                X0 = S.Read(36, 41, (float)0) / 10000;
                Circuit_Name = S.Read(42, 47);
                S1 = S.Read(48, 52, (float)0);
                Pg = S.Read(48, 52, (float)0);
                S0 = S.Read(53, 57, (float)0);
                Qg = S.Read(53, 57, (float)0);
                TAP = S.Read(58, 62, (float)0);
                TB = S.Read(63, 67, (uint)0);
                TC = S.Read(68, 69, (uint)0);
                IA = S.Read(70, 72, (uint)0);
                DEF = S.Read(73, 75, (float)0);
                IE = S.GetChar(76) == 'E';
                SA = S.Read(109, 111, (uint)0);
                NUN = S.Read(116, 118, (uint)0);
                NOP = S.Read(119, 121, (uint)0);
                DJBF = S.Read(123, 128, (float)0);
                DJBT = S.Read(132, 137, (float)0);
                DATA_I = S.Read(161, 168, DateTime.MinValue);
                DATA_F = S.Read(169, 176, DateTime.MaxValue);
                Nome = S.Read(200, 219);
            }

            public string FullDescription
            {
                get
                {
                    string S=string.Empty;
                    foreach (var v in this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                    {
                        S = S + v.Name + ":" + v.GetValue(this) + Environment.NewLine;
                    }
                    return S;
                }
            }
        }
        /// <summary>
        ///  Dados de impedância mútua
        /// </summary>
        internal class DMUT
        {
            /// <summary>
            ///  Número da barra "DE" das linhas com impedância mútua
            /// </summary>
            internal uint BF1, BF2;
            /// <summary>
            ///  Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            ///  Estado da Linha
            /// </summary>
            internal bool enabled;
            /// <summary>
            ///  Número da barra "Para" das linhas com impedância mútua
            /// </summary>
            internal uint BT1, BT2;
            /// <summary>
            ///  N. do circuito das linhas
            /// </summary>
            internal uint NC1, NC2;
            /// <summary>
            ///  Parte resistiva da impedância mútua
            /// </summary>
            internal float RM;
            /// <summary>
            ///  Parte reativa da impedância mútua
            /// </summary>
            internal float XM;
            /// <summary>
            ///  Porcentagem inicial do tracho acoplado da Linha 1/2
            /// </summary>
            internal float pI1, pI2;
            /// <summary>
            ///  Porcentagem final do tracho acoplado da Linha 1/2
            /// </summary>
            internal float pF1, pF2;
            /// <summary>
            ///  Número da Área da Mútua
            /// </summary>
            internal uint IA;
            /// <summary>
            ///  Número da SubÁrea
            /// </summary>
            internal uint SA;
            internal DMUT(ANAFAS_Reader S)
            {
                BF1 = S.Read(1, 5, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'I':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                switch (S.GetChar(7))
                {
                    case 'D':
                    case 'd':
                        enabled = false;
                        break;
                    case 'L':
                    case 'l':
                    case ' ':
                        enabled = true;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                BT1 = S.Read(8, 12, (uint)0);
                NC1 = S.Read(13, 16, (uint)1);
                BF2 = S.Read(17, 21, (uint)0);
                BT2 = S.Read(24, 28, (uint)0);
                NC2 = S.Read(29, 32, (uint)1);
                RM = S.Read(33, 38, (float)0);
                XM = S.Read(39, 44, (float)0);
                pI1 = S.Read(46, 51, (float)0);
                pF1 = S.Read(52, 57, (float)100);
                pI2 = S.Read(58, 63, (float)0);
                pF2 = S.Read(64, 69, (float)100);
                IA = S.Read(70, 72, (uint)1);
                SA = S.Read(74, 75, (uint)0);
            }
        }
        /// <summary>
        ///  Dados de Shunt da Linha
        /// </summary>
        internal class DSHL
        {
            /// <summary>
            ///  N. da Primeira barra "DE"
            /// </summary>
            internal uint BF;
            /// <summary>
            ///  Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            ///  Estado Operativo
            /// </summary>
            internal bool enable;
            /// <summary>
            ///  n. da Segunda Barra Terminal
            /// </summary>
            internal uint BT;
            /// <summary>
            ///  n. do Circuito
            /// </summary>
            internal uint NC;
            internal enum Tc
            {
                /// <summary>
                ///  D ou Branco
                /// </summary>    
                DE,
                /// <summary>
                ///  P
                /// </summary>    
                PARA
            };
            /// <summary>
            ///  Terminal do circuito onde o Shunt está conectado
            /// </summary>
            internal Tc T;
            /// <summary>
            ///  Número de grupo do shunt ou conjunto de shunts com parâmetros idênticos. Permite diferenciar Shunts que estejam em um mesmo terminal de circuito.
            /// </summary>
            internal uint NG;
            /// <summary>
            ///  Potência Reativa nominal Gerada pelo equipamento
            /// </summary>
            internal float QPOS;
            /// <summary>
            ///  Tipo de conexão do shunt de Linha.
            /// </summary>
            internal enum TConex
            {
                /// <summary>
                ///  Shunt com conexão tipo estrala não-aterrado
                /// </summary>
                Y,
                /// <summary>
                ///  Shunt conectado em delta
                /// </summary>
                D,
                /// <summary>
                ///  Estrela não aterrado
                /// </summary>
                N
            }
            /// <summary>
            ///  Tipo de conexão do shunt de Linha.
            /// </summary>
            TConex L;
            /// <summary>
            ///  Resistência de Aterramento
            /// </summary>
            internal float Rn;
            /// <summary>
            ///  Reatância de Aterramento
            /// </summary>
            internal float Xn;
            /// <summary>
            ///  Estado do aterramento - Válido para Shunts conectados em Estrela com Aterramento.
            /// </summary>
            internal enum Eatt
            {
                /// <summary>
                ///  Normal
                /// </summary>    
                N,
                /// <summary>
                ///  ByPass, aterramento sólido
                /// </summary>    
                B
            };
            /// <summary>
            ///  Estado do aterramento - Válido para Shunts conectados em Estrela com Aterramento.
            /// </summary>
            Eatt E;
            /// <summary>
            ///  Nome do Equipamento
            /// </summary>
            internal string Name;
            /// <summary>
            ///  Número total de unidades do grupo
            /// </summary>
            internal uint NUN;
            /// <summary>
            ///  Número total de unidades do grupo em operação
            /// </summary>
            internal uint NOP;
            /// <summary>
            ///  Número da área do Shunt de Linha
            /// </summary>
            internal uint IA;
            /// <summary>
            ///  SubÁrea
            /// </summary>
            internal uint SA;
            /// <summary>
            ///  TD: Tipo de Dado
            /// </summary>
            TD td;
            /// <summary>
            ///  Nome Extendido
            /// </summary>
            internal string Nome_extendido;
            internal DSHL(ANAFAS_Reader S)
            {
                BF = S.Read(1, 5, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'I':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                switch (S.GetChar(7))
                {
                    case 'D':
                    case 'd':
                        enable = false;
                        break;
                    case 'L':
                    case 'l':
                    case ' ':
                        enable = true;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                BT = S.Read(8, 12, (uint)0);
                NC = S.Read(15, 16, (uint)1);
                switch (S.GetChar(17))
                {
                    case 'D':
                    case 'd':
                    case ' ':
                        T = Tc.DE;
                        break;
                    case 'T':
                    case 't':
                        T = Tc.PARA;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                NG = S.Read(18, 19, (uint)1);
                QPOS = S.Read(21, 26, (float)0);
                switch (S.GetChar(28))
                {
                    case 'D':
                    case 'd':
                        L = TConex.D;
                        break;
                    case 'N':
                    case 'n':
                        L = TConex.N;
                        break;
                    case 'Y':
                    case 'y':
                        L = TConex.Y;
                        break;
                    default:
                        L = TConex.Y;
                        break;
                }
                Rn = S.Read(29, 34, (float)0);
                Xn = S.Read(35, 40, (float)0);
                switch (S.GetChar(41))
                {
                    case 'N':
                    case 'n':
                    case ' ':
                        E = Eatt.N;
                        break;
                    case 'B':
                    case 'b':
                        E = Eatt.B;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                Name = S.Read(42, 47);
                NUN = S.Read(49, 51, (uint)1);
                NOP = S.Read(52, 54, (uint)1);
                IA = S.Read(70, 72, (uint)1);
                SA = S.Read(74, 75, (uint)1);
                switch (S.Read(110, 111))
                {
                    case "PL":
                    case "pl":
                        td = TD.PL;
                        break;
                    case "PR":
                    case "pr":
                        td = TD.PR;
                        break;
                    case "CO":
                    case "co":
                        td = TD.CO;
                        break;
                    default:
                        break;
                }
                Nome_extendido = S.Read(113, 132);
            }
        }
        /// <summary>
        ///  Dados de Gerador Eólico Síncrono
        /// </summary>
        internal class DEOL
        {
            /// <summary>
            ///  Número da Barra onde o gerador eólico está conectado
            /// </summary>
            internal uint NB;
            /// <summary>
            ///  Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            ///  Estado Operativo do Gerador Eólico
            /// </summary>
            internal bool enabled;
            /// <summary>
            ///  Numero do grupo
            /// </summary>
            internal uint NG;
            /// <summary>
            ///  Potência Ativa injetada pelo gerador eólico Antes da Falta, em MW
            /// </summary>
            internal float Pinic;
            /// <summary>
            ///  Valor máximo de corrente que pode ser injeatdo pelo conversor em Ampéres RMS
            /// </summary>
            internal float Imax;
            /// <summary>
            ///  Valor mínimo de tensão em PU;
            /// </summary>
            internal float Vmim;
            /// <summary>
            ///  Fator de potência de curto
            /// </summary>
            internal float FP_CC;
            /// <summary>
            ///  Identificação do gerador eólico síncrono.
            /// </summary>
            internal string Nome;
            /// <summary>
            ///  Número total de unidades do grupo
            /// </summary>
            internal uint NUN;
            /// <summary>
            ///  Número total de unidades operantes do grupo
            /// </summary>
            internal uint NOP;
            /// <summary>
            ///  Fator de potência da Pré-Falta
            /// </summary>
            internal float FP_pre;
            /// <summary>
            ///  Número de área do Gerador Eólico
            /// </summary>
            internal uint IA;
            /// <summary>
            ///  Número de SubÁrea
            /// </summary>
            internal uint SA;
            /// <summary>
            ///  Data de entrada
            /// </summary>
            DateTime DATA_I;
            /// <summary>
            ///  Data de Saída
            /// </summary>
            DateTime DATA_F;
            /// <summary>
            ///  Potência nominal do equipamento, MVA
            /// </summary>
            internal float MVA;
            /// <summary>
            ///  Nome por extenso
            /// </summary>
            internal string Nome_extenso;
            internal DEOL(ANAFAS_Reader S)
            {
                NB = S.Read(1, 5, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'I':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                switch (S.GetChar(7))
                {
                    case 'D':
                    case 'd':
                        enabled = false;
                        break;
                    case 'L':
                    case 'l':
                    case ' ':
                        enabled = true;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                NG = S.Read(15, 16, (uint)1);
                Pinic = S.Read(18, 23, (float)0);
                Imax = S.Read(24, 29, (float)0);
                Vmim = S.Read(30, 35, (float)0);
                FP_CC = S.Read(36, 41, (float)1);
                Nome = S.Read(42, 47);
                NUN = S.Read(49, 51, (uint)0);
                NOP = S.Read(52, 54, (uint)0);
                FP_pre = S.Read(56, 61, (float)1);
                IA = S.Read(70, 72, (uint)1);
                SA = S.Read(74, 75, (uint)1);
                MVA = S.Read(87, 91, (float)1);
                DATA_I = S.Read(93, 100, DateTime.MinValue);
                DATA_I = S.Read(101, 108, DateTime.MaxValue);
                Nome_extenso = S.Read(113, 132);
            }
        }
        /// <summary>
        ///  Dados de Área
        /// </summary>
        internal class DARE
        {
            /// <summary>
            ///  Número da Área
            /// </summary>
            internal uint NUN;
            /// <summary>
            ///  Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            ///  Identificação da Área
            /// </summary>
            internal string Nome;
            internal DARE(ANAFAS_Reader S)
            {
                NUN = S.Read(1, 3, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'I':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                Nome = S.Read(18, 54);
            }
        }
        internal class DMOV
        {
            /// <summary>
            ///  Bara "DE"
            /// </summary>
            internal uint BF;
            /// <summary>
            ///  Código de Atualização
            /// </summary>
            internal CHNG chng;
            /// <summary>
            ///  Estado operativo
            /// </summary>
            internal bool enable;
            /// <summary>
            ///  Barra Para
            /// </summary>
            internal uint BT;
            /// <summary>
            ///  Número do circuito progido, caso haja capacitores série em paralelo.
            /// </summary>
            internal uint NC;
            /// <summary>
            ///  Base da tensão do circuito protegido
            /// </summary>
            internal float VBAS;
            /// <summary>
            ///  Corrente de proteção
            /// </summary>
            internal float IPR;
            /// <summary>
            ///  Valor de corrente que provoca disparo no gap
            /// </summary>
            internal float IMAX;
            /// <summary>
            ///  Energia máxima que o MOV pode absorver.
            /// </summary>
            internal float EMAX;
            /// <summary>
            ///  Valor de potência instantanea dissipada no MOV que provoca o disparo do GAP
            /// </summary>
            internal float PMAX;
            /// <summary>
            ///  Tensão entre os terminais do MOV a partir da qual o varistor começa a conduzir valores significativos de corrente.
            /// </summary>
            internal float VPR;
            /// <summary>
            /// Tipo de disparo do GAP
            /// </summary>
            internal enum D { triple, mono };
            /// <summary>
            /// Tipo de disparo do GAP
            /// </summary>
            D d;
            /// <summary>
            /// Tipo de dado:
            /// </summary>
            TD td;
            /// <summary>
            /// Nome
            /// </summary>
            internal string NOME;
            internal DMOV(ANAFAS_Reader S)
            {
                BF = S.Read(1, 5, (uint)0);
                switch (S.GetChar(6))
                {
                    case ' ':
                    case '0':
                    case 'I':
                        chng = CHNG.Include;
                        break;
                    case '1':
                    case 'E':
                        chng = CHNG.Exclude;
                        break;
                    case '2':
                    case '4':
                    case 'M':
                        chng = CHNG.Modificate;
                        break;
                    default:
                        throw (new Exception("Problema com uma barra. linha:"));
                }
                switch (S.GetChar(7))
                {
                    case 'D':
                    case 'd':
                        enable = false;
                        break;
                    case 'L':
                    case 'l':
                    case ' ':
                        enable = true;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                BT = S.Read(8, 12, (uint)0);
                NC = S.Read(13, 16, (uint)0);
                VBAS = S.Read(18, 21, (float)0);
                IPR = S.Read(23, 30, (float)0);
                IMAX = S.Read(32, 39, (float)0);
                EMAX = S.Read(41, 48, (float)0);
                PMAX = S.Read(59, 66, (float)0);
                switch (S.GetChar(74))
                {
                    case '3':
                    case ' ':
                        d = D.triple;
                        break;
                    case '1':
                        d = D.mono;
                        break;
                    default:
                        throw (new Exception("Problema com o valor de enable, linha:"));
                }
                switch (S.Read(110, 111))
                {
                    case "PL":
                    case "pl":
                        td = TD.PL;
                        break;
                    case "PR":
                    case "pr":
                        td = TD.PR;
                        break;
                    case "CO":
                    case "co":
                        td = TD.CO;
                        break;
                    default:
                        break;
                }
                NOME = S.Read(200, 219);
            }

        }
        internal enum FileType { PECO, ANAFAS };
        internal enum TPF { TPFnone, TPFinternal, TPFexternal };
        internal FileType TIPO;
        internal string TITU;
        internal TPF tpf;
        internal float SBASE = 100;
        internal List<string> Coments;
        internal List<BAR> BARS = new List<BAR>();
        internal List<CIR> CIRS = new List<CIR>();
        internal List<DMUT> DMUTS = new List<DMUT>();
        internal List<DSHL> DSHLS = new List<DSHL>();
        internal List<DEOL> DEOLS = new List<DEOL>();
        internal List<DARE> DARES = new List<DARE>();
        internal List<DMOV> DMOVS = new List<DMOV>();
        public ANA() { }
        public ANA(FileStream AnaFile)
        {
            Load(AnaFile);
        }
        public void Load(FileStream AnaFile)
        {
            StreamReader Sr = new StreamReader(AnaFile, Encoding.GetEncoding("Windows-1252"));
            while (!Sr.EndOfStream)
            {
                string S = Sr.ReadLine();
                string[] T = S.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (T[0])
                {
                    case "0":
                    case "TIPO":
                        ANAFAS_Reader R = new ANAFAS_Reader(Sr.ReadLine());
                        switch (R.GetChar(1))
                        {
                            case 'p':
                            case 'P':
                            case ' ':
                                TIPO = FileType.PECO;
                                break;
                            case 'A':
                            case 'a':
                                TIPO = FileType.ANAFAS;
                                break;
                            default:
                                throw new Exception("Format File unkow");
                        }
                        switch (R.GetChar(3))
                        {
                            case '0':
                            case ' ':
                                tpf = TPF.TPFnone;
                                break;
                            case '1':
                                tpf = TPF.TPFinternal;
                                break;
                            case '2':
                                tpf = TPF.TPFexternal;
                                break;
                            default:
                                throw new Exception("Format File unkow");
                        }
                        break;
                    case "1":
                    case "TITU":
                        if (T[1] == "1")
                        {
                            Coments = new List<string>();
                        }
                        else
                        {
                            Console.WriteLine("Desconsiderando comentários");
                        }
                        TITU = Sr.ReadLine();
                        break;
                    case "2":
                    case "CMNT":
                        if (Coments != null)
                        {
                            Coments.Add(Sr.ReadLine());
                        }
                        else
                        {
                            Sr.ReadLine();
                        }
                        break;
                    case "BASE":
                    case "100":
                        T = Sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        SBASE = Convert.ToSingle(T[0].Trim());
                        if (SBASE == 0)
                        {
                            SBASE = 100;
                        }
                        break;
                    case "38":
                    case "DBAR":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    BARS.Add(new BAR(new ANAFAS_Reader(S)));
                                }
                            }
                        }
                        break;
                    case "37":
                    case "DCIR":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    CIRS.Add(new CIR(new ANAFAS_Reader(S)));
                                }
                            }
                        }
                        break;
                    case "39":
                    case "DMUT":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    DMUTS.Add(new DMUT(new ANAFAS_Reader(S)));
                                }
                            }
                        }
                        break;
                    case "36":
                    case "DMOV":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    DMOVS.Add(new DMOV(new ANAFAS_Reader(S)));
                                }
                            }
                        }
                        break;
                    case "35":
                    case "DSHL":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    DSHLS.Add(new DSHL(new ANAFAS_Reader(S)));
                                }
                            }
                        }
                        break;
                    case "DEOL":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S[0] != '(')
                            {
                                DEOLS.Add(new DEOL(new ANAFAS_Reader(S)));
                            }
                        }
                        break;
                    case "DARE":
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    DARES.Add(new DARE(new ANAFAS_Reader(S)));
                                }
                            }
                        }
                        break;
                    case "99":
                        return;
                    default:
                        Console.WriteLine("Não entendi esta Tag:" + T[0]);
                        for (S = Sr.ReadLine(); S != "99999" && !Sr.EndOfStream; S = Sr.ReadLine())
                        {
                            if (S.Length != 0)
                            {
                                if (S[0] != '(')
                                {
                                    //Nothing todo here!
                                    //       ____            
                                    //      /. . \          
                                    //      | --  |        
                                    //       \___/  _           
                                    //        /_\_\| |           
                                    //           | | |      
                                    //         _/_/ \\       
                                    //        | |   /|\\
                                }
                            }
                        }
                        break;
                }
            }
        }
    };
}