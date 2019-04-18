using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSystem.Methods.OverCurrent
{
    class TOverCurrent : TMethod<TInstance.TNODE.TLINE>
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
                return "Over Current Analisys";
            }
        }
        public string Description
        {
            get
            {
                return "Over Current Analisys";
            }
        }
        private class TMyResult : TResult
        {
            private TOverCurrentForm MyForm;
            public TMyResult(TOverCurrentForm Form)
            {
                MyForm = Form;
            }
            public void Show(System.Windows.Forms.Form ParentForm)
            {
                MyForm.Show(ParentForm);
            }
            public object Data
            {
                get
                {
                    return MyForm;
                }
            }
        }
        public TResult Execute(TInstance.TNODE.TLINE Linha)
        {
            TOverCurrentForm Form = new TOverCurrentForm();
            Form.Line = Linha;
            return new TMyResult(Form);
        }
    }
}
