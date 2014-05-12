using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PowerSystem.NetWork;
using PowerSystem.IEEEComtrade;
using PowerSystem.Reason;

namespace PowerSystem.Dialogs
{
    public partial class INFEditor : Form
    {
        List<TInstance.TNODE> LBARRA;
        List<TInstance.TNODE> LSearch;
        private Dictionary<float, LTBARRAFilter> VDict = new Dictionary<float, LTBARRAFilter>();
        class LTBARRAFilter
        {
            string Text;
            public List<TInstance.TNODE> LB = new List<TInstance.TNODE>();
            public LTBARRAFilter(string Text)
            {
                this.Text = Text;
            }
            public override string ToString()
            {
                return Text;
            }
        }
        TInstance Instance;
        public INFEditor(List<TInstance.TNODE> LBARRA, T3F[] I, T3F[] V, IEEEComtrade.TComtrade.DChannel[] D,TInstance Instance)
        {
            this.Instance = Instance;
            InitializeComponent();
            LBVoltage.DisplayMember = "ccbm";
            LBVoltage.Items.AddRange(V);
            LBCurrent.DisplayMember = "ccbm";
            LBCurrent.Items.AddRange(I);
            LBDigital.DisplayMember = "ch_id";
            LBDigital.Items.AddRange(D);
            LSearch = this.LBARRA = LBARRA;
            foreach (TInstance.TNODE Barra in LBARRA)
            {
                LTBARRAFilter LFBarras;
                if (!VDict.TryGetValue(Barra.VBASE, out LFBarras))
                {
                    LFBarras = new LTBARRAFilter(Barra.VBASE.ToString());
                    CBVoltage.Items.Add(LFBarras);
                    VDict.Add(Barra.VBASE, LFBarras);
                }
                LFBarras.LB.Add(Barra);
            }
            LBNode.Columns.Add("Name");
            LBNode.Columns.Add("Voltage");
            LBNode.Columns.Add("Zone");
            LBNode.Columns.Add("Classification");

            LBLine.Columns.Add("Name");
            LBLine.Columns.Add("From");
            LBLine.Columns.Add("To");
            LBLine.Columns.Add("Z1");
            LBLine.Columns.Add("Z0");
            LBLine.Columns.Add("Nome Extenso");
            LBLine.Columns.Add("Tipo");

            netView1.X = netView1.Width / 2;
            netView1.Y = netView1.Height / 2;
        }
        private void LBNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LBNode.FocusedItem != null)
            {
                LBLine.Items.Clear();
                TInstance.TNODE Barra = (TInstance.TNODE)((MyListViewItem)LBNode.FocusedItem).Object;
                foreach (TInstance.TNODE.TLINE Linha in Barra.LINES)
                {
                    LBLine.Items.Add(new MyListViewItem(Linha));
                }
            }
        }
        class MyListViewItem : ListViewItem
        {
            public object Object;
            public MyListViewItem(TInstance.TNODE Barra):base(Barra.Name,1)
            {
                this.Object = Barra;
                base.Name = Barra.Name;
                //base.SubItems.Add(Barra.Name);
                base.SubItems.Add(Barra.VBASE.ToString());
                base.SubItems.Add(Barra.BAR.SubSistema.ToString());
                switch(Barra.BAR.mp){
                    case NetWork.ANA.BAR.MP.Derivation:
                        base.SubItems.Add("Derivation");break;
                    case NetWork.ANA.BAR.MP.MidPoint:
                        base.SubItems.Add("MidPoint");break;
                    case NetWork.ANA.BAR.MP.Normal:
                        base.SubItems.Add("Normal");break;
                }
            }
            public MyListViewItem(TInstance.TNODE.TLINE Linha)
                : base(Linha.Name, 1)
            {
                this.Object = Linha;
                base.Name = Linha.Name;
                base.SubItems.Add(Linha.DE.Name);
                if (Linha.PARA != null)
                {
                    base.SubItems.Add(Linha.PARA.Name);
                }
                else
                {
                    base.SubItems.Add("NULL");
                }
                base.SubItems.Add(Linha.Z1.ToString("F2"));
                base.SubItems.Add(Linha.Z0.ToString("F2"));
                base.SubItems.Add(Linha.CIR.Nome);
                switch(Linha.CIR.tipc){
                    case NetWork.ANA.CIR.TIPC.C:
                        base.SubItems.Add("Load");break;
                    case NetWork.ANA.CIR.TIPC.G:
                        base.SubItems.Add("Generator"); break;
                    case NetWork.ANA.CIR.TIPC.H:
                        base.SubItems.Add("Shunt"); break;
                    case NetWork.ANA.CIR.TIPC.L:
                        base.SubItems.Add("Line"); break;
                    case NetWork.ANA.CIR.TIPC.S:
                        base.SubItems.Add("Serie Capacitor"); break;
                    case NetWork.ANA.CIR.TIPC.T:
                        base.SubItems.Add("Transformer"); break;
                    case NetWork.ANA.CIR.TIPC.Z:
                        base.SubItems.Add("Grounding Transformer"); break;
                }
            }
        }
        private void TxNode_TextChanged(object sender, EventArgs e)
        {
            int n = 0;
            LBNode.Items.Clear();
            foreach (TInstance.TNODE Barra in LSearch)
            {
                if (Barra.Name.ToLower().IndexOf(TxNode.Text.ToLower()) != -1)
                {
                    LBNode.Items.Add(new MyListViewItem(Barra));
                    if (++n > 100)
                    {
                        break;
                    }
                }
            }
        }
        //private void Assert_Click(object sender, EventArgs e)
        //{
        //    T3F V = (T3F)LBVoltage.SelectedItem;
        //    T3F I = (T3F)LBCurrent.SelectedItem;
        //    if (LBNode.FocusedItem != null & V != null)
        //    {
        //        TInstance.TNODE Barra = (TInstance.TNODE)(((MyListViewItem)LBNode.FocusedItem).Object);
        //        Barra.V3F = V;
        //        if (!LOutBarras.Contains(Barra))
        //        {
        //            LOutBarras.Add(Barra);
        //        }
        //    }
        //    if (LBLine.FocusedItem != null && I != null)
        //    {
        //        TInstance.TNODE.TLINE Linha = (TInstance.TNODE.TLINE)(((MyListViewItem)LBLine.FocusedItem).Object);
        //        Linha.I3F = I;
        //        if (!LLines.Contains(Linha))
        //        {
        //            LLines.Add(Linha);
        //        }
        //        foreach (IEEEComtrade.TComtrade.DChannel DCh in LBDigital.SelectedItems)
        //        {
        //            Linha.D.Add(DCh);
        //        }
        //        foreach (IEEEComtrade.TComtrade.DChannel DCh in Linha.D)
        //        {
        //            LBDigital.Items.Remove(DCh);
        //        }
        //    }
        //}
        private void CBVoltage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBVoltage.SelectedIndex == -1)
            {
                LSearch = LBARRA;
            }
            else
            {
                LSearch = ((LTBARRAFilter)CBVoltage.SelectedItem).LB;
            }
            TxNode_TextChanged(sender, e);
        }
        private void LBNode_DoubleClick(object sender, EventArgs e)
        {
            netView1.Nodes.Add(new Methods.NetWorkView.NetView.NodeControl((TInstance.TNODE)((MyListViewItem)LBNode.SelectedItems[0]).Object));
            netView1.Invalidate();
        }
        private void List_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox List = sender as ListBox;
            if (List.Items.Count == 0)
                return;
            int index = List.IndexFromPoint(e.X, e.Y);
            if (index != -1)
            {
                DragDropEffects dde1 = DoDragDrop(List.Items[index], DragDropEffects.All);
                if (dde1 == DragDropEffects.All)
                {
                    List.Items.RemoveAt(List.IndexFromPoint(e.X, e.Y));
                }
            }
        }
        private void DList_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox List = sender as ListBox;
            if (List.Items.Count == 0)
                return;
            List<IEEEComtrade.TComtrade.DChannel>DChannels=new List<IEEEComtrade.TComtrade.DChannel>();
            foreach(int i in List.SelectedIndices)
            {
                DChannels.Add(List.Items[i] as IEEEComtrade.TComtrade.DChannel);
            }
            DragDropEffects dde1 = DoDragDrop(DChannels, DragDropEffects.All);
            if (dde1 == DragDropEffects.All)
            {
                foreach (IEEEComtrade.TComtrade.DChannel DChannel in DChannels)
                {
                    List.Items.Remove(DChannel);
                }
            }
        }
        private void netView1_DragOver(object sender, DragEventArgs e)
        {
            Point p2 = netView1.PointToClient(new Point(e.X, e.Y));
            if (e.Data.GetDataPresent(typeof(T3F)))
            {
                T3F F = e.Data.GetData(typeof(T3F)) as T3F;
                switch (F[0].uu.uu)
                {
                    case IEEEComtrade.TComtrade.AChannel.Tuu.Tu.V:
                        TInstance.TNODE Node = netView1.GetNodeAtPoint(p2);
                        if (Node != null)
                        {
                            e.Effect = DragDropEffects.All;
                        }
                        else
                        {
                            e.Effect = DragDropEffects.None;
                        }
                        break;
                    case IEEEComtrade.TComtrade.AChannel.Tuu.Tu.A:
                        TInstance.TNODE.TLINE Line = netView1.GetConnectionAtPoint(p2);
                        if (Line != null)
                        {
                            e.Effect = DragDropEffects.All;
                        }
                        else
                        {
                            e.Effect = DragDropEffects.None;
                        }
                        break;
                    default:
                        e.Effect = DragDropEffects.None;
                        break;
                }
            }
            else if (e.Data.GetDataPresent(typeof(List<IEEEComtrade.TComtrade.DChannel>)))
            {
                TInstance.TNODE.TLINE Line = netView1.GetConnectionAtPoint(p2);
                if (Line != null)
                {
                    e.Effect = DragDropEffects.All;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }
        private void netView1_DragDrop(object sender, DragEventArgs e)
        {
            Point p2 = netView1.PointToClient(new Point(e.X, e.Y));
            if (e.Data.GetDataPresent(typeof(T3F)))
            {
                T3F F = e.Data.GetData(typeof(T3F)) as T3F;
                switch (F[0].uu.uu)
                {
                    case IEEEComtrade.TComtrade.AChannel.Tuu.Tu.V:
                        TInstance.TNODE Node = netView1.GetNodeAtPoint(p2);
                        Node.V3F = e.Data.GetData(typeof(T3F)) as T3F;
                        Instance.LNode_Using.Add(Node);
                        break;
                    case IEEEComtrade.TComtrade.AChannel.Tuu.Tu.A:
                        TInstance.TNODE.TLINE Line = netView1.GetConnectionAtPoint(p2);
                        Line.I3F = e.Data.GetData(typeof(T3F)) as T3F;
                        Instance.LLine_Using.Add(Line);
                        break;
                }
            }
            else if (e.Data.GetDataPresent(typeof(List<IEEEComtrade.TComtrade.DChannel>)))
            {
                TInstance.TNODE.TLINE Line = netView1.GetConnectionAtPoint(p2);
                Line.D.AddRange((e.Data.GetData(typeof(List<IEEEComtrade.TComtrade.DChannel>)) as List<IEEEComtrade.TComtrade.DChannel>).ToArray());
                if (!Instance.LLine_Using.Contains(Line))
                {
                    Instance.LLine_Using.Add(Line);
                }
            }
        }
        private void LBLine_DoubleClick(object sender, EventArgs e)
        {
            netView1.Nodes.Add(new Methods.NetWorkView.NetView.NodeControl(((TInstance.TNODE.TLINE)((MyListViewItem)LBLine.SelectedItems[0]).Object).PARA));
            netView1.Invalidate();
        }
        private void Salve_Click(object sender, EventArgs e)
        {
            Instance.Save();
        }
    }
    public static class Extended
    {
        private static DialogResult LoadINFEditor(this TInstance Instance, TComtrade Comtrade)
        {
            Dictionary<string, T3F> VDict = new Dictionary<string, T3F>();
            Dictionary<string, T3F> ADict = new Dictionary<string, T3F>();

            foreach (TComtrade.AChannel Item in Comtrade.AnalogChannels)
            {
                if (Item.uu.uu == TComtrade.AChannel.Tuu.Tu.V)
                {
                    T3F V3F;
                    if (!VDict.TryGetValue(Item.ccbm, out V3F))
                    {
                        V3F = new T3F();
                        VDict.Add(Item.ccbm, V3F);
                    }
                    V3F.Add(Item);
                }
                if (Item.uu.uu == TComtrade.AChannel.Tuu.Tu.A)
                {
                    T3F I3F;
                    if (!ADict.TryGetValue(Item.ccbm, out I3F))
                    {
                        I3F = new T3F();
                        ADict.Add(Item.ccbm, I3F);
                    }
                    I3F.Add(Item);
                }
            }
            foreach(T3F V3F in VDict.Values){
                if (V3F.Sequence != null)
                {
                    Comtrade.Reference = V3F.Sequence[1].PhaseReference;
                    break;
                }
            }
            Dialogs.INFEditor Editor = new Dialogs.INFEditor(Instance.LNode, ADict.Values.ToArray(), VDict.Values.ToArray(), Comtrade.DigitalChannels.ToArray(), Instance);
            Editor.Text = Editor.Text + " - " + Comtrade.station_name + " - " + Comtrade.rec_dev_id;
            return Editor.ShowDialog();
        }
        public static bool Save(this TInstance Instance)
        {
            Dictionary<TComtrade,IniParser>Dict=new Dictionary<TComtrade,IniParser>();
            Dictionary<IniParser, List<string>> Lines = new Dictionary<IniParser, List<string>>();
            foreach (TInstance.TNODE.TLINE Line in Instance.LLine_Using)
            {
                IniParser InfParser;
                                string SectionName = "Oscillograph " + Line.Name;
                if (!Dict.TryGetValue(Line.I3F.Comtrade, out InfParser))
                {
                    string Folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + @"\Oscillograph\";
                    FileInfo INF = new FileInfo(Folder + Line.I3F.Comtrade.rec_dev_id + ".inf");
                    INF.Create().Close();
                    InfParser=new IniParser(INF.FullName);
                    InfParser.AddSetting("Oscillograph Main", "rec_dev_id", Line.I3F.Comtrade.rec_dev_id);
                    InfParser.AddSetting("Oscillograph Main", "station_name", Line.I3F.Comtrade.station_name);
                    InfParser.AddSetting("Oscillograph Main", "Lines");
                    InfParser.AddSetting("Oscillograph Main", "Version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    Dict.Add(Line.I3F.Comtrade, InfParser);
                    Lines.Add(InfParser, new List<string>());
                }
                InfParser.AddSetting(SectionName, "De", Line.DE.BAR.NB.ToString());
                InfParser.AddSetting(SectionName, "Para", Line.PARA.BAR.NB.ToString());
                InfParser.AddSetting(SectionName, "Circuito", Line.CIR.NC.ToString());
                string[] Tensoes = new string[Line.DE.V3F.Count];
                for(int i=0;i<Line.DE.V3F.Count;i++)
                {
                    Tensoes[i] = Line.DE.V3F[i].n.ToString();
                }
                InfParser.AddSetting(SectionName, "Tensoes", string.Join(",", Tensoes));
                InfParser.AddSetting(SectionName, "V_ccbm", Line.DE.V3F.ccbm);
                string[] Correntes = new string[Line.I3F.Count];
                for (int i = 0; i < Line.I3F.Count; i++)
                {
                    Correntes[i] = Line.I3F[i].n.ToString();
                }
                InfParser.AddSetting(SectionName, "Correntes", string.Join(",", Correntes));
                InfParser.AddSetting(SectionName, "I_ccbm", Line.I3F.ccbm);
                Lines[InfParser].Add(Line.I3F.ccbm);
                string[] Digitais = new string[Line.D.Count];
                for (int i = 0; i < Line.D.Count; i++)
                {
                    Digitais[i] = Line.D[i].n.ToString();
                }
                InfParser.AddSetting(SectionName, "Digitais", string.Join(",", Digitais));
            }
            foreach (IniParser InfParser in Dict.Values)
            {
                InfParser.AddSetting("Oscillograph Main", "Lines", string.Join(",", Lines[InfParser]));
                InfParser.SaveSettings();
            }
            return true;
        }
        public static bool Load(this TInstance Instance, TComtrade Comtrade, Stream EmbedInf)
        {
            if (!Instance.TimeIntersecting(Comtrade))
                return false;
            string Folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + @"\Oscillograph\";
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
            FileInfo F = new FileInfo(Folder + Comtrade.rec_dev_id + ".inf");
            if (!F.Exists)
            {
                switch (Instance.LoadINFEditor(Comtrade))
                {
                    case DialogResult.OK:
                        return true;
                    case DialogResult.Ignore:
                        return Instance.LoadReasonFile(Comtrade, EmbedInf);
                    case DialogResult.Cancel:
                        return false;
                }
            return false;
            }
            try
            {
                IniParser InfParser = new IniParser(F.FullName);
                if (InfParser.GetSetting("Oscillograph Main", "rec_dev_id") == Comtrade.rec_dev_id && InfParser.GetSetting("Oscillograph Main", "station_name") == Comtrade.station_name)
                {
                    if (InfParser.GetSetting("Oscillograph Main", "Version") == System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())
                    {
                        string[] Lines = InfParser.GetSetting("Oscillograph Main", "Lines").Split(',');
                        foreach (string sLine in Lines)
                        {
                            string SectionName = "Oscillograph " + sLine;
                            Int32 NB = Convert.ToInt32(InfParser.GetSetting(SectionName, "De"));
                            Int32 NF = Convert.ToInt32(InfParser.GetSetting(SectionName, "Para"));
                            Int16 NC = Convert.ToInt16(InfParser.GetSetting(SectionName, "Circuito"));
                            string Vccbm = InfParser.GetSetting(SectionName, "V_ccbm");
                            string Iccbm = InfParser.GetSetting(SectionName, "I_ccbm");
                            TInstance.TNODE Node = Instance.LNode.Find((TInstance.TNODE lNode) => lNode.BAR.NB == NB);
                            if (Node != null)
                            {
                                if (Node.V3F.Count == 0)
                                {
                                    string[] Tensoes = InfParser.GetSetting(SectionName, "Tensoes").Split(',');
                                    foreach (string Tensao in Tensoes)
                                    {
                                        Int16 n = Convert.ToInt16(Tensao);
                                        if (n < Comtrade.AnalogChannels.Length && n >= 0)
                                        {
                                            if (Comtrade.AnalogChannels[n - 1].ccbm.ToUpper() == Vccbm)
                                            {
                                                Node.V3F.Add(Comtrade.AnalogChannels[n - 1]);
                                                Comtrade.AnalogChannels[n - 1].Barra = Node;
                                            }
                                        }
                                    }
                                    if (Node.V3F.Count > 0)
                                    {
                                        Instance.LNode_Using.Add(Node);
                                        if (Comtrade.Reference == null)
                                        {
                                            if (Node.V3F.Sequence != null)
                                            {
                                                Comtrade.Reference = Node.V3F.Sequence[1].PhaseReference;
                                            }
                                        }
                                    }
                                }
                                TInstance.TNODE.TLINE Line = Node.LINES.Find((TInstance.TNODE.TLINE lLine) => lLine.PARA.BAR.NB == NF && lLine.CIR.NC == NC);
                                if (Line != null)
                                {
                                    Line.I3F = new T3F();
                                    string[] Correntes = InfParser.GetSetting(SectionName, "Correntes").Split(',');
                                    foreach (string Corrente in Correntes)
                                    {
                                        Int16 n = Convert.ToInt16(Corrente);
                                        if (n < Comtrade.AnalogChannels.Length && n >= 0)
                                        {
                                            if (Comtrade.AnalogChannels[n - 1].ccbm.ToUpper() == Iccbm)
                                            {
                                                Line.I3F.Add(Comtrade.AnalogChannels[n - 1]);
                                                Comtrade.AnalogChannels[n - 1].Barra = Node;
                                            }
                                        }
                                    }
                                    string[] Digitais = InfParser.GetSetting(SectionName, "Digitais").Split(',');
                                    foreach (string Digital in Digitais)
                                    {
                                        Int16 n = Convert.ToInt16(Digital);
                                        Line.D.Add(Comtrade.DigitalChannels[n - 1]);
                                    }
                                    Instance.LLine_Using.Add(Line);
                                }
                            }
                        }
                    }
                    else
                    {
                        F.Delete();
                        Load(Instance, Comtrade, EmbedInf);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool TimeIntersecting(this TInstance Instance, TComtrade Comtrade)
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
                return true;
            }
            return false;
        }
        //public static bool Load(this TInstance Instance, TComtrade Comtrade, Stream Inf)
        //{
        //    if (Instance.Time.Start > Instance.Time.End)
        //    {
        //        Instance.Time.Start = Comtrade.start_time;
        //        Instance.Time.End = Comtrade.end_time;
        //    }
        //    if (Comtrade.start_time <= Instance.Time.End && Comtrade.end_time >= Instance.Time.Start)
        //    {
        //        Instance.Time.Start = Comtrade.start_time < Instance.Time.Start ? Comtrade.start_time : Instance.Time.Start;
        //        Instance.Time.A = Instance.Time.Start > Instance.Time.A ? Instance.Time.Start : Instance.Time.A;
        //        Instance.Time.End = Comtrade.end_time > Instance.Time.End ? Comtrade.end_time : Instance.Time.End;
        //        Instance.Time.B = Instance.Time.End < Instance.Time.B ? Instance.Time.End : Instance.Time.B;

        //        Dictionary<string, T3F> VDict = new Dictionary<string, T3F>();
        //        Dictionary<string, T3F> ADict = new Dictionary<string, T3F>();

        //        foreach (TComtrade.AChannel Item in Comtrade.AnalogChannels)
        //        {
        //            if (Item.uu.uu == TComtrade.AChannel.Tuu.Tu.V)
        //            {
        //                T3F V3F;
        //                if (!VDict.TryGetValue(Item.ccbm, out V3F))
        //                {
        //                    V3F = new T3F();
        //                    VDict.Add(Item.ccbm, V3F);
        //                }
        //                V3F.Add(Item);
        //            }
        //            if (Item.uu.uu == TComtrade.AChannel.Tuu.Tu.A)
        //            {
        //                T3F I3F;
        //                if (!ADict.TryGetValue(Item.ccbm, out I3F))
        //                {
        //                    I3F = new T3F();
        //                    ADict.Add(Item.ccbm, I3F);
        //                }
        //                I3F.Add(Item);
        //            }
        //        }
        //        //Definindo referência fasorial como sendo uma das sequência positivas dos grupos de tensão
        //        foreach (T3F V3F in VDict.Values)
        //        {
        //            if (V3F.Sequence != null)
        //            {
        //                Comtrade.Reference = V3F.Sequence[1].PhaseReference;
        //                break;
        //            }
        //        }
        //        Dialogs.INFEditor Selectionator = new Dialogs.INFEditor(Instance.LNode, ADict.Values.ToArray(), VDict.Values.ToArray(), Comtrade.DigitalChannels.ToArray());
        //        Selectionator.Text = Selectionator.Text + " - " + Comtrade.station_name + " - " + Comtrade.rec_dev_id;
        //        Selectionator.InFile.Enabled = (Inf != null);
        //        switch (Selectionator.ShowDialog())
        //        {
        //            case System.Windows.Forms.DialogResult.OK:
        //                if (Selectionator.OutLines.Length == 0 || Selectionator.Barras.Length == 0)
        //                {
        //                    return false;
        //                }
        //                Instance.LLine_Using.AddRange(Selectionator.OutLines);
        //                Instance.LNode_Using.AddRange(Selectionator.Barras);
        //                return true;
        //            case System.Windows.Forms.DialogResult.Ignore:
        //                return Instance.LoadReasonFile(Comtrade, Inf);
        //            case System.Windows.Forms.DialogResult.Abort: //Quando foi clicado em Automático
        //                string Sthis = Comtrade.rec_dev_id.ToLower();
        //                IEnumerable<TInstance.TNODE> OBarrras = Instance.LNode.OrderBy(Barra => CMath.Levenshtein.Compute(Barra.Name.ToLower(), Sthis));
        //                //List<TInstance.TNODE.TLINHA> LLine_Using=new List<TInstance.TNODE.TLINHA>();
        //                //LLine_Using
        //                //List<TInstance.TNODE> LNode_Using = new List<TInstance.TNODE>();
        //                //LNode_Using
        //                foreach (T3F I3F in ADict.Values)
        //                {
        //                    string Sthat = clear(I3F[0].ccbm);
        //                    foreach (TInstance.TNODE Barra in OBarrras)
        //                    {
        //                        bool t = false;
        //                        IEnumerable<TInstance.TNODE.TLINE> OLinhas = Barra.LINES.OrderBy(Linha => CMath.Levenshtein.Compute(clear(Linha.PARA.Name), Sthat));
        //                        foreach (TInstance.TNODE.TLINE Linha in OLinhas)
        //                        {
        //                            if (Linha.CIR.tipc == ANA.CIR.TIPC.L && !Instance.LLine_Using.Contains(Linha))
        //                            {
        //                                Linha.I3F = I3F;
        //                                Instance.LLine_Using.Add(Linha);
        //                                IEnumerable<T3F> OV3F = VDict.Values.OrderBy(V3F => CMath.Levenshtein.Compute(V3F[0].ccbm, Linha.DE.VBASE.ToString("F0")));
        //                                Linha.DE.V3F = OV3F.First();
        //                                if (!Instance.LNode_Using.Contains(Linha.DE))
        //                                {
        //                                    Instance.LNode_Using.Add(Linha.DE);
        //                                }
        //                                t = true;
        //                                break;
        //                            }
        //                        }
        //                        if (t) break;
        //                    }
        //                }
        //                foreach (TComtrade.DChannel DCh in Comtrade.DigitalChannels)
        //                {
        //                    IEnumerable<TInstance.TNODE.TLINE> ODLinhas = Instance.LLine_Using.OrderBy(Linha => CMath.Levenshtein.Compute(Linha.Name, DCh.ch_id));
        //                    ODLinhas.First().D.Add(DCh);
        //                }
        //                return true;
        //                break;
        //            default:
        //                return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //private static float GetVoltageByString(string p)
        //{
        //    string pf = string.Empty;
        //    foreach (char c in p)
        //    {
        //        if (c >= '0' && c <= '9')
        //        {
        //            pf = pf + c;
        //        }
        //    }
        //    if (pf == "")
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return Convert.ToSingle(pf, System.Globalization.CultureInfo.InvariantCulture);
        //    }
        //}
        //private static string clear(string s)
        //{
        //    s = s.ToLower();
        //    //Remove ##_ {LT_, SE_, DJ_}
        //    if (s.IndexOf('_') >= 0)
        //        s = s.Substring(s.IndexOf('_') + 1);
        //    //Remove caracteres especiais
        //    string sC = "";
        //    foreach (char c in s)
        //    {
        //        if (c >= 'a' && c <= 'z' || c >= '0' && c <= '9')
        //        {
        //            sC = sC + c;
        //        }
        //    }
        //    return sC;
        //}
    }
}
