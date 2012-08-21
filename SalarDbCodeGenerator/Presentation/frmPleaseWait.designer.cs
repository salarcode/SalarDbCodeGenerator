namespace SalarDbCodeGenerator
{
    partial class frmPleaseWait
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
			this.tmrProgress = new System.Windows.Forms.Timer(this.components);
			this.lblMessage = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlContainer = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.barProgress = new System.Windows.Forms.ProgressBar();
			this.pnlContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// tmrProgress
			// 
			this.tmrProgress.Enabled = true;
			this.tmrProgress.Tick += new System.EventHandler(this.tmrProgress_Tick);
			// 
			// lblMessage
			// 
			this.lblMessage.BackColor = System.Drawing.Color.Transparent;
			this.lblMessage.Location = new System.Drawing.Point(9, 29);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(311, 15);
			this.lblMessage.TabIndex = 11;
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(128, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Please wait...";
			// 
			// pnlContainer
			// 
			this.pnlContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlContainer.Controls.Add(this.btnCancel);
			this.pnlContainer.Controls.Add(this.barProgress);
			this.pnlContainer.Controls.Add(this.lblMessage);
			this.pnlContainer.Controls.Add(this.label1);
			this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlContainer.Location = new System.Drawing.Point(0, 0);
			this.pnlContainer.Name = "pnlContainer";
			this.pnlContainer.Size = new System.Drawing.Size(328, 90);
			this.pnlContainer.TabIndex = 14;
			this.pnlContainer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlContainer_MouseMove);
			this.pnlContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlContainer_MouseDown);
			this.pnlContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlContainer_MouseUp);
			// 
			// btnCancel
			// 
			this.btnCancel.Enabled = false;
			this.btnCancel.Location = new System.Drawing.Point(126, 104);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 26);
			this.btnCancel.TabIndex = 15;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// barProgress
			// 
			this.barProgress.Location = new System.Drawing.Point(13, 49);
			this.barProgress.Maximum = 50;
			this.barProgress.Name = "barProgress";
			this.barProgress.Size = new System.Drawing.Size(303, 26);
			this.barProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.barProgress.TabIndex = 14;
			// 
			// frmPleaseWait
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(328, 90);
			this.ControlBox = false;
			this.Controls.Add(this.pnlContainer);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPleaseWait";
			this.TopMost = true;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Please wait...";
			this.Load += new System.EventHandler(this.frmWait_Load);
			this.pnlContainer.ResumeLayout(false);
			this.pnlContainer.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrProgress;
        internal System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.ProgressBar barProgress;
		internal System.Windows.Forms.Button btnCancel;
    }
}