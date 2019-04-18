using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

using PowerSystem.IEEEComtrade;
using PowerSystem.NetWork;
using PowerSystem;

using PowerSystem.Methods;

using PowerSystem.Reason;
using PowerSystem.Dialogs;

namespace PowerSystem.Forms
{
    public partial class MainForm : Form
    {
        private Dictionary<Form, TInstance> Instances = new Dictionary<Form, TInstance>();
        public MainForm()
        {
            InitializeComponent();
            StripContainer.SuspendLayout();
            StripContainer.Join(TMethodManager.Main.MenuTool, StripContainer.Controls[StripContainer.Controls.Count - 1].Right, 0);
            foreach (TMethod<TInstance.TNODE.TLINE> Method in TMethodManager.Method_Line)
            {
                if (Method.MenuTool != null)
                {
                    StripContainer.Join(Method.MenuTool, StripContainer.Controls[StripContainer.Controls.Count - 1].Right, 0);
                }
            }
            foreach (TMethod<TInstance> Method in TMethodManager.Method_Instance)
            {
                if (Method.MenuTool != null)
                {
                    StripContainer.Join(Method.MenuTool, StripContainer.Controls[StripContainer.Controls.Count - 1].Right, 0);
                }
            }
            StripContainer.ResumeLayout();
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            foreach (Tools.TTool Tool in Tools.TToolManager.LTool)
            {
                Tool.OpenFile = OpenFile;
                toolsMenu.DropDownItems.Add(Tool.MenuTool);
            }
            foreach (TMethod<TInstance> Method in TMethodManager.Method_Instance)
            {
                toolsMenu.DropDownItems.Add(Method.Description).Click += (object sender, EventArgs e) =>
                {
                    if(ActiveMdiChild!=null){
                        TInstance Instance;
                        Instances.TryGetValue(ActiveMdiChild, out Instance);
                        Method.Execute(Instance).Show(this);
                    }
                };
            }
        }
        private bool LoadComtrade(ANA ANAF, TComtrade Comtrade, Stream Inf = null)
        {
            bool b=false;
            foreach (TInstance Instance in Instances.Values)
            {
                if (b = Instance.Load(Comtrade, Inf))
                {
                    break;
                }
            }
            if (!b)
            {
                TInstance Instance = new TInstance(ANAF, Comtrade.start_time);
                if (Instance.Load(Comtrade, Inf))
                {
                    TResult Result=Methods.TMethodManager.Main.Execute(Instance);
                    Result.Show(this);
                    Instances.Add((Form)Result.Data, Instance);
                    ((Form)Result.Data).FormClosed += (object sender, FormClosedEventArgs e) =>
                    {
                        Form F = (Form)sender;
                        Instances.Remove(F);
                    };
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private void ImportFile(object sender, EventArgs e)
        {
            if (Config.AnaFile != string.Empty)
            {
                FileStream F = File.Open(Config.AnaFile, FileMode.Open);
                ANA ANAF;
                try
                {
                    ANAF = new ANA(F);
                }
                catch (Exception x)
                {
                    MessageBox.Show("Houve problemas com o arquivo de topologia do AnaFas, por favor selecione um novo arquivo");
                    Config.AnaFile = string.Empty;
                    ImportFile(sender, e);
                    return;
                }
                F.Close();

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Comtrade Files (*.cfg;*.zic)|*.cfg;*.zic| Compacted Comtrade Files (*.zic)|*.zic|Comtrade Files (*.cfg)|*.cfg";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        OpenFile(fileName, ANAF);
                    }
                    if (Instances.Count > 1)
                    {
                        this.LayoutMdi(MdiLayout.TileHorizontal);
                    }
                }
            }
        }
        public bool OpenFile(string fileName)
        {
            FileStream F = File.Open(Config.AnaFile, FileMode.Open);
            ANA ObjAna = new NetWork.ANA(F);
            F.Close();
            return OpenFile(fileName, ObjAna);
        }
        public bool OpenFile(string fileName, ANA ObjAna=null)
        {
            switch (fileName.ToLower().Substring(fileName.Length - 3))
            {
                case "cfg":
                    FileInfo Fcfg = new FileInfo(fileName);
                    FileInfo Fdat = new FileInfo(fileName.Substring(0, fileName.Length - 3) + "dat");
                    FileInfo Finf = new FileInfo(fileName.Substring(0, fileName.Length - 3) + "inf");
                    if (Fcfg.Exists & Fdat.Exists)
                    {
                        try
                        {
                            TComtrade Comtrade;
                            Comtrade = new TComtrade(Fcfg.OpenRead(), Fdat.OpenRead());
                            if (Finf.Exists)
                            {
                                return LoadComtrade(ObjAna, Comtrade, Finf.OpenRead());
                            }
                            else
                            {
                                return LoadComtrade(ObjAna, Comtrade);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Faltam arquivos");
                    }
                    break;
                case "zic":
                    try
                    {
                        Zic CompactedFile = new Zic(fileName);
                        if (CompactedFile.dat != null && CompactedFile.cfg != null)
                        {
                            TComtrade Comtrade;
                            Comtrade = new TComtrade(CompactedFile.cfg, CompactedFile.dat);
                            return LoadComtrade(ObjAna, Comtrade, CompactedFile.inf);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;
            }
            return false;
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild!=null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = "Comtrade Files (*.cfg)|*.cfg|Comtrade Compacted File (*.zic)|*.zic";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string FileName = saveFileDialog.FileName;
                    switch (saveFileDialog.FileName.Substring(saveFileDialog.FileName.Length-3).ToLower())
                    {
                        case "cfg":
                            Stream CFG = new FileStream(FileName, FileMode.Create);
                            Stream DAT = new FileStream(FileName.Substring(0, FileName.Length - 3) + "dat", FileMode.Create);
                            throw (new Exception("Code não implementado"));
                            //Active.cComtrade.Save(CFG, DAT);
                            CFG.Close();
                            DAT.Close();
                            break;
                        case "zic":
                            if (ActiveMdiChild != null)
                            {
                                throw (new Exception("Code não implementado"));
                                TInstance Instance;
                                if (Instances.TryGetValue(ActiveMdiChild, out Instance))
                                {
                                    //Criar algoritmo para recuperar comtrade distribuido
                                    Stream CFG2 = new MemoryStream();
                                    Stream DAT2 = new MemoryStream();
                                    //childForm.GroupComponent.LComtrade[0].Write(CFG2, DAT2);
                                    Zic CompactedFile = new Zic(CFG2, DAT2);
                                    //CompactedFile.SaveAs(saveFileDialog.OpenFile(), childForm.GroupComponent.LComtrade[0].rec_dev_id);
                                    CFG2.Close();
                                    DAT2.Close();
                                }
                            }
                            break;
                    };
                }
            }
        }
        #region Funções terminadas
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }      
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }
        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }
        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }
        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }
        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }
        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }
        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
        #endregion
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.ConfigForm Cfg = new Forms.ConfigForm();
            if (Cfg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Config.Commit();
            }
            else
            {
                Config.Reload();
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox AboutBox = new AboutBox();
            AboutBox.ShowDialog(this);
        }
    }
}
