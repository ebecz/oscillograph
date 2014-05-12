using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

using PowerSystem.IEEEComtrade;

namespace PowerSystem.Methods.ImpedanceView
{
    public class TImpedance : TMethod<TInstance.TNODE.TLINE>
    {
        public System.Windows.Forms.ToolStrip MenuTool
        {
            get
            {
                return null;
            }
        }
        public string Text
        {
            get
            {
                return "Show Impedance Chart";
            }
        }
        public string Description
        {
            get
            {
                return Text;
            }
        }
        private class TMyResult : TResult
        {
            TImpedanceView MyForm;
            public TMyResult(TImpedanceView MyForm)
            {
                this.MyForm = MyForm;
            }
            public void Show(Form ParentForm)
            {
                MyForm.Show(ParentForm);
            }
            public object Data
            {
                get
                {
                    return this;
                }
            }
        }
        public TResult Execute(TInstance.TNODE.TLINE Linha)
        {
            TImpedanceView F = new TImpedanceView(Linha);
            return new TMyResult(F);
        }
    }
}
