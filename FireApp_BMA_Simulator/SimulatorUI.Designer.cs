namespace FireApp_BMA_Simulator
{
    partial class SimulatorUI
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
            this.cmdGenerateEvent = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdGenerateEvent
            // 
            this.cmdGenerateEvent.Location = new System.Drawing.Point(12, 12);
            this.cmdGenerateEvent.Name = "cmdGenerateEvent";
            this.cmdGenerateEvent.Size = new System.Drawing.Size(692, 170);
            this.cmdGenerateEvent.TabIndex = 0;
            this.cmdGenerateEvent.Text = "generate Event";
            this.cmdGenerateEvent.UseVisualStyleBackColor = true;
            this.cmdGenerateEvent.Click += new System.EventHandler(this.cmdGenerateEvent_Click);
            // 
            // SimulatorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 194);
            this.Controls.Add(this.cmdGenerateEvent);
            this.Name = "SimulatorUI";
            this.Text = "SimulatorUI";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdGenerateEvent;
    }
}