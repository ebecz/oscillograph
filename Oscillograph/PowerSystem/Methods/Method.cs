using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;

using PowerSystem;
using PowerSystem.IEEEComtrade;
using PowerSystem.NetWork;

namespace PowerSystem.Methods
{
    public static class TMethodManager
    {
        //Old
        public static TMethod<TInstance.TNODE.TLINE> FaultDescriptor;
        //Newer
        public static List<TMethod<TInstance.TNODE.TLINE>> Method_Line = new List<TMethod<TInstance.TNODE.TLINE>>();
        public static List<TMethod<TInstance.TNODE>> Method_Node=new List<TMethod<TInstance.TNODE>>();
        public static List<TMethod<TInstance>> Method_Instance=new List<TMethod<TInstance>>();
        public static TMethod<TInstance> Main;

        static TMethodManager()
        {
            Main = new Methods.WaveView.TWaveView();
            //FaultDescriptor = new Methods.FaultDescriptor();
            Method_Line.Add(new Methods.DjTime());
            Method_Line.Add(new Methods.PhasorView.TPhasorView());
            Method_Line.Add(new Methods.FaultLocation());
            Method_Line.Add(new Methods.ImpedanceView.TImpedance());
            Method_Line.Add(new Methods.OverCurrent.TOverCurrent());
            Method_Instance.Add(new Methods.NetWorkView.TNetWorkViewer());
        }
    }
    public interface TResult
    {
        void Show(Form ParentForm);
        object Data
        {
            get;
        }
    }
    public interface TMethod<T>
    {
        string Text
        {
            get;
        }
        string Description
        {
            get;
        }
        TResult Execute(T Argument);
        ToolStrip MenuTool
        {
            get;
        }
    }
}