namespace PowerSystem.Dialogs
{
    partial class INFEditor
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
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LBVoltage = new System.Windows.Forms.ListBox();
            this.TxNode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LBCurrent = new System.Windows.Forms.ListBox();
            this.LBDigital = new System.Windows.Forms.ListBox();
            this.Ok = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Ignore = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.CBVoltage = new System.Windows.Forms.ComboBox();
            this.LBNode = new System.Windows.Forms.ListView();
            this.LBLine = new System.Windows.Forms.ListView();
            this.Salve = new System.Windows.Forms.Button();
            this.netView1 = new PowerSystem.Methods.NetWorkView.NetView();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Lines:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label5.Click += new System.EventHandler(this.TxNode_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(538, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Voltages";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LBVoltage
            // 
            this.LBVoltage.FormattingEnabled = true;
            this.LBVoltage.Location = new System.Drawing.Point(538, 25);
            this.LBVoltage.Name = "LBVoltage";
            this.LBVoltage.Size = new System.Drawing.Size(170, 82);
            this.LBVoltage.TabIndex = 2;
            this.LBVoltage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.List_MouseDown);
            // 
            // TxNode
            // 
            this.TxNode.Location = new System.Drawing.Point(179, 6);
            this.TxNode.Name = "TxNode";
            this.TxNode.Size = new System.Drawing.Size(134, 20);
            this.TxNode.TabIndex = 6;
            this.TxNode.TextChanged += new System.EventHandler(this.TxNode_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Nodes:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(711, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Currents";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LBCurrent
            // 
            this.LBCurrent.FormattingEnabled = true;
            this.LBCurrent.Location = new System.Drawing.Point(714, 25);
            this.LBCurrent.Name = "LBCurrent";
            this.LBCurrent.Size = new System.Drawing.Size(170, 82);
            this.LBCurrent.TabIndex = 2;
            this.LBCurrent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.List_MouseDown);
            // 
            // LBDigital
            // 
            this.LBDigital.FormattingEnabled = true;
            this.LBDigital.Location = new System.Drawing.Point(891, 25);
            this.LBDigital.Name = "LBDigital";
            this.LBDigital.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.LBDigital.Size = new System.Drawing.Size(170, 82);
            this.LBDigital.TabIndex = 5;
            this.LBDigital.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DList_MouseDown);
            // 
            // Ok
            // 
            this.Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Ok.Location = new System.Drawing.Point(268, 41);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 23);
            this.Ok.TabIndex = 8;
            this.Ok.Text = "Ok";
            this.Ok.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(349, 41);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 10;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(888, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Digitals";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Ignore
            // 
            this.Ignore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Ignore.Location = new System.Drawing.Point(430, 41);
            this.Ignore.Name = "Ignore";
            this.Ignore.Size = new System.Drawing.Size(75, 23);
            this.Ignore.TabIndex = 10;
            this.Ignore.Text = "Ignore";
            this.Ignore.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(135, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Name:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(319, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Voltage:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // CBVoltage
            // 
            this.CBVoltage.FormattingEnabled = true;
            this.CBVoltage.Location = new System.Drawing.Point(371, 6);
            this.CBVoltage.Name = "CBVoltage";
            this.CBVoltage.Size = new System.Drawing.Size(134, 21);
            this.CBVoltage.TabIndex = 13;
            this.CBVoltage.SelectedIndexChanged += new System.EventHandler(this.CBVoltage_SelectedIndexChanged);
            // 
            // LBNode
            // 
            this.LBNode.FullRowSelect = true;
            this.LBNode.Location = new System.Drawing.Point(12, 74);
            this.LBNode.MultiSelect = false;
            this.LBNode.Name = "LBNode";
            this.LBNode.Size = new System.Drawing.Size(520, 134);
            this.LBNode.TabIndex = 14;
            this.LBNode.UseCompatibleStateImageBehavior = false;
            this.LBNode.View = System.Windows.Forms.View.Details;
            this.LBNode.SelectedIndexChanged += new System.EventHandler(this.LBNode_SelectedIndexChanged);
            this.LBNode.DoubleClick += new System.EventHandler(this.LBNode_DoubleClick);
            // 
            // LBLine
            // 
            this.LBLine.FullRowSelect = true;
            this.LBLine.Location = new System.Drawing.Point(9, 254);
            this.LBLine.MultiSelect = false;
            this.LBLine.Name = "LBLine";
            this.LBLine.Size = new System.Drawing.Size(523, 142);
            this.LBLine.TabIndex = 15;
            this.LBLine.UseCompatibleStateImageBehavior = false;
            this.LBLine.View = System.Windows.Forms.View.Details;
            this.LBLine.DoubleClick += new System.EventHandler(this.LBLine_DoubleClick);
            // 
            // Salve
            // 
            this.Salve.Location = new System.Drawing.Point(163, 41);
            this.Salve.Name = "Salve";
            this.Salve.Size = new System.Drawing.Size(99, 23);
            this.Salve.TabIndex = 8;
            this.Salve.Text = "Salve in AppData";
            this.Salve.UseVisualStyleBackColor = true;
            this.Salve.Click += new System.EventHandler(this.Salve_Click);
            // 
            // netView1
            // 
            this.netView1.AllowDrop = true;
            this.netView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.netView1.BackColor = System.Drawing.Color.White;
            this.netView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.netView1.Location = new System.Drawing.Point(538, 113);
            this.netView1.Name = "netView1";
            this.netView1.Size = new System.Drawing.Size(523, 283);
            this.netView1.TabIndex = 16;
            this.netView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.netView1_DragDrop);
            this.netView1.DragOver += new System.Windows.Forms.DragEventHandler(this.netView1_DragOver);
            // 
            // INFEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 408);
            this.Controls.Add(this.netView1);
            this.Controls.Add(this.LBLine);
            this.Controls.Add(this.LBNode);
            this.Controls.Add(this.CBVoltage);
            this.Controls.Add(this.TxNode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LBVoltage);
            this.Controls.Add(this.LBCurrent);
            this.Controls.Add(this.Ignore);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Salve);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.LBDigital);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "INFEditor";
            this.Text = "Select";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox LBVoltage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox LBCurrent;
        private System.Windows.Forms.TextBox TxNode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox LBDigital;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox CBVoltage;
        private System.Windows.Forms.ListView LBNode;
        private System.Windows.Forms.ListView LBLine;
        public System.Windows.Forms.Button Ignore;
        private Methods.NetWorkView.NetView netView1;
        private System.Windows.Forms.Button Salve;


    }
}