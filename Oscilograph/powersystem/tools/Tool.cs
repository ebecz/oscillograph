using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerSystem.Tools
{
    public static class TToolManager
    {
        public static List<TTool> LTool = new List<TTool>();
        static TToolManager()
        {
            LTool.Add(new Tools.Comtrade_Snooper.TComtrade_Snooper());
        }
    }
    public delegate bool TOpenFile(string FileName);
    public interface TTool
    {
        string Text
        {
            get;
        }
        string Description
        {
            get;
        }
        ToolStripMenuItem MenuTool
        {
            get;
        }
        TOpenFile OpenFile
        {
            set;
        }
    }
}
