namespace VideoSender
{
    partial class Form1
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
            this.cboDevices = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.progBuffer = new System.Windows.Forms.ProgressBar();
            this.progSend = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // cboDevices
            // 
            this.cboDevices.FormattingEnabled = true;
            this.cboDevices.Location = new System.Drawing.Point(12, 12);
            this.cboDevices.Name = "cboDevices";
            this.cboDevices.Size = new System.Drawing.Size(243, 21);
            this.cboDevices.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(261, 10);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(29, 62);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(320, 240);
            this.pbImage.TabIndex = 2;
            this.pbImage.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(342, 10);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // progBuffer
            // 
            this.progBuffer.Location = new System.Drawing.Point(29, 308);
            this.progBuffer.Name = "progBuffer";
            this.progBuffer.Size = new System.Drawing.Size(320, 15);
            this.progBuffer.TabIndex = 5;
            // 
            // progSend
            // 
            this.progSend.Location = new System.Drawing.Point(29, 329);
            this.progSend.Name = "progSend";
            this.progSend.Size = new System.Drawing.Size(320, 15);
            this.progSend.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 462);
            this.Controls.Add(this.progSend);
            this.Controls.Add(this.progBuffer);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.cboDevices);
            this.Name = "Form1";
            this.Text = "Video Send";
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboDevices;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar progBuffer;
        private System.Windows.Forms.ProgressBar progSend;
    }
}

