namespace GBMSAPI_CS_Example.ACQUISITION_FORM
{
    partial class AcquisitionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AcquisitionForm));
            this.label13 = new System.Windows.Forms.Label();
            this.AutoCapturePhaseTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.NominalFrameRateTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CurrentFrameRateTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.RollPreviewModeTextBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.FullResolutionPreviewAcceptedLabel = new System.Windows.Forms.Label();
            this.AcquireFlatObjectOnRollAreaAcceptedLabel = new System.Windows.Forms.Label();
            this.HLPCompletenessTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.AcceptImageButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ObjectNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.AutoCaptureAccettedLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ArtefactsSizeTextBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.SizeTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.ContrastTextBox = new System.Windows.Forms.TextBox();
            this.ImSYTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StartStopButton = new System.Windows.Forms.Button();
            this.AcquisitionManagementGeneralTimer = new System.Windows.Forms.Timer(this.components);
            this.DiagnosticMessageListBox = new System.Windows.Forms.ListBox();
            this.ImSXTextBox = new System.Windows.Forms.TextBox();
            this.AcquiredImagePictureBox = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.StopTypeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ImageResTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbRolledFlatRatio = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tbDryAreaPercent = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tbWetAreaPercent = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.AcquiredFramesNumTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.LostFramesNumTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PreviewLabel = new System.Windows.Forms.Label();
            this.WaitFirstFrameLabel = new System.Windows.Forms.Label();
            this.cbExcludeFinalization = new System.Windows.Forms.CheckBox();
            this.bUpdateBackgroundImage = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AcquiredImagePictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 230);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 14;
            this.label13.Text = "Contrast";
            // 
            // AutoCapturePhaseTextBox
            // 
            this.AutoCapturePhaseTextBox.Location = new System.Drawing.Point(113, 192);
            this.AutoCapturePhaseTextBox.Name = "AutoCapturePhaseTextBox";
            this.AutoCapturePhaseTextBox.ReadOnly = true;
            this.AutoCapturePhaseTextBox.Size = new System.Drawing.Size(100, 20);
            this.AutoCapturePhaseTextBox.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 195);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(102, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Auto-Capture Phase";
            // 
            // NominalFrameRateTextBox
            // 
            this.NominalFrameRateTextBox.Location = new System.Drawing.Point(113, 162);
            this.NominalFrameRateTextBox.Name = "NominalFrameRateTextBox";
            this.NominalFrameRateTextBox.ReadOnly = true;
            this.NominalFrameRateTextBox.Size = new System.Drawing.Size(100, 20);
            this.NominalFrameRateTextBox.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Nominal Frame Rate";
            // 
            // CurrentFrameRateTextBox
            // 
            this.CurrentFrameRateTextBox.Location = new System.Drawing.Point(113, 133);
            this.CurrentFrameRateTextBox.Name = "CurrentFrameRateTextBox";
            this.CurrentFrameRateTextBox.ReadOnly = true;
            this.CurrentFrameRateTextBox.Size = new System.Drawing.Size(100, 20);
            this.CurrentFrameRateTextBox.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Current Frame Rate";
            // 
            // RollPreviewModeTextBox
            // 
            this.RollPreviewModeTextBox.Location = new System.Drawing.Point(112, 108);
            this.RollPreviewModeTextBox.Name = "RollPreviewModeTextBox";
            this.RollPreviewModeTextBox.ReadOnly = true;
            this.RollPreviewModeTextBox.Size = new System.Drawing.Size(101, 20);
            this.RollPreviewModeTextBox.TabIndex = 23;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 111);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(96, 13);
            this.label20.TabIndex = 22;
            this.label20.Text = "Roll Preview Mode";
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.Green;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(10, 37);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(142, 17);
            this.label18.TabIndex = 23;
            this.label18.Text = "Green: Accepted";
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.White;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(10, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(142, 17);
            this.label17.TabIndex = 22;
            this.label17.Text = "White: Not requested";
            // 
            // FullResolutionPreviewAcceptedLabel
            // 
            this.FullResolutionPreviewAcceptedLabel.BackColor = System.Drawing.Color.White;
            this.FullResolutionPreviewAcceptedLabel.Location = new System.Drawing.Point(210, 63);
            this.FullResolutionPreviewAcceptedLabel.Name = "FullResolutionPreviewAcceptedLabel";
            this.FullResolutionPreviewAcceptedLabel.Size = new System.Drawing.Size(10, 13);
            this.FullResolutionPreviewAcceptedLabel.TabIndex = 19;
            // 
            // AcquireFlatObjectOnRollAreaAcceptedLabel
            // 
            this.AcquireFlatObjectOnRollAreaAcceptedLabel.BackColor = System.Drawing.Color.White;
            this.AcquireFlatObjectOnRollAreaAcceptedLabel.Location = new System.Drawing.Point(169, 86);
            this.AcquireFlatObjectOnRollAreaAcceptedLabel.Name = "AcquireFlatObjectOnRollAreaAcceptedLabel";
            this.AcquireFlatObjectOnRollAreaAcceptedLabel.Size = new System.Drawing.Size(10, 13);
            this.AcquireFlatObjectOnRollAreaAcceptedLabel.TabIndex = 18;
            // 
            // HLPCompletenessTextBox
            // 
            this.HLPCompletenessTextBox.Location = new System.Drawing.Point(113, 295);
            this.HLPCompletenessTextBox.Name = "HLPCompletenessTextBox";
            this.HLPCompletenessTextBox.ReadOnly = true;
            this.HLPCompletenessTextBox.Size = new System.Drawing.Size(100, 20);
            this.HLPCompletenessTextBox.TabIndex = 21;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 298);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(97, 13);
            this.label16.TabIndex = 20;
            this.label16.Text = "HLP Completeness";
            // 
            // AcceptImageButton
            // 
            this.AcceptImageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AcceptImageButton.Location = new System.Drawing.Point(860, 96);
            this.AcceptImageButton.Name = "AcceptImageButton";
            this.AcceptImageButton.Size = new System.Drawing.Size(235, 33);
            this.AcceptImageButton.TabIndex = 18;
            this.AcceptImageButton.Text = "ACCEPT";
            this.AcceptImageButton.UseVisualStyleBackColor = true;
            this.AcceptImageButton.Click += new System.EventHandler(this.AcceptImageButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Location = new System.Drawing.Point(7, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(171, 37);
            this.panel1.TabIndex = 25;
            // 
            // ObjectNameTextBox
            // 
            this.ObjectNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectNameTextBox.Location = new System.Drawing.Point(860, 21);
            this.ObjectNameTextBox.Name = "ObjectNameTextBox";
            this.ObjectNameTextBox.Size = new System.Drawing.Size(235, 30);
            this.ObjectNameTextBox.TabIndex = 17;
            this.ObjectNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RollPreviewModeTextBox);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.FullResolutionPreviewAcceptedLabel);
            this.groupBox2.Controls.Add(this.AcquireFlatObjectOnRollAreaAcceptedLabel);
            this.groupBox2.Controls.Add(this.AutoCaptureAccettedLabel);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(860, 188);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 144);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Acquisition Options";
            // 
            // AutoCaptureAccettedLabel
            // 
            this.AutoCaptureAccettedLabel.BackColor = System.Drawing.Color.White;
            this.AutoCaptureAccettedLabel.Location = new System.Drawing.Point(81, 64);
            this.AutoCaptureAccettedLabel.Name = "AutoCaptureAccettedLabel";
            this.AutoCaptureAccettedLabel.Size = new System.Drawing.Size(10, 13);
            this.AutoCaptureAccettedLabel.TabIndex = 17;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(160, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Acquire Flat Object On Roll Area";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(109, 63);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Full Res in Preview";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Auto-Capture";
            // 
            // ArtefactsSizeTextBox
            // 
            this.ArtefactsSizeTextBox.Location = new System.Drawing.Point(112, 325);
            this.ArtefactsSizeTextBox.Name = "ArtefactsSizeTextBox";
            this.ArtefactsSizeTextBox.ReadOnly = true;
            this.ArtefactsSizeTextBox.Size = new System.Drawing.Size(100, 20);
            this.ArtefactsSizeTextBox.TabIndex = 19;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 328);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "Artefacts Size";
            // 
            // SizeTextBox
            // 
            this.SizeTextBox.Location = new System.Drawing.Point(113, 260);
            this.SizeTextBox.Name = "SizeTextBox";
            this.SizeTextBox.ReadOnly = true;
            this.SizeTextBox.Size = new System.Drawing.Size(100, 20);
            this.SizeTextBox.TabIndex = 17;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 263);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(27, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Size";
            // 
            // ContrastTextBox
            // 
            this.ContrastTextBox.Location = new System.Drawing.Point(113, 227);
            this.ContrastTextBox.Name = "ContrastTextBox";
            this.ContrastTextBox.ReadOnly = true;
            this.ContrastTextBox.Size = new System.Drawing.Size(100, 20);
            this.ContrastTextBox.TabIndex = 15;
            // 
            // ImSYTextBox
            // 
            this.ImSYTextBox.Location = new System.Drawing.Point(113, 105);
            this.ImSYTextBox.Name = "ImSYTextBox";
            this.ImSYTextBox.ReadOnly = true;
            this.ImSYTextBox.Size = new System.Drawing.Size(100, 20);
            this.ImSYTextBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Image Size Y";
            // 
            // StartStopButton
            // 
            this.StartStopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartStopButton.Location = new System.Drawing.Point(860, 58);
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(235, 32);
            this.StartStopButton.TabIndex = 12;
            this.StartStopButton.Text = "START";
            this.StartStopButton.UseVisualStyleBackColor = true;
            this.StartStopButton.Click += new System.EventHandler(this.StartStopButton_Click);
            // 
            // AcquisitionManagementGeneralTimer
            // 
            this.AcquisitionManagementGeneralTimer.Interval = 5;
            this.AcquisitionManagementGeneralTimer.Tick += new System.EventHandler(this.AcquisitionManagementGeneralTimer_Tick);
            // 
            // DiagnosticMessageListBox
            // 
            this.DiagnosticMessageListBox.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DiagnosticMessageListBox.ForeColor = System.Drawing.Color.Red;
            this.DiagnosticMessageListBox.FormattingEnabled = true;
            this.DiagnosticMessageListBox.ItemHeight = 18;
            this.DiagnosticMessageListBox.Location = new System.Drawing.Point(609, 540);
            this.DiagnosticMessageListBox.Name = "DiagnosticMessageListBox";
            this.DiagnosticMessageListBox.Size = new System.Drawing.Size(486, 76);
            this.DiagnosticMessageListBox.TabIndex = 11;
            // 
            // ImSXTextBox
            // 
            this.ImSXTextBox.Location = new System.Drawing.Point(113, 76);
            this.ImSXTextBox.Name = "ImSXTextBox";
            this.ImSXTextBox.ReadOnly = true;
            this.ImSXTextBox.Size = new System.Drawing.Size(100, 20);
            this.ImSXTextBox.TabIndex = 5;
            // 
            // AcquiredImagePictureBox
            // 
            this.AcquiredImagePictureBox.BackColor = System.Drawing.Color.White;
            this.AcquiredImagePictureBox.Location = new System.Drawing.Point(3, 20);
            this.AcquiredImagePictureBox.Name = "AcquiredImagePictureBox";
            this.AcquiredImagePictureBox.Size = new System.Drawing.Size(600, 600);
            this.AcquiredImagePictureBox.TabIndex = 10;
            this.AcquiredImagePictureBox.TabStop = false;
            this.AcquiredImagePictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.AcquiredImagePictureBox_Paint);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Image Size X";
            // 
            // StopTypeTextBox
            // 
            this.StopTypeTextBox.Location = new System.Drawing.Point(113, 49);
            this.StopTypeTextBox.Name = "StopTypeTextBox";
            this.StopTypeTextBox.ReadOnly = true;
            this.StopTypeTextBox.Size = new System.Drawing.Size(100, 20);
            this.StopTypeTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Stop Type";
            // 
            // ImageResTextBox
            // 
            this.ImageResTextBox.Location = new System.Drawing.Point(113, 20);
            this.ImageResTextBox.Name = "ImageResTextBox";
            this.ImageResTextBox.ReadOnly = true;
            this.ImageResTextBox.Size = new System.Drawing.Size(100, 20);
            this.ImageResTextBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbRolledFlatRatio);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.tbDryAreaPercent);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.tbWetAreaPercent);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.AcquiredFramesNumTextBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.LostFramesNumTextBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.HLPCompletenessTextBox);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.ArtefactsSizeTextBox);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.SizeTextBox);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.ContrastTextBox);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.AutoCapturePhaseTextBox);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.NominalFrameRateTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.CurrentFrameRateTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.ImSYTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ImSXTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.StopTypeTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ImageResTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(609, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 513);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image Info";
            // 
            // tbRolledFlatRatio
            // 
            this.tbRolledFlatRatio.Location = new System.Drawing.Point(112, 476);
            this.tbRolledFlatRatio.Name = "tbRolledFlatRatio";
            this.tbRolledFlatRatio.ReadOnly = true;
            this.tbRolledFlatRatio.Size = new System.Drawing.Size(100, 20);
            this.tbRolledFlatRatio.TabIndex = 37;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 479);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(82, 13);
            this.label22.TabIndex = 36;
            this.label22.Text = "Rolled/Flat ratio";
            // 
            // tbDryAreaPercent
            // 
            this.tbDryAreaPercent.Location = new System.Drawing.Point(113, 415);
            this.tbDryAreaPercent.Name = "tbDryAreaPercent";
            this.tbDryAreaPercent.ReadOnly = true;
            this.tbDryAreaPercent.Size = new System.Drawing.Size(100, 20);
            this.tbDryAreaPercent.TabIndex = 29;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 418);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 13);
            this.label19.TabIndex = 28;
            this.label19.Text = "Dry Area Percent";
            // 
            // tbWetAreaPercent
            // 
            this.tbWetAreaPercent.Location = new System.Drawing.Point(112, 445);
            this.tbWetAreaPercent.Name = "tbWetAreaPercent";
            this.tbWetAreaPercent.ReadOnly = true;
            this.tbWetAreaPercent.Size = new System.Drawing.Size(100, 20);
            this.tbWetAreaPercent.TabIndex = 27;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 448);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(92, 13);
            this.label21.TabIndex = 26;
            this.label21.Text = "Wet Area Percent";
            // 
            // AcquiredFramesNumTextBox
            // 
            this.AcquiredFramesNumTextBox.Location = new System.Drawing.Point(113, 354);
            this.AcquiredFramesNumTextBox.Name = "AcquiredFramesNumTextBox";
            this.AcquiredFramesNumTextBox.ReadOnly = true;
            this.AcquiredFramesNumTextBox.Size = new System.Drawing.Size(100, 20);
            this.AcquiredFramesNumTextBox.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 357);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Acquired Frames";
            // 
            // LostFramesNumTextBox
            // 
            this.LostFramesNumTextBox.Location = new System.Drawing.Point(112, 384);
            this.LostFramesNumTextBox.Name = "LostFramesNumTextBox";
            this.LostFramesNumTextBox.ReadOnly = true;
            this.LostFramesNumTextBox.Size = new System.Drawing.Size(100, 20);
            this.LostFramesNumTextBox.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 387);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Lost Frames";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Resolution";
            // 
            // PreviewLabel
            // 
            this.PreviewLabel.AutoSize = true;
            this.PreviewLabel.BackColor = System.Drawing.Color.White;
            this.PreviewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreviewLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.PreviewLabel.Location = new System.Drawing.Point(23, 35);
            this.PreviewLabel.Name = "PreviewLabel";
            this.PreviewLabel.Size = new System.Drawing.Size(128, 36);
            this.PreviewLabel.TabIndex = 14;
            this.PreviewLabel.Text = "Preview";
            this.PreviewLabel.Visible = false;
            // 
            // WaitFirstFrameLabel
            // 
            this.WaitFirstFrameLabel.AutoSize = true;
            this.WaitFirstFrameLabel.BackColor = System.Drawing.Color.White;
            this.WaitFirstFrameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaitFirstFrameLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.WaitFirstFrameLabel.Location = new System.Drawing.Point(245, 321);
            this.WaitFirstFrameLabel.Name = "WaitFirstFrameLabel";
            this.WaitFirstFrameLabel.Size = new System.Drawing.Size(233, 36);
            this.WaitFirstFrameLabel.TabIndex = 13;
            this.WaitFirstFrameLabel.Text = "Don\'t put finger";
            this.WaitFirstFrameLabel.Visible = false;
            // 
            // cbExcludeFinalization
            // 
            this.cbExcludeFinalization.AutoSize = true;
            this.cbExcludeFinalization.Location = new System.Drawing.Point(869, 340);
            this.cbExcludeFinalization.Name = "cbExcludeFinalization";
            this.cbExcludeFinalization.Size = new System.Drawing.Size(119, 17);
            this.cbExcludeFinalization.TabIndex = 19;
            this.cbExcludeFinalization.Text = "Exclude Finalization";
            this.cbExcludeFinalization.UseVisualStyleBackColor = true;
            this.cbExcludeFinalization.CheckedChanged += new System.EventHandler(this.cbExcludeFinalization_CheckedChanged);
            // 
            // bUpdateBackgroundImage
            // 
            this.bUpdateBackgroundImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bUpdateBackgroundImage.Location = new System.Drawing.Point(860, 137);
            this.bUpdateBackgroundImage.Name = "bUpdateBackgroundImage";
            this.bUpdateBackgroundImage.Size = new System.Drawing.Size(235, 45);
            this.bUpdateBackgroundImage.TabIndex = 20;
            this.bUpdateBackgroundImage.Text = "UPDATE BACKGROUND IMAGE";
            this.bUpdateBackgroundImage.UseVisualStyleBackColor = true;
            this.bUpdateBackgroundImage.Click += new System.EventHandler(this.bUpdateBackgroundImage_Click);
            // 
            // AcquisitionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 637);
            this.Controls.Add(this.bUpdateBackgroundImage);
            this.Controls.Add(this.cbExcludeFinalization);
            this.Controls.Add(this.AcceptImageButton);
            this.Controls.Add(this.ObjectNameTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.StartStopButton);
            this.Controls.Add(this.DiagnosticMessageListBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.PreviewLabel);
            this.Controls.Add(this.WaitFirstFrameLabel);
            this.Controls.Add(this.AcquiredImagePictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AcquisitionForm";
            this.Text = "Fingerprint Image Acquisition";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AcquisitionForm_FormClosed);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AcquiredImagePictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox AutoCapturePhaseTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox NominalFrameRateTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox CurrentFrameRateTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox RollPreviewModeTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label FullResolutionPreviewAcceptedLabel;
        private System.Windows.Forms.Label AcquireFlatObjectOnRollAreaAcceptedLabel;
        private System.Windows.Forms.TextBox HLPCompletenessTextBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button AcceptImageButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ObjectNameTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label AutoCaptureAccettedLabel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ArtefactsSizeTextBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox SizeTextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox ContrastTextBox;
        private System.Windows.Forms.TextBox ImSYTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button StartStopButton;
        private System.Windows.Forms.Timer AcquisitionManagementGeneralTimer;
        private System.Windows.Forms.ListBox DiagnosticMessageListBox;
        private System.Windows.Forms.TextBox ImSXTextBox;
        private System.Windows.Forms.PictureBox AcquiredImagePictureBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox StopTypeTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ImageResTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label PreviewLabel;
        private System.Windows.Forms.Label WaitFirstFrameLabel;
        private System.Windows.Forms.TextBox AcquiredFramesNumTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox LostFramesNumTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbExcludeFinalization;
        private System.Windows.Forms.Button bUpdateBackgroundImage;
        private System.Windows.Forms.TextBox tbDryAreaPercent;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbWetAreaPercent;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tbRolledFlatRatio;
        private System.Windows.Forms.Label label22;
    }
}