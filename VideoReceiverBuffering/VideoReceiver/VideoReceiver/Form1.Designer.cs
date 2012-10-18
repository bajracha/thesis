namespace VideoReceiver
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
            this.btnStart = new System.Windows.Forms.Button();
            this.pbFrame = new System.Windows.Forms.PictureBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.progDisplay = new System.Windows.Forms.ProgressBar();
            this.progBuffer = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pbFrame
            // 
            this.pbFrame.Location = new System.Drawing.Point(12, 60);
            this.pbFrame.Name = "pbFrame";
            this.pbFrame.Size = new System.Drawing.Size(320, 240);
            this.pbFrame.TabIndex = 1;
            this.pbFrame.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(103, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // progDisplay
            // 
            this.progDisplay.ForeColor = System.Drawing.Color.ForestGreen;
            this.progDisplay.Location = new System.Drawing.Point(12, 306);
            this.progDisplay.Name = "progDisplay";
            this.progDisplay.Size = new System.Drawing.Size(319, 17);
            this.progDisplay.TabIndex = 3;
            // 
            // progBuffer
            // 
            this.progBuffer.ForeColor = System.Drawing.Color.Gray;
            this.progBuffer.Location = new System.Drawing.Point(12, 329);
            this.progBuffer.Name = "progBuffer";
            this.progBuffer.Size = new System.Drawing.Size(319, 17);
            this.progBuffer.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.progBuffer);
            this.Controls.Add(this.progDisplay);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.pbFrame);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Video Receive";
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pbFrame;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar progDisplay;
        private System.Windows.Forms.ProgressBar progBuffer;
    }
}

