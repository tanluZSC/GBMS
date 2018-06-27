namespace GBMSAPI_CS_Example.CALIBRATION
{
    partial class CalibrationForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationForm));
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rbRolledThenarArea = new System.Windows.Forms.RadioButton();
			this.rbRolledJointArea = new System.Windows.Forms.RadioButton();
			this.rbRollGaArea = new System.Windows.Forms.RadioButton();
			this.rbRollIqsArea = new System.Windows.Forms.RadioButton();
			this.rbFullFrameArea = new System.Windows.Forms.RadioButton();
			this.GetNewCalibrationImageButton = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.CalibrateDeviceButton = new System.Windows.Forms.Button();
			this.SetFactoryCalibrationButton = new System.Windows.Forms.Button();
			this.SaveButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.DiagnosticListBox = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ImSYTextBox = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.ImSXTextBox = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.CalibrationImagePictureBox = new System.Windows.Forms.PictureBox();
			this.groupBox2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.CalibrationImagePictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rbRolledThenarArea);
			this.groupBox2.Controls.Add(this.rbRolledJointArea);
			this.groupBox2.Controls.Add(this.rbRollGaArea);
			this.groupBox2.Controls.Add(this.rbRollIqsArea);
			this.groupBox2.Controls.Add(this.rbFullFrameArea);
			this.groupBox2.Controls.Add(this.GetNewCalibrationImageButton);
			this.groupBox2.Location = new System.Drawing.Point(861, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(304, 286);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Calibration Image Acquisition";
			// 
			// rbRolledThenarArea
			// 
			this.rbRolledThenarArea.AutoSize = true;
			this.rbRolledThenarArea.Location = new System.Drawing.Point(9, 163);
			this.rbRolledThenarArea.Name = "rbRolledThenarArea";
			this.rbRolledThenarArea.Size = new System.Drawing.Size(148, 17);
			this.rbRolledThenarArea.TabIndex = 10;
			this.rbRolledThenarArea.TabStop = true;
			this.rbRolledThenarArea.Text = "ROLLED THENAR AREA";
			this.rbRolledThenarArea.UseVisualStyleBackColor = true;
			this.rbRolledThenarArea.CheckedChanged += new System.EventHandler(this.rbRolledThenarArea_CheckedChanged);
			// 
			// rbRolledJointArea
			// 
			this.rbRolledJointArea.AutoSize = true;
			this.rbRolledJointArea.Location = new System.Drawing.Point(9, 128);
			this.rbRolledJointArea.Name = "rbRolledJointArea";
			this.rbRolledJointArea.Size = new System.Drawing.Size(134, 17);
			this.rbRolledJointArea.TabIndex = 9;
			this.rbRolledJointArea.TabStop = true;
			this.rbRolledJointArea.Text = "ROLLED JOINT AREA";
			this.rbRolledJointArea.UseVisualStyleBackColor = true;
			this.rbRolledJointArea.CheckedChanged += new System.EventHandler(this.rbRolledJointArea_CheckedChanged);
			// 
			// rbRollGaArea
			// 
			this.rbRollGaArea.AutoSize = true;
			this.rbRollGaArea.Location = new System.Drawing.Point(9, 92);
			this.rbRollGaArea.Name = "rbRollGaArea";
			this.rbRollGaArea.Size = new System.Drawing.Size(103, 17);
			this.rbRollGaArea.TabIndex = 8;
			this.rbRollGaArea.TabStop = true;
			this.rbRollGaArea.Text = "ROLL GA AREA";
			this.rbRollGaArea.UseVisualStyleBackColor = true;
			this.rbRollGaArea.CheckedChanged += new System.EventHandler(this.rbRollGaArea_CheckedChanged);
			// 
			// rbRollIqsArea
			// 
			this.rbRollIqsArea.AutoSize = true;
			this.rbRollIqsArea.Location = new System.Drawing.Point(9, 57);
			this.rbRollIqsArea.Name = "rbRollIqsArea";
			this.rbRollIqsArea.Size = new System.Drawing.Size(106, 17);
			this.rbRollIqsArea.TabIndex = 7;
			this.rbRollIqsArea.TabStop = true;
			this.rbRollIqsArea.Text = "ROLL IQS AREA";
			this.rbRollIqsArea.UseVisualStyleBackColor = true;
			this.rbRollIqsArea.CheckedChanged += new System.EventHandler(this.rbRollIqsArea_CheckedChanged);
			// 
			// rbFullFrameArea
			// 
			this.rbFullFrameArea.AutoSize = true;
			this.rbFullFrameArea.Location = new System.Drawing.Point(9, 28);
			this.rbFullFrameArea.Name = "rbFullFrameArea";
			this.rbFullFrameArea.Size = new System.Drawing.Size(123, 17);
			this.rbFullFrameArea.TabIndex = 6;
			this.rbFullFrameArea.TabStop = true;
			this.rbFullFrameArea.Text = "FULL FRAME AREA";
			this.rbFullFrameArea.UseVisualStyleBackColor = true;
			this.rbFullFrameArea.CheckedChanged += new System.EventHandler(this.rbFullFrameArea_CheckedChanged);
			// 
			// GetNewCalibrationImageButton
			// 
			this.GetNewCalibrationImageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GetNewCalibrationImageButton.Location = new System.Drawing.Point(80, 213);
			this.GetNewCalibrationImageButton.Name = "GetNewCalibrationImageButton";
			this.GetNewCalibrationImageButton.Size = new System.Drawing.Size(145, 46);
			this.GetNewCalibrationImageButton.TabIndex = 5;
			this.GetNewCalibrationImageButton.Text = "Get New Image";
			this.GetNewCalibrationImageButton.UseVisualStyleBackColor = true;
			this.GetNewCalibrationImageButton.Click += new System.EventHandler(this.GetNewCalibrationImageButton_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.CalibrateDeviceButton);
			this.groupBox5.Controls.Add(this.SetFactoryCalibrationButton);
			this.groupBox5.Location = new System.Drawing.Point(12, 110);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(286, 100);
			this.groupBox5.TabIndex = 8;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Device Calibration";
			// 
			// CalibrateDeviceButton
			// 
			this.CalibrateDeviceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CalibrateDeviceButton.Image = ((System.Drawing.Image)(resources.GetObject("CalibrateDeviceButton.Image")));
			this.CalibrateDeviceButton.Location = new System.Drawing.Point(10, 30);
			this.CalibrateDeviceButton.Name = "CalibrateDeviceButton";
			this.CalibrateDeviceButton.Size = new System.Drawing.Size(127, 44);
			this.CalibrateDeviceButton.TabIndex = 2;
			this.CalibrateDeviceButton.Text = "Calibrate";
			this.CalibrateDeviceButton.UseVisualStyleBackColor = true;
			this.CalibrateDeviceButton.Click += new System.EventHandler(this.CalibrateDeviceButton_Click);
			// 
			// SetFactoryCalibrationButton
			// 
			this.SetFactoryCalibrationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SetFactoryCalibrationButton.Image = ((System.Drawing.Image)(resources.GetObject("SetFactoryCalibrationButton.Image")));
			this.SetFactoryCalibrationButton.Location = new System.Drawing.Point(151, 30);
			this.SetFactoryCalibrationButton.Name = "SetFactoryCalibrationButton";
			this.SetFactoryCalibrationButton.Size = new System.Drawing.Size(119, 44);
			this.SetFactoryCalibrationButton.TabIndex = 3;
			this.SetFactoryCalibrationButton.Text = "Set Factory";
			this.SetFactoryCalibrationButton.UseVisualStyleBackColor = true;
			this.SetFactoryCalibrationButton.Click += new System.EventHandler(this.SetFactoryCalibrationButton_Click);
			// 
			// SaveButton
			// 
			this.SaveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
			this.SaveButton.Location = new System.Drawing.Point(16, 28);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(145, 46);
			this.SaveButton.TabIndex = 4;
			this.SaveButton.Text = "SaveImage";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.DiagnosticListBox);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.ImSYTextBox);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.ImSXTextBox);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Location = new System.Drawing.Point(861, 383);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(310, 204);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Calibration Image Info";
			// 
			// DiagnosticListBox
			// 
			this.DiagnosticListBox.FormattingEnabled = true;
			this.DiagnosticListBox.Location = new System.Drawing.Point(8, 104);
			this.DiagnosticListBox.Name = "DiagnosticListBox";
			this.DiagnosticListBox.Size = new System.Drawing.Size(296, 82);
			this.DiagnosticListBox.TabIndex = 27;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 83);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 26;
			this.label1.Text = "Diagnostic Value";
			// 
			// ImSYTextBox
			// 
			this.ImSYTextBox.Location = new System.Drawing.Point(111, 53);
			this.ImSYTextBox.Name = "ImSYTextBox";
			this.ImSYTextBox.ReadOnly = true;
			this.ImSYTextBox.Size = new System.Drawing.Size(100, 20);
			this.ImSYTextBox.TabIndex = 23;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(5, 56);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(69, 13);
			this.label13.TabIndex = 22;
			this.label13.Text = "Image Size Y";
			// 
			// ImSXTextBox
			// 
			this.ImSXTextBox.Location = new System.Drawing.Point(112, 22);
			this.ImSXTextBox.Name = "ImSXTextBox";
			this.ImSXTextBox.ReadOnly = true;
			this.ImSXTextBox.Size = new System.Drawing.Size(100, 20);
			this.ImSXTextBox.TabIndex = 21;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(6, 25);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(69, 13);
			this.label14.TabIndex = 20;
			this.label14.Text = "Image Size X";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.SaveButton);
			this.groupBox4.Location = new System.Drawing.Point(12, 19);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(200, 85);
			this.groupBox4.TabIndex = 8;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Calibration Image Storage";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.groupBox5);
			this.groupBox3.Controls.Add(this.groupBox4);
			this.groupBox3.Location = new System.Drawing.Point(861, 616);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(310, 228);
			this.groupBox3.TabIndex = 11;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Calibration Image Setting";
			// 
			// CalibrationImagePictureBox
			// 
			this.CalibrationImagePictureBox.BackColor = System.Drawing.Color.White;
			this.CalibrationImagePictureBox.Location = new System.Drawing.Point(12, 12);
			this.CalibrationImagePictureBox.Name = "CalibrationImagePictureBox";
			this.CalibrationImagePictureBox.Size = new System.Drawing.Size(832, 832);
			this.CalibrationImagePictureBox.TabIndex = 8;
			this.CalibrationImagePictureBox.TabStop = false;
			// 
			// CalibrationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1190, 860);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.CalibrationImagePictureBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CalibrationForm";
			this.Text = "Calibration Tool";
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.CalibrationImagePictureBox)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbRollIqsArea;
        private System.Windows.Forms.RadioButton rbFullFrameArea;
        private System.Windows.Forms.Button GetNewCalibrationImageButton;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button CalibrateDeviceButton;
        private System.Windows.Forms.Button SetFactoryCalibrationButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox DiagnosticListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ImSYTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox ImSXTextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PictureBox CalibrationImagePictureBox;
		private System.Windows.Forms.RadioButton rbRolledThenarArea;
		private System.Windows.Forms.RadioButton rbRolledJointArea;
		private System.Windows.Forms.RadioButton rbRollGaArea;
    }
}