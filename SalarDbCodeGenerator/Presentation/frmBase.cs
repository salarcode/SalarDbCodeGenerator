using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SalarDbCodeGenerator.Presentation
{
	public partial class frmBase : Form
	{
        public frmBase()
		{
			InitializeComponent();
		}
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// frmBase
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::SalarDbCodeGenerator.Properties.Resources.AppIcon;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmBase";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}
		protected virtual bool ValidateForm()
		{
			return this.Validate();
		}

	}
}
