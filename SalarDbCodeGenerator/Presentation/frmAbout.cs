using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SalarDbCodeGenerator.Presentation
{
	public partial class frmAbout : frmBase
	{
		public frmAbout()
		{
			InitializeComponent();
		}

		private void lnkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo start = new ProcessStartInfo("mailto:" + lnkEmail.Text);
			try
			{
				start.UseShellExecute = true;
				Process.Start(start);
			}
			catch { }
		}

		private void lnkWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo start = new ProcessStartInfo(lnkWebSite.Text);
			try
			{
				start.UseShellExecute = true;
				Process.Start(start);
			}
			catch { }
		}

		private void frmAbout_Load(object sender, EventArgs e)
		{
			lblVersion.Text = SalarDbCodeGenerator.DbProject.AppConfig.AppVersionFull;
		}

		private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo start = new ProcessStartInfo(lnkUpdate.Text);
			try
			{
				start.UseShellExecute = true;
				Process.Start(start);
			}
			catch { }

		}
	}
}
