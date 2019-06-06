namespace VioGIS
{
    partial class SetSpatialReference
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chbNumber = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCenterLongitude = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbtn6degree = new System.Windows.Forms.RadioButton();
            this.rbtn3degree = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnWGS = new System.Windows.Forms.RadioButton();
            this.rbtnBeijing = new System.Windows.Forms.RadioButton();
            this.rbtnXian = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.chbNumber);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(505, 491);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "空间参考基本参数";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(337, 437);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 27);
            this.button2.TabIndex = 5;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(337, 388);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 29);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chbNumber
            // 
            this.chbNumber.AutoSize = true;
            this.chbNumber.Location = new System.Drawing.Point(47, 413);
            this.chbNumber.Name = "chbNumber";
            this.chbNumber.Size = new System.Drawing.Size(134, 19);
            this.chbNumber.TabIndex = 3;
            this.chbNumber.Text = "横坐标前加带号";
            this.chbNumber.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.txtCenterLongitude);
            this.groupBox4.Location = new System.Drawing.Point(24, 253);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(466, 100);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "中央子午线经度确定";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "中央子午线";
            // 
            // txtCenterLongitude
            // 
            this.txtCenterLongitude.Location = new System.Drawing.Point(226, 43);
            this.txtCenterLongitude.Name = "txtCenterLongitude";
            this.txtCenterLongitude.Size = new System.Drawing.Size(187, 25);
            this.txtCenterLongitude.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbtn6degree);
            this.groupBox3.Controls.Add(this.rbtn3degree);
            this.groupBox3.Location = new System.Drawing.Point(290, 37);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 177);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "分带类型";
            // 
            // rbtn6degree
            // 
            this.rbtn6degree.AutoSize = true;
            this.rbtn6degree.Location = new System.Drawing.Point(38, 120);
            this.rbtn6degree.Name = "rbtn6degree";
            this.rbtn6degree.Size = new System.Drawing.Size(96, 19);
            this.rbtn6degree.TabIndex = 1;
            this.rbtn6degree.TabStop = true;
            this.rbtn6degree.Text = "6度带投影";
            this.rbtn6degree.UseVisualStyleBackColor = true;
            // 
            // rbtn3degree
            // 
            this.rbtn3degree.AutoSize = true;
            this.rbtn3degree.Location = new System.Drawing.Point(38, 60);
            this.rbtn3degree.Name = "rbtn3degree";
            this.rbtn3degree.Size = new System.Drawing.Size(96, 19);
            this.rbtn3degree.TabIndex = 0;
            this.rbtn3degree.TabStop = true;
            this.rbtn3degree.Text = "3度带投影";
            this.rbtn3degree.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnWGS);
            this.groupBox2.Controls.Add(this.rbtnBeijing);
            this.groupBox2.Controls.Add(this.rbtnXian);
            this.groupBox2.Location = new System.Drawing.Point(24, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 177);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "坐标系类型";
            // 
            // rbtnWGS
            // 
            this.rbtnWGS.AutoSize = true;
            this.rbtnWGS.Location = new System.Drawing.Point(33, 136);
            this.rbtnWGS.Name = "rbtnWGS";
            this.rbtnWGS.Size = new System.Drawing.Size(76, 19);
            this.rbtnWGS.TabIndex = 2;
            this.rbtnWGS.TabStop = true;
            this.rbtnWGS.Text = "WGS-84";
            this.rbtnWGS.UseVisualStyleBackColor = true;
            // 
            // rbtnBeijing
            // 
            this.rbtnBeijing.AutoSize = true;
            this.rbtnBeijing.Location = new System.Drawing.Point(33, 93);
            this.rbtnBeijing.Name = "rbtnBeijing";
            this.rbtnBeijing.Size = new System.Drawing.Size(74, 19);
            this.rbtnBeijing.TabIndex = 1;
            this.rbtnBeijing.TabStop = true;
            this.rbtnBeijing.Text = "北京54";
            this.rbtnBeijing.UseVisualStyleBackColor = true;
            // 
            // rbtnXian
            // 
            this.rbtnXian.AutoSize = true;
            this.rbtnXian.Location = new System.Drawing.Point(33, 45);
            this.rbtnXian.Name = "rbtnXian";
            this.rbtnXian.Size = new System.Drawing.Size(74, 19);
            this.rbtnXian.TabIndex = 0;
            this.rbtnXian.TabStop = true;
            this.rbtnXian.Text = "西安80";
            this.rbtnXian.UseVisualStyleBackColor = true;
            // 
            // SetSpatialReference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 516);
            this.Controls.Add(this.groupBox1);
            this.Name = "SetSpatialReference";
            this.Text = "设置空间参考";
            this.Load += new System.EventHandler(this.SetSpatialReference_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chbNumber;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCenterLongitude;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbtn6degree;
        private System.Windows.Forms.RadioButton rbtn3degree;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbtnWGS;
        private System.Windows.Forms.RadioButton rbtnBeijing;
        private System.Windows.Forms.RadioButton rbtnXian;
    }
}