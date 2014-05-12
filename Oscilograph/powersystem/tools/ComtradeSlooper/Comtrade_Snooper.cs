using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerSystem.Tools.Comtrade_Snooper
{
    public class TComtrade_Snooper : TTool
    {
        public string Text
        {
            get
            {
                return "Execute Comtrade Snooper";
            }
        }
        public string Description
        {
            get
            {
                return "Execute Comtrade Snooper";
            }
        }
        public ToolStripMenuItem MenuTool
        {
            get
            {
                ToolStripMenuItem Menu=new ToolStripMenuItem(Text);
                Menu.Click += (object sender, EventArgs e) =>
                {
                    TMyForm MyForm = new TMyForm();
                    MyForm.OpenFile = OpenFile;
                    MyForm.Show();
                };
                return Menu;
            }
        }
        Tools.TOpenFile _OpenFile;
        public Tools.TOpenFile OpenFile
        {
            set
            {
                _OpenFile=value;
            }
            get{
                return _OpenFile;
            }
        }
    }
}
