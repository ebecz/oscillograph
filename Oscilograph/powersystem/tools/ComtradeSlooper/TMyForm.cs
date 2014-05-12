
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ExcelLibrary.Office.Excel;

using System.Numerics;

using PowerSystem;
using PowerSystem.CMath;

using PowerSystem.Reason;

namespace PowerSystem.Tools.Comtrade_Snooper
{
    public partial class TMyForm : Form
    {
        public TMyForm()
        {
            InitializeComponent();
        }
        public Tools.TOpenFile OpenFile;
        protected override void OnLoad(EventArgs e)
        {
            FileList.Columns.Add("FILE");
            FileList.Columns.Add("SE");
            FileList.Columns.Add("DATE");
            FileList.Columns.Add("TIME");
            FileList.Columns.Add("FAULT LINE");
            FileList.Columns.Add("LENGHT");
            FileList.Columns.Add("RDP (%)");
            FileList.Columns.Add("RDP Tipo");
            FileList.Columns.Add("Takagi (%)");
            FileList.Columns.Add("Takagi Tipo");
            base.OnLoad(e);
        }
        class TMyListViewItem : ListViewItem
        {
            public FileInfo File;
            public bool Computed=false;
            public TMyListViewItem(FileInfo File)
                : base(File.Name, 1)
            {
                base.Name = File.Name;
                this.File = File;
            }
        }
        private void LookupFile(DirectoryInfo Dir)
        {
            foreach (FileInfo File in Dir.GetFiles())
            {
                if (File.Extension == ".zic")
                {
                    FileList.Items.Add(new TMyListViewItem(File));
                }
            }
            foreach (DirectoryInfo Dir2 in Dir.GetDirectories())
            {
                LookupFile(Dir2);
            }
        }
        private void LookupButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBrowser = new FolderBrowserDialog();
            FBrowser.SelectedPath = @"C:\Users\Elias\Documents\LabSpot\Projeto CAT\Oscilografias\Repositório de oscilografias\BGA";
            FileList.SuspendLayout();
            if (FBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo Dir = new DirectoryInfo(FBrowser.SelectedPath);
                LookupFile(Dir);
            }
            FileList.ResumeLayout();
        }
        bool StopSurvey = false;
        private TInstance.TNODE.TLINE GetFaultRecord(TInstance Instance, Stream Hdr, out double Position, out string Tipo)
        {
            StreamReader R = new StreamReader(Hdr);
            string[] V;
            string S;
            do
            {
                S = R.ReadLine();
            } while (S != "Fault location");
            while ((V = R.ReadLine().Trim().Split(new char[]{'\t'},StringSplitOptions.RemoveEmptyEntries))[0] != "Date")
            {
                if (V[1] != "No fault")
                {
                    if (V[1].Split(' ')[1] != "0.000")
                    {
                        Position = Convert.ToSingle(V[1].Split(' ')[1], System.Globalization.CultureInfo.InvariantCulture);
                        Tipo = V[1].Split(' ')[0].Replace(',', ' ').Trim();
                        foreach (TInstance.TNODE Barra in Instance.LNode_Using)
                        {
                            foreach (TInstance.TNODE.TLINE Linha in Barra.LINES)
                            {
                                if (Linha.Name == V[0])
                                {
                                    return Linha;
                                }
                            }
                        }
                    }
                }
            }
            Position = 0;
            Tipo = "?";
            return null;
        }
        public class TCurve
        {
            public List<Complex[]> Z = new List<Complex[]>();
            public string Name;
            TInstance.TNODE.TLINE Line;
            public ulong n = 0;
            public double Min = double.MaxValue;
            public TCurve(string Name, TInstance.TNODE.TLINE Line, TfZ fZ)
            {
                this.Name = Name;
                this.fZ = fZ;
                this.Line = Line;
                Complex[] Zn = new Complex[Line.I3F[0].BaseComtrade.endsamp[0]];
                for (ulong i = 1; i < Line.I3F[0].BaseComtrade.endsamp[0]; i++)
                {
                    Complex Z = fZ(i) / Line.Z1;
                    if (Z.Real > 0 && Min > Z.Magnitude)
                    {
                        Min = Z.Magnitude;
                        n = i;
                    }
                }
            }
            public delegate Complex TfZ(ulong n);
            public TfZ fZ;
        }
        private double LocateFault(TInstance.TNODE.TLINE Line, out string Tp, out double R)
        {
            if (Line.I3F.Count > 3 && Line.DE.V3F.Count > 3)
            {
                List<TCurve> Curves = new List<TCurve>();
                Complex[] Zn = new Complex[Line.I3F[0].BaseComtrade.endsamp[0]];
                TPhasor
                Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU),
                Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU),
                k0I0 = Line.I3F.Sequence[0].Reader(TPhasor.TReaderMode.PU) * Line.K0;
                TCurve.TfZ fab = delegate(ulong n)
                {
                    if ((Va[n] - Vb[n]).Magnitude > 0.05)
                    {
                        return (Va[n] - Vb[n]) / (Ia[n] - Ib[n]);
                    }
                    else
                    {
                        return 1 / Complex.Zero;
                    }
                },
                fbc = delegate(ulong n)
                {
                    if ((Vb[n] - Vc[n]).Magnitude > 0.05)
                    {
                        return (Vb[n] - Vc[n]) / (Ib[n] - Ic[n]);
                    }
                    else
                    {
                        return 1 / Complex.Zero;
                    }
                },
                fca = delegate(ulong n)
                {
                    if ((Vc[n] - Va[n]).Magnitude > 0.05)
                    {
                        return (Vc[n] - Va[n]) / (Ic[n] - Ia[n]);
                    }
                    else
                    {
                        return 1 / Complex.Zero;
                    }
                };
                Curves.Add(new TCurve("zAB", Line, fab));
                Curves.Add(new TCurve("zBC", Line, fbc));
                Curves.Add(new TCurve("zCA", Line, fca));
                Curves.Add(new TCurve("zAn", Line, delegate(ulong n)
                {

                    if ((Va[n]).Magnitude > 0.05)
                    {
                        return Va[n] / (Ia[n] + k0I0[n]);
                    }
                    else
                    {
                        return 1 / Complex.Zero;
                    }
                }));
                Curves.Add(new TCurve("zBn", Line, delegate(ulong n)
                {
                    if ((Vb[n]).Magnitude > 0.05)
                    {
                        return Vb[n] / (Ib[n] + k0I0[n]);
                    }
                    else
                    {
                        return 1 / Complex.Zero;
                    }
                }));
                Curves.Add(new TCurve("zCn", Line, delegate(ulong n)
                {
                    if ((Vc[n]).Magnitude > 0.05)
                    {
                        return Vc[n] / (Ic[n] + k0I0[n]);
                    }
                    else
                    {
                        return 1 / Complex.Zero;
                    }
                }));
                R = double.MaxValue;
                ulong n2 = 0;
                foreach (TCurve Curve in Curves)
                {
                    if (R > Curve.Min)
                    {
                        n2 = Curve.n;
                        R = Curve.Min;
                    }
                }
                return Takagi.Calcular(Line, n2, out Tp);
            }
            else
            {
                Tp = "";
                R = 0;
                return double.MaxValue;
            }
        }
        private TInstance.TNODE.TLINE FindFault(TInstance Instance, out double P, out string Tp)
        {
            double R, R2 = double.MaxValue;
            Tp = "";
            P = double.MaxValue;
            TInstance.TNODE.TLINE Line = null;
            foreach (TInstance.TNODE.TLINE Linha in Instance.LLine_Using)
            {
                double ft = LocateFault(Linha, out Tp, out R);
                if (R < R2 & ft > 0)
                {
                    Line = Linha;
                    R2 = R;
                    P = ft;
                }
            }
            return Line;
        }
        private void SurveyButton_Click(object sender, EventArgs e)
        {
            if (SurveyButton.Text == "Survey")
            {
                SurveyButton.Text = "Stop Survey";
                foreach (TMyListViewItem Item in FileList.Items)
                {
                    FileList.Focus();
                    if (!Item.Computed)
                    {
                        IEEEComtrade.Zic vZic = new IEEEComtrade.Zic(Item.File.FullName);
                        if (vZic.cfg != null & vZic.dat != null & vZic.inf != null)
                        {
                            Application.DoEvents();
                            IEEEComtrade.TComtrade Comtrade = new IEEEComtrade.TComtrade(vZic.cfg, vZic.dat);
                            TInstance Instance = new TInstance();
                            if (Instance.LoadReasonFile(Comtrade, vZic.inf))
                            {
                                double P = 0, R = 0;
                                string Tp = "";
                                TInstance.TNODE.TLINE Linha = null;
                                Item.SubItems.Add(Comtrade.station_name);
                                Item.SubItems.Add(Comtrade.start_time.ToShortDateString());
                                Item.SubItems.Add(Comtrade.start_time.ToShortTimeString());
                                if (vZic.hdr != null)
                                {
                                    Linha = GetFaultRecord(Instance, vZic.hdr, out P, out Tp);
                                }
                                if (Linha != null)
                                {
                                    Item.SubItems.Add(Linha.Name);
                                    Item.SubItems.Add(Linha.L.ToString());
                                    Item.SubItems.Add((100 * P / Linha.L).ToString());
                                    Item.SubItems.Add(Tp);
                                    Item.SubItems.Add((100 * LocateFault(Linha, out Tp, out R)).ToString());
                                    Item.SubItems.Add(Tp);
                                }
                                else
                                {
                                    Linha = FindFault(Instance, out P, out Tp);
                                    if (Linha != null && P < 2)
                                    {
                                        Item.SubItems.Add(Linha.Name);
                                        Item.SubItems.Add(Linha.L.ToString());
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add((P * 100).ToString());
                                        Item.SubItems.Add(Tp.ToString());
                                    }
                                    else
                                    {
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add("?");
                                        Item.SubItems.Add("?");
                                    }
                                }
                                FileList.EnsureVisible(Item.Index);
                            }
                        }
                        else
                        {
                            Item.SubItems.Add("Problema de compatibilidade do SoftWare com o Arquivo");
                            Item.SubItems.Add("?");
                            Item.SubItems.Add("?");
                            Item.SubItems.Add("?");
                            Item.SubItems.Add("?");
                            Item.SubItems.Add("?");
                        }
                        vZic.close();
                        Item.Computed = true;
                    }
                    if (StopSurvey)
                    {
                        break;
                    }
                }
                StopSurvey = false;
                SurveyButton.Text = "Survey";
            }
            else
            {
                StopSurvey = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog MySaveFileDialog = new SaveFileDialog();
            MySaveFileDialog.AddExtension = true;
            MySaveFileDialog.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
            if (MySaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Workbook WBook = new Workbook();
                FileInfo F = new FileInfo(MySaveFileDialog.FileName);
                Worksheet WSheet = new Worksheet(F.Name);
                WBook.Worksheets.Add(WSheet);
                int Row=0;
                int Col=0;
                WSheet.Cells[Row, Col]=new Cell("Path");
                foreach (ColumnHeader Item in FileList.Columns)
                {
                    Col++;
                    WSheet.Cells[Row, Col]=new Cell(Item.Text);
                }
                Row++;
                foreach (TMyListViewItem Item in FileList.Items)
                {
                    Col=0;
                    WSheet.Cells[Row, Col]=new Cell(Item.File.FullName);
                    foreach (ListViewItem.ListViewSubItem sb in Item.SubItems)
                    {
                        Col++;
                        WSheet.Cells[Row, Col] = new Cell(sb.Text);
                    }
                    FileList.EnsureVisible(Item.Index);
                    Row++;
                }
                WBook.Save(F.FullName);
            }
        }
        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (FileList.SelectedItems.Count > 0)
            {
                if (OpenFile != null)
                {
                    TMyListViewItem Item = (TMyListViewItem)FileList.SelectedItems[0];
                    OpenFile(Item.File.FullName);
                }
                //TMyListViewItem Item =(TMyListViewItem )FileList.SelectedItems[0];
                //FileStream F = File.Open(PowerSystem.Config.AnaFile, FileMode.Open);
                //PowerSystem.NetWork.ANA AnaObj = new PowerSystem.NetWork.ANA(F);
                //F.Close();
                //PowerSystem.Forms.MainForm MyForm = new PowerSystem.Forms.MainForm();
                //MyForm.OpenFile(Item.File.FullName, AnaObj);
                //MyForm.ShowDialog(this);
            }
        }
    }
}
