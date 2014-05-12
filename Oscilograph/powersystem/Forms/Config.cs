using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace PowerSystem.Forms
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            AnaFilePath.Text = Config.AnaFile;
            foreach (Control C in groupBoxColor.Controls)
            {
                if (C is Button)
                {
                    C.BackColor = Config.GetColorByPhase(C.Name);
                }
            }
            Zone.Text = Config.DefaultZone;
            ReloadZone();
        }
        private void OpenAnaFile_Click(object sender, EventArgs e)
        {
            string sPath = Config.pOpenAnaFile();
            AnaFilePath.Text = sPath;
        }
        private void bColor_Click(object sender, EventArgs e)
        {
            Button Butt = (Button)sender;
            ColorDialog D = new ColorDialog();
            D.Color = Butt.BackColor;
            if (D.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Config.SetColorByPhase(Butt.Name, D.Color);
                Butt.BackColor = D.Color;
            }
        }
        private void Zone_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.DefaultZone = Zone.Text;
        }
        private void ReloadZone(){
            Zone.Items.Clear();
            if (File.Exists(AnaFilePath.Text))
            {
                FileStream F = File.Open(AnaFilePath.Text, FileMode.Open);
                NetWork.ANA AnaObj = new NetWork.ANA(F);
                foreach (NetWork.ANA.DARE DARE in AnaObj.DARES)
                {
                    Zone.Items.Add(DARE.Nome);
                }
                F.Close();
            }
        }
        private void Apply_Click(object sender, EventArgs e)
        {
            Config.Commit();
            ReloadZone();
        }

        private void AnaFilePath_TextChanged(object sender, EventArgs e)
        {
            Config.AnaFile = AnaFilePath.Text;
        }
    }
}
