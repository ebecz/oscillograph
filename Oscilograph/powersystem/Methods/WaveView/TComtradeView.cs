using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PowerSystem.IEEEComtrade;
using PowerSystem.NetWork;

namespace PowerSystem.WaveView
{
    public class TWaveViewForm : Form
    {
        private SplitContainer Split;
        internal TGrafico Grafico;
        internal TLegenda Legenda;
        //public void Load(TInstance.TBARRA.TLINHA Linha)
        //{
            
        //}
        public TWaveViewForm(TInstance Instance)
        {
            this.WindowState = FormWindowState.Maximized;
            Split = new SplitContainer();
            Split.Dock = DockStyle.Fill;
            this.Controls.Add(Split);

            Legenda = new TLegenda(Grafico = new TGrafico(Instance));

            Legenda.Dock = DockStyle.Fill;
            Legenda.GotFocus += new EventHandler(SetFocusOnRightControl);
            GotFocus += new EventHandler(SetFocusOnRightControl);
            Split.SplitterDistance = (int)(16 * Font.Height + Font.Height * 2);
            Split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.Font = new Font(FontFamily.GenericMonospace, Font.Size - 1,FontStyle.Bold);
            Legenda.Font = this.Font;
            Grafico.Font = this.Font;

            Grafico.ActiveCursor = Instance.Cursor;

            Split.Panel2.Controls.Add(Grafico);
            Split.Panel1.Controls.Add(Legenda);
        }
        void SetFocusOnRightControl(object sender, EventArgs e)
        {
            Grafico.Focus();
        }
    }
}
