namespace WinUsbTest
{
	partial class MainForm
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
			this.txtLength = new System.Windows.Forms.TextBox();
			this.txtIndex = new System.Windows.Forms.TextBox();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.lblLength = new System.Windows.Forms.Label();
			this.txtRequest = new System.Windows.Forms.TextBox();
			this.lblIndex = new System.Windows.Forms.Label();
			this.lblValue = new System.Windows.Forms.Label();
			this.lblRequest = new System.Windows.Forms.Label();
			this.txtType = new System.Windows.Forms.TextBox();
			this.lblType = new System.Windows.Forms.Label();
			this.lblOutDataList = new System.Windows.Forms.Label();
			this.txtOutData = new System.Windows.Forms.TextBox();
			this.drpEndpoint = new System.Windows.Forms.ComboBox();
			this.lblEndpoint = new System.Windows.Forms.Label();
			this.lblInData = new System.Windows.Forms.Label();
			this.lblOutData = new System.Windows.Forms.Label();
			this.txtLog = new System.Windows.Forms.RichTextBox();
			this.btnXfer = new System.Windows.Forms.Button();
			this.txtGuid = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.grpEndpoint = new System.Windows.Forms.GroupBox();
			this.txtReadTimeout = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.lblReadTimeout = new System.Windows.Forms.Label();
			this.chkWriteUseShortPacket = new System.Windows.Forms.CheckBox();
			this.chkReadUseShortPacket = new System.Windows.Forms.CheckBox();
			this.grpSetup = new System.Windows.Forms.GroupBox();
			this.btnFind = new System.Windows.Forms.Button();
			this.lblAttach = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.radOut = new System.Windows.Forms.RadioButton();
			this.radIn = new System.Windows.Forms.RadioButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.radText = new System.Windows.Forms.RadioButton();
			this.radHex = new System.Windows.Forms.RadioButton();
			this.grpEndpoint.SuspendLayout();
			this.grpSetup.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtLength
			// 
			this.txtLength.Location = new System.Drawing.Point(113, 213);
			this.txtLength.Name = "txtLength";
			this.txtLength.Size = new System.Drawing.Size(30, 20);
			this.txtLength.TabIndex = 4;
			this.txtLength.Text = "0";
			this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtIndex
			// 
			this.txtIndex.Location = new System.Drawing.Point(119, 32);
			this.txtIndex.Name = "txtIndex";
			this.txtIndex.Size = new System.Drawing.Size(30, 20);
			this.txtIndex.TabIndex = 3;
			this.txtIndex.Text = "0000";
			// 
			// txtValue
			// 
			this.txtValue.Location = new System.Drawing.Point(79, 32);
			this.txtValue.Name = "txtValue";
			this.txtValue.Size = new System.Drawing.Size(30, 20);
			this.txtValue.TabIndex = 2;
			this.txtValue.Text = "0000";
			// 
			// lblLength
			// 
			this.lblLength.AutoSize = true;
			this.lblLength.Location = new System.Drawing.Point(64, 216);
			this.lblLength.Name = "lblLength";
			this.lblLength.Size = new System.Drawing.Size(43, 13);
			this.lblLength.TabIndex = 1;
			this.lblLength.Text = "Length:";
			// 
			// txtRequest
			// 
			this.txtRequest.Location = new System.Drawing.Point(46, 32);
			this.txtRequest.Name = "txtRequest";
			this.txtRequest.Size = new System.Drawing.Size(18, 20);
			this.txtRequest.TabIndex = 1;
			this.txtRequest.Text = "00";
			// 
			// lblIndex
			// 
			this.lblIndex.AutoSize = true;
			this.lblIndex.Location = new System.Drawing.Point(116, 16);
			this.lblIndex.Name = "lblIndex";
			this.lblIndex.Size = new System.Drawing.Size(33, 13);
			this.lblIndex.TabIndex = 1;
			this.lblIndex.Text = "Index";
			// 
			// lblValue
			// 
			this.lblValue.AutoSize = true;
			this.lblValue.Location = new System.Drawing.Point(76, 16);
			this.lblValue.Name = "lblValue";
			this.lblValue.Size = new System.Drawing.Size(34, 13);
			this.lblValue.TabIndex = 1;
			this.lblValue.Text = "Value";
			// 
			// lblRequest
			// 
			this.lblRequest.AutoSize = true;
			this.lblRequest.Location = new System.Drawing.Point(43, 16);
			this.lblRequest.Name = "lblRequest";
			this.lblRequest.Size = new System.Drawing.Size(27, 13);
			this.lblRequest.TabIndex = 1;
			this.lblRequest.Text = "Req";
			// 
			// txtType
			// 
			this.txtType.Location = new System.Drawing.Point(13, 32);
			this.txtType.Name = "txtType";
			this.txtType.Size = new System.Drawing.Size(18, 20);
			this.txtType.TabIndex = 0;
			this.txtType.Text = "00";
			// 
			// lblType
			// 
			this.lblType.AutoSize = true;
			this.lblType.Location = new System.Drawing.Point(6, 16);
			this.lblType.Name = "lblType";
			this.lblType.Size = new System.Drawing.Size(31, 13);
			this.lblType.TabIndex = 1;
			this.lblType.Text = "Type";
			// 
			// lblOutDataList
			// 
			this.lblOutDataList.AutoSize = true;
			this.lblOutDataList.Location = new System.Drawing.Point(62, 250);
			this.lblOutDataList.Name = "lblOutDataList";
			this.lblOutDataList.Size = new System.Drawing.Size(53, 13);
			this.lblOutDataList.TabIndex = 4;
			this.lblOutDataList.Text = "Out Data:";
			// 
			// txtOutData
			// 
			this.txtOutData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutData.Location = new System.Drawing.Point(121, 247);
			this.txtOutData.Name = "txtOutData";
			this.txtOutData.Size = new System.Drawing.Size(184, 20);
			this.txtOutData.TabIndex = 6;
			// 
			// drpEndpoint
			// 
			this.drpEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpEndpoint.FormattingEnabled = true;
			this.drpEndpoint.Location = new System.Drawing.Point(70, 53);
			this.drpEndpoint.Name = "drpEndpoint";
			this.drpEndpoint.Size = new System.Drawing.Size(78, 21);
			this.drpEndpoint.TabIndex = 2;
			this.drpEndpoint.SelectedIndexChanged += new System.EventHandler(this.drpEndpoint_SelectedIndexChanged);
			// 
			// lblEndpoint
			// 
			this.lblEndpoint.AutoSize = true;
			this.lblEndpoint.Location = new System.Drawing.Point(12, 56);
			this.lblEndpoint.Name = "lblEndpoint";
			this.lblEndpoint.Size = new System.Drawing.Size(52, 13);
			this.lblEndpoint.TabIndex = 6;
			this.lblEndpoint.Text = "Endpoint:";
			// 
			// lblInData
			// 
			this.lblInData.AutoSize = true;
			this.lblInData.ForeColor = System.Drawing.Color.Blue;
			this.lblInData.Location = new System.Drawing.Point(18, 279);
			this.lblInData.Name = "lblInData";
			this.lblInData.Size = new System.Drawing.Size(42, 13);
			this.lblInData.TabIndex = 7;
			this.lblInData.Text = "In Data";
			// 
			// lblOutData
			// 
			this.lblOutData.AutoSize = true;
			this.lblOutData.ForeColor = System.Drawing.Color.Green;
			this.lblOutData.Location = new System.Drawing.Point(70, 279);
			this.lblOutData.Name = "lblOutData";
			this.lblOutData.Size = new System.Drawing.Size(50, 13);
			this.lblOutData.TabIndex = 8;
			this.lblOutData.Text = "Out Data";
			// 
			// txtLog
			// 
			this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtLog.Location = new System.Drawing.Point(16, 295);
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.Size = new System.Drawing.Size(289, 149);
			this.txtLog.TabIndex = 7;
			this.txtLog.Text = "";
			this.txtLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtLog_MouseDoubleClick);
			// 
			// btnXfer
			// 
			this.btnXfer.AutoSize = true;
			this.btnXfer.Location = new System.Drawing.Point(12, 181);
			this.btnXfer.Name = "btnXfer";
			this.btnXfer.Size = new System.Drawing.Size(82, 23);
			this.btnXfer.TabIndex = 1;
			this.btnXfer.Text = "Transfer Data";
			this.btnXfer.UseVisualStyleBackColor = true;
			this.btnXfer.Click += new System.EventHandler(this.btnXfer_Click);
			// 
			// txtGuid
			// 
			this.txtGuid.Location = new System.Drawing.Point(12, 25);
			this.txtGuid.Name = "txtGuid";
			this.txtGuid.Size = new System.Drawing.Size(293, 20);
			this.txtGuid.TabIndex = 9;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(12, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(99, 13);
			this.label8.TabIndex = 10;
			this.label8.Text = "USB Device GUID:";
			// 
			// grpEndpoint
			// 
			this.grpEndpoint.Controls.Add(this.txtReadTimeout);
			this.grpEndpoint.Controls.Add(this.btnClear);
			this.grpEndpoint.Controls.Add(this.lblReadTimeout);
			this.grpEndpoint.Controls.Add(this.chkWriteUseShortPacket);
			this.grpEndpoint.Controls.Add(this.chkReadUseShortPacket);
			this.grpEndpoint.Enabled = false;
			this.grpEndpoint.Location = new System.Drawing.Point(12, 80);
			this.grpEndpoint.Name = "grpEndpoint";
			this.grpEndpoint.Size = new System.Drawing.Size(293, 95);
			this.grpEndpoint.TabIndex = 11;
			this.grpEndpoint.TabStop = false;
			this.grpEndpoint.Text = "Endpoint properties";
			// 
			// txtReadTimeout
			// 
			this.txtReadTimeout.Location = new System.Drawing.Point(164, 65);
			this.txtReadTimeout.Name = "txtReadTimeout";
			this.txtReadTimeout.Size = new System.Drawing.Size(40, 20);
			this.txtReadTimeout.TabIndex = 3;
			this.txtReadTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtReadTimeout.Leave += new System.EventHandler(this.txtReadTimeout_Leave);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(227, 15);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(41, 23);
			this.btnClear.TabIndex = 13;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// lblReadTimeout
			// 
			this.lblReadTimeout.AutoSize = true;
			this.lblReadTimeout.Location = new System.Drawing.Point(6, 68);
			this.lblReadTimeout.Name = "lblReadTimeout";
			this.lblReadTimeout.Size = new System.Drawing.Size(152, 13);
			this.lblReadTimeout.TabIndex = 2;
			this.lblReadTimeout.Text = "Read Timeout (ms, 0 = infinity):";
			// 
			// chkWriteUseShortPacket
			// 
			this.chkWriteUseShortPacket.AutoSize = true;
			this.chkWriteUseShortPacket.Location = new System.Drawing.Point(6, 42);
			this.chkWriteUseShortPacket.Name = "chkWriteUseShortPacket";
			this.chkWriteUseShortPacket.Size = new System.Drawing.Size(129, 17);
			this.chkWriteUseShortPacket.TabIndex = 1;
			this.chkWriteUseShortPacket.Text = "WriteUseShortPacket";
			this.chkWriteUseShortPacket.UseVisualStyleBackColor = true;
			this.chkWriteUseShortPacket.Click += new System.EventHandler(this.chkWriteUseShortPacket_Click);
			// 
			// chkReadUseShortPacket
			// 
			this.chkReadUseShortPacket.AutoSize = true;
			this.chkReadUseShortPacket.Location = new System.Drawing.Point(6, 19);
			this.chkReadUseShortPacket.Name = "chkReadUseShortPacket";
			this.chkReadUseShortPacket.Size = new System.Drawing.Size(130, 17);
			this.chkReadUseShortPacket.TabIndex = 0;
			this.chkReadUseShortPacket.Text = "ReadUseShortPacket";
			this.chkReadUseShortPacket.UseVisualStyleBackColor = true;
			this.chkReadUseShortPacket.Click += new System.EventHandler(this.chkReadUseShortPacket_Click);
			// 
			// grpSetup
			// 
			this.grpSetup.Controls.Add(this.txtIndex);
			this.grpSetup.Controls.Add(this.lblType);
			this.grpSetup.Controls.Add(this.txtValue);
			this.grpSetup.Controls.Add(this.txtType);
			this.grpSetup.Controls.Add(this.txtRequest);
			this.grpSetup.Controls.Add(this.lblRequest);
			this.grpSetup.Controls.Add(this.lblIndex);
			this.grpSetup.Controls.Add(this.lblValue);
			this.grpSetup.Location = new System.Drawing.Point(149, 181);
			this.grpSetup.Name = "grpSetup";
			this.grpSetup.Size = new System.Drawing.Size(156, 60);
			this.grpSetup.TabIndex = 12;
			this.grpSetup.TabStop = false;
			this.grpSetup.Text = "Control Transfer (hex values)";
			// 
			// btnFind
			// 
			this.btnFind.Location = new System.Drawing.Point(264, 51);
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(41, 23);
			this.btnFind.TabIndex = 13;
			this.btnFind.Text = "Find";
			this.btnFind.UseVisualStyleBackColor = true;
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			// 
			// lblAttach
			// 
			this.lblAttach.Location = new System.Drawing.Point(251, 9);
			this.lblAttach.Name = "lblAttach";
			this.lblAttach.Size = new System.Drawing.Size(54, 13);
			this.lblAttach.TabIndex = 14;
			this.lblAttach.Text = "Detached";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radOut);
			this.panel1.Controls.Add(this.radIn);
			this.panel1.Location = new System.Drawing.Point(149, 48);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(112, 30);
			this.panel1.TabIndex = 16;
			// 
			// radOut
			// 
			this.radOut.AutoSize = true;
			this.radOut.Location = new System.Drawing.Point(45, 6);
			this.radOut.Name = "radOut";
			this.radOut.Size = new System.Drawing.Size(42, 17);
			this.radOut.TabIndex = 6;
			this.radOut.Text = "Out";
			this.radOut.UseVisualStyleBackColor = true;
			// 
			// radIn
			// 
			this.radIn.AutoSize = true;
			this.radIn.Location = new System.Drawing.Point(5, 6);
			this.radIn.Name = "radIn";
			this.radIn.Size = new System.Drawing.Size(34, 17);
			this.radIn.TabIndex = 5;
			this.radIn.Text = "In";
			this.radIn.UseVisualStyleBackColor = true;
			this.radIn.CheckedChanged += new System.EventHandler(this.radIn_CheckedChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.radText);
			this.panel2.Controls.Add(this.radHex);
			this.panel2.Location = new System.Drawing.Point(5, 210);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(58, 57);
			this.panel2.TabIndex = 17;
			// 
			// radText
			// 
			this.radText.AutoSize = true;
			this.radText.Checked = true;
			this.radText.Location = new System.Drawing.Point(7, 4);
			this.radText.Name = "radText";
			this.radText.Size = new System.Drawing.Size(46, 17);
			this.radText.TabIndex = 16;
			this.radText.TabStop = true;
			this.radText.Text = "Text";
			this.radText.UseVisualStyleBackColor = true;
			// 
			// radHex
			// 
			this.radHex.AutoSize = true;
			this.radHex.Location = new System.Drawing.Point(7, 27);
			this.radHex.Name = "radHex";
			this.radHex.Size = new System.Drawing.Size(44, 17);
			this.radHex.TabIndex = 17;
			this.radHex.Text = "Hex";
			this.radHex.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(317, 456);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lblAttach);
			this.Controls.Add(this.btnFind);
			this.Controls.Add(this.grpSetup);
			this.Controls.Add(this.grpEndpoint);
			this.Controls.Add(this.txtLength);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtGuid);
			this.Controls.Add(this.btnXfer);
			this.Controls.Add(this.lblLength);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.lblOutData);
			this.Controls.Add(this.lblInData);
			this.Controls.Add(this.lblEndpoint);
			this.Controls.Add(this.drpEndpoint);
			this.Controls.Add(this.lblOutDataList);
			this.Controls.Add(this.txtOutData);
			this.MinimumSize = new System.Drawing.Size(333, 380);
			this.Name = "MainForm";
			this.Text = "WinUSB Test Interface";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.grpEndpoint.ResumeLayout(false);
			this.grpEndpoint.PerformLayout();
			this.grpSetup.ResumeLayout(false);
			this.grpSetup.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.TextBox txtRequest;
		private System.Windows.Forms.Label lblRequest;
		private System.Windows.Forms.TextBox txtType;
		private System.Windows.Forms.Label lblType;
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.TextBox txtLength;
		private System.Windows.Forms.TextBox txtIndex;
		private System.Windows.Forms.Label lblLength;
		private System.Windows.Forms.Label lblIndex;
		private System.Windows.Forms.Label lblOutDataList;
		private System.Windows.Forms.TextBox txtOutData;
		private System.Windows.Forms.ComboBox drpEndpoint;
		private System.Windows.Forms.Label lblEndpoint;
		private System.Windows.Forms.Label lblInData;
		private System.Windows.Forms.Label lblOutData;
		private System.Windows.Forms.RichTextBox txtLog;
		private System.Windows.Forms.Button btnXfer;
		private System.Windows.Forms.TextBox txtGuid;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox grpEndpoint;
		private System.Windows.Forms.CheckBox chkReadUseShortPacket;
		private System.Windows.Forms.CheckBox chkWriteUseShortPacket;
		private System.Windows.Forms.TextBox txtReadTimeout;
		private System.Windows.Forms.Label lblReadTimeout;
		private System.Windows.Forms.GroupBox grpSetup;
		private System.Windows.Forms.Button btnFind;
		private System.Windows.Forms.Label lblAttach;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton radOut;
		private System.Windows.Forms.RadioButton radIn;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton radText;
		private System.Windows.Forms.RadioButton radHex;
	}
}

