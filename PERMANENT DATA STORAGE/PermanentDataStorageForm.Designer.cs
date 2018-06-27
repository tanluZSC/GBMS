namespace GBMSAPI_CS_Example.PERMANENT_DATA_STORAGE
{
    partial class PermanentDataStorageForm
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
            this.StorageSizeLabel = new System.Windows.Forms.Label();
            this.ReadButton = new System.Windows.Forms.Button();
            this.WriteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StorageSizeLabel
            // 
            this.StorageSizeLabel.AutoSize = true;
            this.StorageSizeLabel.Location = new System.Drawing.Point(13, 145);
            this.StorageSizeLabel.Name = "StorageSizeLabel";
            this.StorageSizeLabel.Size = new System.Drawing.Size(0, 13);
            this.StorageSizeLabel.TabIndex = 5;
            // 
            // ReadButton
            // 
            this.ReadButton.Location = new System.Drawing.Point(12, 80);
            this.ReadButton.Name = "ReadButton";
            this.ReadButton.Size = new System.Drawing.Size(181, 39);
            this.ReadButton.TabIndex = 4;
            this.ReadButton.Text = "Read To File";
            this.ReadButton.UseVisualStyleBackColor = true;
            this.ReadButton.Click += new System.EventHandler(this.ReadButton_Click);
            // 
            // WriteButton
            // 
            this.WriteButton.Location = new System.Drawing.Point(12, 12);
            this.WriteButton.Name = "WriteButton";
            this.WriteButton.Size = new System.Drawing.Size(181, 39);
            this.WriteButton.TabIndex = 3;
            this.WriteButton.Text = "Write From File";
            this.WriteButton.UseVisualStyleBackColor = true;
            this.WriteButton.Click += new System.EventHandler(this.WriteButton_Click);
            // 
            // PermanentDataStorageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 176);
            this.Controls.Add(this.StorageSizeLabel);
            this.Controls.Add(this.ReadButton);
            this.Controls.Add(this.WriteButton);
            this.Name = "PermanentDataStorageForm";
            this.Text = "PermanentDataStorageForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StorageSizeLabel;
        private System.Windows.Forms.Button ReadButton;
        private System.Windows.Forms.Button WriteButton;
    }
}