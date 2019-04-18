using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerSystem.Methods.NetWorkView
{
    class TNetWorkViewer : TMethod<TInstance>
    {
        public System.Windows.Forms.ToolStrip MenuTool
        {
            get
            {
                System.Windows.Forms.ToolStrip Menu=new System.Windows.Forms.ToolStrip();
                Menu.Items.Add("See NetWork").Click+=(object sender, EventArgs e)=>{
                    MessageBox.Show("Ainda não implementado");
                };
                return Menu;
            }
        }
        public string Text
        {
            get
            {
                return "NetWork Visualizer";
            }
        }
        public string Description
        {
            get
            {
                return "NetWork Visualizer";
            }
        }
        private class TNetViewForm : System.Windows.Forms.Form
        {
            private NetView MyNetView;
            public TNetViewForm(TInstance Instance)
            {
                MyNetView = new NetView();
                foreach (TInstance.TNODE.TLINE Line in Instance.LLine_Using)
                {
                    NetView.NodeControl NodeDE = new NetView.NodeControl(Line.DE);
                    NodeDE.Top = 10;
                    NodeDE.Left = 100 + 500 * MyNetView.Nodes.Systens.Count;
                    MyNetView.Nodes.Add(NodeDE);

                    NetView.NodeControl NodePara = new NetView.NodeControl(Line.PARA);
                    MyNetView.Nodes.Add(NodePara);
                }
                MyNetView.Parent = this;
                MyNetView.Paint += (object sender, PaintEventArgs e) =>
                {
                    if (b)
                    {
                        MyNetView.Auto();
                        b = false;
                    }
                };
                MyNetView.Dock = DockStyle.Fill;
            }
            bool b = true;
        }
        private class TMyResult : TResult
        {
            private TNetViewForm MyForm;
            public TMyResult(TNetViewForm Form)
            {
                MyForm = Form;
            }
            public object Data
            {
                get
                {
                    return MyForm;
                }
            }
            public void Show(System.Windows.Forms.Form ParentForm)
            {
                MessageBox.Show("Ctrl + Click = Arrastar Barra" + Environment.NewLine +
                    "f + click = Rotacionar Barra" + Environment.NewLine +
                    "Botão do meio = Arrastar Tela" + Environment.NewLine +
                    "Scroll = Zoom" + Environment.NewLine +
                    "Botões Direito + Esquerdo do Mouse = Arrastar tela também" + Environment.NewLine +
                    "Manter o mouse sobre Barra ou Linha Selecionada mostra propriedades");
                MyForm.Show(ParentForm);
            }
        }
        public TResult Execute(TInstance Instance)
        {
            TNetViewForm Form = new TNetViewForm(Instance);
            return new TMyResult(Form);
        }
    }
}
