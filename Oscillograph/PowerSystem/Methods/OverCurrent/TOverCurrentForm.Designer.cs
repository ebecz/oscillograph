namespace PowerSystem.Methods.OverCurrent
{
    partial class TOverCurrentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CurveSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ClassTipo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.UpDownMultiple = new System.Windows.Forms.NumericUpDown();
            this.UpDownPickUp = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.UpDownDec = new System.Windows.Forms.NumericUpDown();
            this.PaintPanel = new Methods.OverCurrent.TPaintPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.UpDownInstanteneous = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownMultiple)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownPickUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownDec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownInstanteneous)).BeginInit();
            this.SuspendLayout();
            // 
            // CurveSelection
            // 
            this.CurveSelection.FormattingEnabled = true;
            this.CurveSelection.Location = new System.Drawing.Point(12, 31);
            this.CurveSelection.Name = "CurveSelection";
            this.CurveSelection.Size = new System.Drawing.Size(142, 21);
            this.CurveSelection.TabIndex = 0;
            this.CurveSelection.SelectedIndexChanged += new System.EventHandler(this.CurveSelection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Curve:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pick Up:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Multiple";
            // 
            // ClassTipo
            // 
            this.ClassTipo.FormattingEnabled = true;
            this.ClassTipo.Location = new System.Drawing.Point(12, 71);
            this.ClassTipo.Name = "ClassTipo";
            this.ClassTipo.Size = new System.Drawing.Size(142, 21);
            this.ClassTipo.TabIndex = 0;
            this.ClassTipo.SelectedIndexChanged += new System.EventHandler(this.ClassTipo_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Classification:";
            // 
            // UpDownMultiple
            // 
            this.UpDownMultiple.DecimalPlaces = 3;
            this.UpDownMultiple.Location = new System.Drawing.Point(12, 111);
            this.UpDownMultiple.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.UpDownMultiple.Name = "UpDownMultiple";
            this.UpDownMultiple.Size = new System.Drawing.Size(142, 20);
            this.UpDownMultiple.TabIndex = 4;
            this.UpDownMultiple.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpDownMultiple.ValueChanged += new System.EventHandler(this.UpDownMultiple_ValueChanged);
            // 
            // UpDownPickUp
            // 
            this.UpDownPickUp.DecimalPlaces = 3;
            this.UpDownPickUp.Location = new System.Drawing.Point(12, 150);
            this.UpDownPickUp.Name = "UpDownPickUp";
            this.UpDownPickUp.Size = new System.Drawing.Size(142, 20);
            this.UpDownPickUp.TabIndex = 4;
            this.UpDownPickUp.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpDownPickUp.ValueChanged += new System.EventHandler(this.UpDownPickUp_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Decade:";
            // 
            // UpDownDec
            // 
            this.UpDownDec.Location = new System.Drawing.Point(12, 228);
            this.UpDownDec.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.UpDownDec.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpDownDec.Name = "UpDownDec";
            this.UpDownDec.Size = new System.Drawing.Size(142, 20);
            this.UpDownDec.TabIndex = 4;
            this.UpDownDec.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.UpDownDec.ValueChanged += new System.EventHandler(this.UpDownDec_ValueChanged);
            // 
            // PaintPanel
            // 
            this.PaintPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PaintPanel.BackColor = System.Drawing.Color.White;
            this.PaintPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PaintPanel.Location = new System.Drawing.Point(160, 12);
            this.PaintPanel.Name = "PaintPanel";
            this.PaintPanel.Size = new System.Drawing.Size(393, 238);
            this.PaintPanel.TabIndex = 3;
            this.PaintPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintPanel_Paint);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Instantaneous:";
            // 
            // UpDownInstanteneous
            // 
            this.UpDownInstanteneous.DecimalPlaces = 3;
            this.UpDownInstanteneous.Location = new System.Drawing.Point(12, 189);
            this.UpDownInstanteneous.Name = "UpDownInstanteneous";
            this.UpDownInstanteneous.Size = new System.Drawing.Size(142, 20);
            this.UpDownInstanteneous.TabIndex = 4;
            this.UpDownInstanteneous.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpDownInstanteneous.ValueChanged += new System.EventHandler(this.UpDownPickUp_ValueChanged);
            // 
            // TOverCurrentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 262);
            this.Controls.Add(this.UpDownDec);
            this.Controls.Add(this.UpDownInstanteneous);
            this.Controls.Add(this.UpDownPickUp);
            this.Controls.Add(this.UpDownMultiple);
            this.Controls.Add(this.PaintPanel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ClassTipo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CurveSelection);
            this.Name = "TOverCurrentForm";
            this.Text = "TOverCurrentForm";
            ((System.ComponentModel.ISupportInitialize)(this.UpDownMultiple)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownPickUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownDec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownInstanteneous)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CurveSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private TPaintPanel PaintPanel;
        private System.Windows.Forms.ComboBox ClassTipo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown UpDownMultiple;
        private System.Windows.Forms.NumericUpDown UpDownPickUp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown UpDownDec;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown UpDownInstanteneous;
    }
}