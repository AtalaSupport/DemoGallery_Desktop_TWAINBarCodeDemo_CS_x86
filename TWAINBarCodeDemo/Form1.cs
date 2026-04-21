using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace TWAINBarCodeDemo
{
	/// <summary>
	/// This simple Winform will use the BatchScan class to demonstrate scanning and saving 
	/// muliple documents, while reading barcode encoded instructions.  By default, all the 
	/// directories have the base path of "C:\TWAINoutput", and the file names have the format
	/// "untitled_x.tif".  This is only changed when a correct barcode is recognized.  This
	/// demo program currently does not account for barcodes that are incorrectly formated.
	/// 
	/// Requirements:
	/// -TWAIN supported scanner with an Automatic Document Feeder (ADF).
	/// -at least one page with barcodes encoded with Symbology Code39 barcode font.
	/// -at least one page to be scanned in after the barcodded page (cover sheet).
	/// 
	/// Note:  This class does not use any dotTWAIN or DotImage classes directly.  Thoes are all
	/// used in the BatchScan Class.  This class is simply to show the class running.
	/// 
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button button_startscan;
		private System.Windows.Forms.CheckBox autodeskew;
		private System.Windows.Forms.CheckBox autodespeckle;
		private System.Windows.Forms.TextBox statusBox;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		
		private BatchScan bs = new BatchScan();
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			// register an Event for message printing
			bs.ScanMsg += new TWAINBarCodeDemo.BatchScan.BatchScanEventHandler(this.OnScanMsg);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.button_startscan = new System.Windows.Forms.Button();
			this.autodeskew = new System.Windows.Forms.CheckBox();
			this.autodespeckle = new System.Windows.Forms.CheckBox();
			this.statusBox = new System.Windows.Forms.TextBox();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// button_startscan
			// 
			this.button_startscan.Location = new System.Drawing.Point(104, 80);
			this.button_startscan.Name = "button_startscan";
			this.button_startscan.Size = new System.Drawing.Size(104, 23);
			this.button_startscan.TabIndex = 0;
			this.button_startscan.Text = "Start Batch Scan";
			this.button_startscan.Click += new System.EventHandler(this.OnStartScan);
			// 
			// autodeskew
			// 
			this.autodeskew.Location = new System.Drawing.Point(48, 32);
			this.autodeskew.Name = "autodeskew";
			this.autodeskew.TabIndex = 1;
			this.autodeskew.Text = "Auto Deskew";
			// 
			// autodespeckle
			// 
			this.autodespeckle.Location = new System.Drawing.Point(184, 32);
			this.autodespeckle.Name = "autodespeckle";
			this.autodespeckle.TabIndex = 2;
			this.autodespeckle.Text = "Auto Despeckle";
			// 
			// statusBox
			// 
			this.statusBox.Location = new System.Drawing.Point(8, 120);
			this.statusBox.Multiline = true;
			this.statusBox.Name = "statusBox";
			this.statusBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.statusBox.Size = new System.Drawing.Size(328, 136);
			this.statusBox.TabIndex = 3;
			this.statusBox.Text = "";
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "Help";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "About";
			this.menuItem2.Click += new System.EventHandler(this.OnHelpSelect);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(344, 266);
			this.Controls.Add(this.statusBox);
			this.Controls.Add(this.autodespeckle);
			this.Controls.Add(this.autodeskew);
			this.Controls.Add(this.button_startscan);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "My TWAIN Demo";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
		
		// EVENT - Start Scan Button Click
		private void OnStartScan(object sender, System.EventArgs e)
		{
			// assign comands to preform to images
			bs.Deskew = autodeskew.Checked;
			bs.Despeckle = autodespeckle.Checked;
			
			try
			{
				bs.Process();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		// EVENT - message comming from BatchScan object, display in the textbox.
		private void OnScanMsg(object sender, BatchScan.BatchScanEventArgs e)
		{
			statusBox.AppendText(e.msg + "\r\n");
		}
		
		// A little bit of info
		private void OnHelpSelect(object sender, System.EventArgs e)
		{
			AtalaDemos.AboutBox.About aboutBox = new AtalaDemos.AboutBox.About("About Atalasoft Barcode Sorter Demo",
				"DotImage and DotTWAIN Barcode Demo");
			aboutBox.Description = @"This demo shows how to scan multiple pages (using DotTwain) and file them based on a barcode.  Each group of pages have their own cover sheet with a barcode that holds information about where to save the following scanned images.  Please see the Readme file included in the project for more information on this demo.  Requires DotImage, DotImage BarcodeReader Code39, and DotTwain.";
			aboutBox.ShowDialog();

		}
	}
}
