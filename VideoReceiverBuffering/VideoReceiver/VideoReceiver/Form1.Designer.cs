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
            this.dayPicker = new System.Windows.Forms.DateTimePicker();
            this.day = new System.Windows.Forms.Label();
            this.hour = new System.Windows.Forms.Label();
            this.min = new System.Windows.Forms.Label();
            this.hourCombo = new System.Windows.Forms.ComboBox();
            this.mincombo = new System.Windows.Forms.ComboBox();
            this.datetimebutton = new System.Windows.Forms.Button();
            this.stopbutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(344, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pbFrame
            // 
            this.pbFrame.Location = new System.Drawing.Point(344, 60);
            this.pbFrame.Name = "pbFrame";
            this.pbFrame.Size = new System.Drawing.Size(320, 240);
            this.pbFrame.TabIndex = 1;
            this.pbFrame.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(435, 12);
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
            this.progDisplay.Location = new System.Drawing.Point(344, 306);
            this.progDisplay.Name = "progDisplay";
            this.progDisplay.Size = new System.Drawing.Size(319, 17);
            this.progDisplay.TabIndex = 3;
            // 
            // progBuffer
            // 
            this.progBuffer.ForeColor = System.Drawing.Color.Gray;
            this.progBuffer.Location = new System.Drawing.Point(344, 329);
            this.progBuffer.Name = "progBuffer";
            this.progBuffer.Size = new System.Drawing.Size(319, 17);
            this.progBuffer.TabIndex = 4;
            // 
            // dayPicker
            // 
            this.dayPicker.Location = new System.Drawing.Point(52, 60);
            this.dayPicker.Name = "dayPicker";
            this.dayPicker.Size = new System.Drawing.Size(200, 20);
            this.dayPicker.TabIndex = 5;
            // 
            // day
            // 
            this.day.AutoSize = true;
            this.day.Location = new System.Drawing.Point(13, 62);
            this.day.Name = "day";
            this.day.Size = new System.Drawing.Size(26, 13);
            this.day.TabIndex = 6;
            this.day.Text = "Day";
            this.day.Click += new System.EventHandler(this.label1_Click);
            // 
            // hour
            // 
            this.hour.AutoSize = true;
            this.hour.Location = new System.Drawing.Point(13, 89);
            this.hour.Name = "hour";
            this.hour.Size = new System.Drawing.Size(30, 13);
            this.hour.TabIndex = 7;
            this.hour.Text = "Hour";
            // 
            // min
            // 
            this.min.AutoSize = true;
            this.min.Location = new System.Drawing.Point(16, 115);
            this.min.Name = "min";
            this.min.Size = new System.Drawing.Size(24, 13);
            this.min.TabIndex = 8;
            this.min.Text = "Min";
            // 
            // hourCombo
            // 
            this.hourCombo.FormattingEnabled = true;
            this.hourCombo.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.hourCombo.Location = new System.Drawing.Point(52, 89);
            this.hourCombo.Name = "hourCombo";
            this.hourCombo.Size = new System.Drawing.Size(121, 21);
            this.hourCombo.TabIndex = 9;
            this.hourCombo.Text = "0";
            // 
            // mincombo
            // 
            this.mincombo.FormattingEnabled = true;
            this.mincombo.Items.AddRange(new object[] {
            "0",
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.mincombo.Location = new System.Drawing.Point(52, 115);
            this.mincombo.Name = "mincombo";
            this.mincombo.Size = new System.Drawing.Size(121, 21);
            this.mincombo.TabIndex = 10;
            this.mincombo.Text = "0";
            // 
            // datetimebutton
            // 
            this.datetimebutton.Location = new System.Drawing.Point(16, 149);
            this.datetimebutton.Name = "datetimebutton";
            this.datetimebutton.Size = new System.Drawing.Size(75, 23);
            this.datetimebutton.TabIndex = 11;
            this.datetimebutton.Text = "Play";
            this.datetimebutton.UseVisualStyleBackColor = true;
            this.datetimebutton.Click += new System.EventHandler(this.datetimebutton_Click);
            // 
            // stopbutton
            // 
            this.stopbutton.Location = new System.Drawing.Point(105, 149);
            this.stopbutton.Name = "stopbutton";
            this.stopbutton.Size = new System.Drawing.Size(75, 23);
            this.stopbutton.TabIndex = 12;
            this.stopbutton.Text = "Stop";
            this.stopbutton.UseVisualStyleBackColor = true;
            this.stopbutton.Click += new System.EventHandler(this.stopbutton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.stopbutton);
            this.Controls.Add(this.datetimebutton);
            this.Controls.Add(this.mincombo);
            this.Controls.Add(this.hourCombo);
            this.Controls.Add(this.min);
            this.Controls.Add(this.hour);
            this.Controls.Add(this.day);
            this.Controls.Add(this.dayPicker);
            this.Controls.Add(this.progBuffer);
            this.Controls.Add(this.progDisplay);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.pbFrame);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Video Receive";
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pbFrame;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar progDisplay;
        private System.Windows.Forms.ProgressBar progBuffer;
        private System.Windows.Forms.DateTimePicker dayPicker;
        private System.Windows.Forms.Label day;
        private System.Windows.Forms.Label hour;
        private System.Windows.Forms.Label min;
        private System.Windows.Forms.ComboBox hourCombo;
        private System.Windows.Forms.ComboBox mincombo;
        private System.Windows.Forms.Button datetimebutton;
        private System.Windows.Forms.Button stopbutton;
    }
}

