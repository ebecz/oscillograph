using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;
using PowerSystem.CMath;

namespace PowerSystem.Methods
{
    class FaultLocation : TMethod<TInstance.TNODE.TLINE>
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
                return Description;
            }
        }
        public string Description
        {
            get
            {
                return "Fault Location - Takagi's Modified Method";
            }
        }
        private class TMyResult:TResult
        {
            public double m;
            public FaultDescriptor.TMyResult Descriptor;    
            public void Show(Form ParentForm)
            {
                MessageBox.Show("Falta: " + (Descriptor.A ? "A" : "") + (Descriptor.B ? "B" : "") + (Descriptor.C ? "C" : "") + (Descriptor.N ? "N" : "") + ":" + (m * 100).ToString("F1") + "%");
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
            TMyResult Result = new TMyResult();
            Result.Descriptor = (Methods.FaultDescriptor.TMyResult)(new Methods.FaultDescriptor()).Execute(Linha);
            Result.m = Calcular(Linha, Linha.DE.Instance.Cursor, Result.Descriptor);
            return Result;
        }
        public double Calcular(TInstance.TNODE.TLINE Line, TTimeCursor D, FaultDescriptor.TMyResult Descriptor)
        {
            double R = 0;
            if (Descriptor.A && Descriptor.B && Descriptor.C)
            {
                Complex V1 = Line.DE.V3F.Sequence[1].Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex I1 = Line.I3F.Sequence[1].Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                R = (V1 / I1).Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            if (Descriptor.A && Descriptor.B && !Descriptor.C)
            {
                Complex Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Z1 = (Va - Vb) / (Ia - Ib);
                R = Z1.Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            if (!Descriptor.A && Descriptor.B && Descriptor.C)
            {
                Complex Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];

                Complex Z1 = (Vb - Vc) / (Ib - Ic);
                R = Z1.Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            if (Descriptor.A && !Descriptor.B && Descriptor.C)
            {
                Complex Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Z1 = (Vc - Va) / (Ic - Ia);
                R = Z1.Imaginary / (Line.Z1).Imaginary;
                return R;
            }
            Complex Lz1 = Line.Z1;
            Complex Lz0 = Line.Z0;
            Complex K = Line.K0;
            Complex Io = Line.I3F.Sequence[0].Reader(TPhasor.TReaderMode.PU)[D.Cursor];
            if (Descriptor.A && !Descriptor.B && !Descriptor.C)
            {
                Complex Va = Line.DE.V3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ia = Line.I3F.A.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                R = (Va * Complex.Conjugate(Io)).Imaginary / (Line.Z1 * (Ia + K * Io) * Complex.Conjugate(Io)).Imaginary;
                return R;
            }
            if (!Descriptor.A && Descriptor.B && !Descriptor.C)
            {
                Complex Vb = Line.DE.V3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ib = Line.I3F.B.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                R = (Vb * Complex.Conjugate(Io)).Imaginary / (Line.Z1 * (Ib + K * Io) * Complex.Conjugate(Io)).Imaginary;
                return R;
            }
            if (!Descriptor.A && !Descriptor.B && Descriptor.C)
            {
                Complex Vc = Line.DE.V3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                Complex Ic = Line.I3F.C.Phasor.Reader(TPhasor.TReaderMode.PU)[D.Cursor];
                R = (Vc * Complex.Conjugate(Io)).Imaginary / (Line.Z1 * (Ic + K * Io) * Complex.Conjugate(Io)).Imaginary;
                return R;
            }
            return R;
        }
    }
}
