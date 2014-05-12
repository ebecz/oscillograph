using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

using PowerSystem.WaveView;
using PowerSystem.NetWork;

namespace PowerSystem.Methods.WaveView
{
    internal partial class TWaveView : TMethod<TInstance>
    {
        public string Text
        {
            get
            {
                return "Main Panel";
            }
        }
        public string Description
        {
            get
            {
                return "Exibição do Painel Principal";
            }
        }
        public TWaveView()
        {
            ToolStripButton CursorStripButton = new System.Windows.Forms.ToolStripButton();
            ToolStripButton AnchorStripButton = new System.Windows.Forms.ToolStripButton();
            ToolStripButton IsoScaleStripButton = new System.Windows.Forms.ToolStripButton();
            ToolStripButton AutoZoomStripButton = new System.Windows.Forms.ToolStripButton();
            ToolStripButton AutoTimeStripButton = new System.Windows.Forms.ToolStripButton();
            ToolStripButton AutoStripButton = new System.Windows.Forms.ToolStripButton();
            // 
            // CursorStripButton
            // 
            CursorStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            CursorStripButton.Image = Resource.CursorStripButton_Image;
            CursorStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            CursorStripButton.Name = "CursorStripButton";
            CursorStripButton.Size = new System.Drawing.Size(23, 22);
            CursorStripButton.Text = "Cursor";
            CursorStripButton.ToolTipText = "Set Cursor on time line";
            CursorStripButton.Click += (object sender, EventArgs e) =>
            {
                TWaveViewForm Component = (TWaveViewForm)Active;
                Component.Grafico.ActiveCursor = Component.Grafico.Instance.Cursor;
                Component.Grafico.ActiveCursor.Enabled = false;
                AnchorStripButton.Checked = false;
                CursorStripButton.Checked = true;
            };
            // 
            // AnchorStripButton
            // 
            AnchorStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            AnchorStripButton.Image = Resource.AnchorStripButton_Image;
            AnchorStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            AnchorStripButton.Name = "AnchorStripButton";
            AnchorStripButton.Size = new System.Drawing.Size(23, 22);
            AnchorStripButton.Text = "Anchor";
            AnchorStripButton.ToolTipText = "Anchor";
            AnchorStripButton.Click += (object sender, EventArgs e) =>
            {
                TWaveViewForm Component = (TWaveViewForm)Active;
                Component.Grafico.ActiveCursor = Component.Grafico.Instance.Anchor;
                Component.Grafico.Instance.Anchor.Enabled = false;
                Component.Grafico.Instance.Time.Reference.Fixed = false;
                AnchorStripButton.Checked = true;
                CursorStripButton.Checked = false;
            };
            // 
            // IsoScaleStripButton
            // 
            IsoScaleStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            IsoScaleStripButton.Image = Resource.IsoScaleStripButton_Image;
            IsoScaleStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            IsoScaleStripButton.Name = "IsoScaleStripButton";
            IsoScaleStripButton.Size = new System.Drawing.Size(23, 22);
            IsoScaleStripButton.Text = "Auto scale";
            IsoScaleStripButton.ToolTipText = "Iso Scale";
            IsoScaleStripButton.Click += (object sender, EventArgs e) =>
            {
                TWaveViewForm Component = (TWaveViewForm)Active;
                Component.Grafico.IsoScale();
            };
            // 
            // AutoZoomStripButton
            // 
            AutoZoomStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            AutoZoomStripButton.Image = Resource.AutoZoomStripButton_Image;
            AutoZoomStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            AutoZoomStripButton.Name = "AutoZoomStripButton";
            AutoZoomStripButton.Size = new System.Drawing.Size(23, 22);
            AutoZoomStripButton.Text = "Zoom in time Line";
            AutoZoomStripButton.Click += (object sender, EventArgs e) =>
            {
                TWaveViewForm Component = (TWaveViewForm)Active;
                Component.Grafico.Rescale();
            };
            AutoZoomStripButton.MouseDown += (object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    TWaveViewForm Component = (TWaveViewForm)Active;
                    Component.Grafico.AutoScale = AutoZoomStripButton.Checked = !AutoZoomStripButton.Checked;
                }
            };
            // 
            // AutoTimeStripButton
            // 
            AutoTimeStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            AutoTimeStripButton.Image = Resource.AutoTimeStripButton_Image;
            AutoTimeStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            AutoTimeStripButton.Name = "AutoTimeStripButton";
            AutoTimeStripButton.Size = new System.Drawing.Size(23, 22);
            AutoTimeStripButton.Text = "Zoom Time Scale";
            AutoTimeStripButton.Click += (object sender, EventArgs e) =>
            {
                TWaveViewForm Component = (TWaveViewForm)Active;
                Component.Grafico.Instance.Time.SetAandB(Component.Grafico.Instance.Time.Start, Component.Grafico.Instance.Time.End);
            };
            // 
            // AutoStripButton1
            // 
            AutoStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            AutoStripButton.Image = Resource.AutoTimeStripButton_Image;
            AutoStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            AutoStripButton.Name = "AutoStripButton1";
            AutoStripButton.Size = new System.Drawing.Size(23, 22);
            AutoStripButton.Text = "All Record";
            AutoStripButton.ToolTipText = "All Record";
            AutoStripButton.Click += (object sender, EventArgs e) =>
            {
                TWaveViewForm Component = (TWaveViewForm)Active;
                Component.Grafico.Instance.Time.SetAandB(Component.Grafico.Instance.Time.Start, Component.Grafico.Instance.Time.End);
            };
            // 
            // toolStrip
            // 
            ToolStrip.AllowDrop = true;
            ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            CursorStripButton,
            AnchorStripButton,
            IsoScaleStripButton,
            AutoZoomStripButton,
            AutoTimeStripButton});
            ToolStrip.Location = new System.Drawing.Point(3, 0);
            ToolStrip.Name = "toolStrip";
            ToolStrip.Size = new System.Drawing.Size(289, 25);
            ToolStrip.TabIndex = 1;
            ToolStrip.Text = "ToolStrip";
            ToolStrip.Enabled = false;
        }
        private ToolStrip ToolStrip=new ToolStrip();
        public ToolStrip MenuTool
        {
            get
            {
                return ToolStrip;
            }
        }
        private TWaveViewForm Active;
        private class TMyResult: TResult
        {
            public TWaveViewForm MyForm;
            public TMyResult(TWaveViewForm Form)
            {
                MyForm = Form;
            }
            public void Show(Form ParentForm)
            {
                MyForm.MdiParent = ParentForm;
                MyForm.Show();
            }
            public object Data
            {
                get
                {
                    return MyForm;
                }
            }
        }
        public TResult Execute(TInstance Instance)
        {
            TWaveViewForm MyForm = new TWaveViewForm(Instance);
            MyForm.Activated += (object sender, EventArgs e) =>
            {
                MenuTool.Enabled = true;
                Active = (TWaveViewForm)sender;
            };
            MyForm.Deactivate += (object sender, EventArgs e) =>
            {
                MenuTool.Enabled = false;
            };
            return new TMyResult(MyForm);
        }
    }
}
