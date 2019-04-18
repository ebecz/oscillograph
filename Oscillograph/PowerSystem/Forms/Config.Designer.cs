namespace PowerSystem.Forms
{
    partial class ConfigForm
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
            this.tab = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.Zone = new System.Windows.Forms.ComboBox();
            this.groupBoxColor = new System.Windows.Forms.GroupBox();
            this.N = new System.Windows.Forms.Button();
            this.S2 = new System.Windows.Forms.Button();
            this.C = new System.Windows.Forms.Button();
            this.Off = new System.Windows.Forms.Button();
            this.S1 = new System.Windows.Forms.Button();
            this.B = new System.Windows.Forms.Button();
            this.On = new System.Windows.Forms.Button();
            this.S0 = new System.Windows.Forms.Button();
            this.A = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AnaFilePath = new System.Windows.Forms.TextBox();
            this.OpenAnaFile = new System.Windows.Forms.Button();
            this.tabMethods = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Apply = new System.Windows.Forms.Button();
            this.tab.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupBoxColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab
            // 
            this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tab.Controls.Add(this.tabGeneral);
            this.tab.Controls.Add(this.tabMethods);
            this.tab.Location = new System.Drawing.Point(0, 0);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(284, 329);
            this.tab.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.Zone);
            this.tabGeneral.Controls.Add(this.groupBoxColor);
            this.tabGeneral.Controls.Add(this.label14);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.AnaFilePath);
            this.tabGeneral.Controls.Add(this.OpenAnaFile);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(276, 303);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General Settings";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // Zone
            // 
            this.Zone.FormattingEnabled = true;
            this.Zone.Location = new System.Drawing.Point(103, 256);
            this.Zone.Name = "Zone";
            this.Zone.Size = new System.Drawing.Size(140, 21);
            this.Zone.TabIndex = 25;
            this.Zone.SelectedIndexChanged += new System.EventHandler(this.Zone_SelectedIndexChanged);
            // 
            // groupBoxColor
            // 
            this.groupBoxColor.Controls.Add(this.N);
            this.groupBoxColor.Controls.Add(this.S2);
            this.groupBoxColor.Controls.Add(this.C);
            this.groupBoxColor.Controls.Add(this.Off);
            this.groupBoxColor.Controls.Add(this.S1);
            this.groupBoxColor.Controls.Add(this.B);
            this.groupBoxColor.Controls.Add(this.On);
            this.groupBoxColor.Controls.Add(this.S0);
            this.groupBoxColor.Controls.Add(this.A);
            this.groupBoxColor.Controls.Add(this.label6);
            this.groupBoxColor.Controls.Add(this.label8);
            this.groupBoxColor.Controls.Add(this.label13);
            this.groupBoxColor.Controls.Add(this.label5);
            this.groupBoxColor.Controls.Add(this.label7);
            this.groupBoxColor.Controls.Add(this.label12);
            this.groupBoxColor.Controls.Add(this.label4);
            this.groupBoxColor.Controls.Add(this.label11);
            this.groupBoxColor.Controls.Add(this.label3);
            this.groupBoxColor.Controls.Add(this.label9);
            this.groupBoxColor.Controls.Add(this.label10);
            this.groupBoxColor.Controls.Add(this.label2);
            this.groupBoxColor.Location = new System.Drawing.Point(9, 54);
            this.groupBoxColor.Name = "groupBoxColor";
            this.groupBoxColor.Size = new System.Drawing.Size(259, 196);
            this.groupBoxColor.TabIndex = 4;
            this.groupBoxColor.TabStop = false;
            this.groupBoxColor.Text = "Colors";
            // 
            // N
            // 
            this.N.Location = new System.Drawing.Point(207, 42);
            this.N.Name = "N";
            this.N.Size = new System.Drawing.Size(27, 25);
            this.N.TabIndex = 18;
            this.N.UseVisualStyleBackColor = true;
            this.N.Click += new System.EventHandler(this.bColor_Click);
            // 
            // S2
            // 
            this.S2.Location = new System.Drawing.Point(150, 102);
            this.S2.Name = "S2";
            this.S2.Size = new System.Drawing.Size(27, 25);
            this.S2.TabIndex = 19;
            this.S2.UseVisualStyleBackColor = true;
            this.S2.Click += new System.EventHandler(this.bColor_Click);
            // 
            // C
            // 
            this.C.Location = new System.Drawing.Point(150, 42);
            this.C.Name = "C";
            this.C.Size = new System.Drawing.Size(27, 25);
            this.C.TabIndex = 16;
            this.C.UseVisualStyleBackColor = true;
            this.C.Click += new System.EventHandler(this.bColor_Click);
            // 
            // Off
            // 
            this.Off.Location = new System.Drawing.Point(94, 158);
            this.Off.Name = "Off";
            this.Off.Size = new System.Drawing.Size(27, 25);
            this.Off.TabIndex = 17;
            this.Off.UseVisualStyleBackColor = true;
            this.Off.Click += new System.EventHandler(this.bColor_Click);
            // 
            // S1
            // 
            this.S1.Location = new System.Drawing.Point(94, 102);
            this.S1.Name = "S1";
            this.S1.Size = new System.Drawing.Size(27, 25);
            this.S1.TabIndex = 23;
            this.S1.UseVisualStyleBackColor = true;
            this.S1.Click += new System.EventHandler(this.bColor_Click);
            // 
            // B
            // 
            this.B.Location = new System.Drawing.Point(94, 42);
            this.B.Name = "B";
            this.B.Size = new System.Drawing.Size(27, 25);
            this.B.TabIndex = 24;
            this.B.UseVisualStyleBackColor = true;
            this.B.Click += new System.EventHandler(this.bColor_Click);
            // 
            // On
            // 
            this.On.Location = new System.Drawing.Point(38, 158);
            this.On.Name = "On";
            this.On.Size = new System.Drawing.Size(27, 25);
            this.On.TabIndex = 22;
            this.On.UseVisualStyleBackColor = true;
            this.On.Click += new System.EventHandler(this.bColor_Click);
            // 
            // S0
            // 
            this.S0.Location = new System.Drawing.Point(38, 102);
            this.S0.Name = "S0";
            this.S0.Size = new System.Drawing.Size(27, 25);
            this.S0.TabIndex = 20;
            this.S0.UseVisualStyleBackColor = true;
            this.S0.Click += new System.EventHandler(this.bColor_Click);
            // 
            // A
            // 
            this.A.Location = new System.Drawing.Point(38, 42);
            this.A.Name = "A";
            this.A.Size = new System.Drawing.Size(27, 25);
            this.A.TabIndex = 21;
            this.A.UseVisualStyleBackColor = true;
            this.A.Click += new System.EventHandler(this.bColor_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(183, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "N:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(71, 164);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Off:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(127, 108);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(23, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "S2:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(127, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "C:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "On:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(71, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "S1:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(71, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "B:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 108);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "S0:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "A:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Digital Signals";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Colors";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Colors";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(24, 256);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Default Zone:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ana File Path";
            // 
            // AnaFilePath
            // 
            this.AnaFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AnaFilePath.Location = new System.Drawing.Point(6, 25);
            this.AnaFilePath.Name = "AnaFilePath";
            this.AnaFilePath.Size = new System.Drawing.Size(179, 20);
            this.AnaFilePath.TabIndex = 1;
            this.AnaFilePath.TextChanged += new System.EventHandler(this.AnaFilePath_TextChanged);
            // 
            // OpenAnaFile
            // 
            this.OpenAnaFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenAnaFile.Location = new System.Drawing.Point(193, 25);
            this.OpenAnaFile.Name = "OpenAnaFile";
            this.OpenAnaFile.Size = new System.Drawing.Size(75, 23);
            this.OpenAnaFile.TabIndex = 0;
            this.OpenAnaFile.Text = "Open";
            this.OpenAnaFile.UseVisualStyleBackColor = true;
            this.OpenAnaFile.Click += new System.EventHandler(this.OpenAnaFile_Click);
            // 
            // tabMethods
            // 
            this.tabMethods.Location = new System.Drawing.Point(4, 22);
            this.tabMethods.Name = "tabMethods";
            this.tabMethods.Padding = new System.Windows.Forms.Padding(3);
            this.tabMethods.Size = new System.Drawing.Size(276, 303);
            this.tabMethods.TabIndex = 1;
            this.tabMethods.Text = "Methods";
            this.tabMethods.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(15, 335);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(106, 335);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Apply
            // 
            this.Apply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Apply.Location = new System.Drawing.Point(196, 335);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(75, 23);
            this.Apply.TabIndex = 1;
            this.Apply.Text = "Apply";
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 362);
            this.Controls.Add(this.Apply);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tab);
            this.Name = "ConfigForm";
            this.Text = "Configuration";
            this.tab.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.groupBoxColor.ResumeLayout(false);
            this.groupBoxColor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabMethods;
        private System.Windows.Forms.Button OpenAnaFile;
        private System.Windows.Forms.TextBox AnaFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBoxColor;
        private System.Windows.Forms.Button N;
        private System.Windows.Forms.Button S2;
        private System.Windows.Forms.Button C;
        private System.Windows.Forms.Button Off;
        private System.Windows.Forms.Button S1;
        private System.Windows.Forms.Button B;
        private System.Windows.Forms.Button On;
        private System.Windows.Forms.Button S0;
        private System.Windows.Forms.Button A;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox Zone;
        private System.Windows.Forms.Button Apply;
    }
}