namespace WinUsbExample
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
			this.txtGuid = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.lstDevices = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// txtGuid
			// 
			this.txtGuid.Location = new System.Drawing.Point(12, 25);
			this.txtGuid.Name = "txtGuid";
			this.txtGuid.Size = new System.Drawing.Size(279, 20);
			this.txtGuid.TabIndex = 9;
			this.txtGuid.TextChanged += new System.EventHandler(this.txtGuid_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(9, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(99, 13);
			this.label8.TabIndex = 10;
			this.label8.Text = "USB Device GUID:";
			// 
			// lstDevices
			// 
			this.lstDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstDevices.FormattingEnabled = true;
			this.lstDevices.Location = new System.Drawing.Point(12, 51);
			this.lstDevices.Name = "lstDevices";
			this.lstDevices.Size = new System.Drawing.Size(279, 121);
			this.lstDevices.TabIndex = 11;
			this.lstDevices.DoubleClick += new System.EventHandler(this.lstDevices_DoubleClick);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(303, 186);
			this.Controls.Add(this.lstDevices);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtGuid);
			this.Name = "Form1";
			this.Text = "WinUSB Example";
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtGuid;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ListBox lstDevices;
	}
}

