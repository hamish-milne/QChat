namespace UDPTest
{
	partial class Main
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
			this.textSourceIP = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textSourcePort = new System.Windows.Forms.TextBox();
			this.textDestIP = new System.Windows.Forms.TextBox();
			this.textDestPort = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonBind = new System.Windows.Forms.Button();
			this.buttonSend = new System.Windows.Forms.Button();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// textSourceIP
			// 
			this.textSourceIP.Location = new System.Drawing.Point(100, 9);
			this.textSourceIP.Name = "textSourceIP";
			this.textSourceIP.Size = new System.Drawing.Size(100, 20);
			this.textSourceIP.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Source IP";
			// 
			// textSourcePort
			// 
			this.textSourcePort.Location = new System.Drawing.Point(100, 36);
			this.textSourcePort.Name = "textSourcePort";
			this.textSourcePort.Size = new System.Drawing.Size(100, 20);
			this.textSourcePort.TabIndex = 2;
			// 
			// textDestIP
			// 
			this.textDestIP.Location = new System.Drawing.Point(100, 63);
			this.textDestIP.Name = "textDestIP";
			this.textDestIP.Size = new System.Drawing.Size(100, 20);
			this.textDestIP.TabIndex = 3;
			// 
			// textDestPort
			// 
			this.textDestPort.Location = new System.Drawing.Point(100, 90);
			this.textDestPort.Name = "textDestPort";
			this.textDestPort.Size = new System.Drawing.Size(100, 20);
			this.textDestPort.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Source Port";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(73, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Destination IP";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Destination Port";
			// 
			// buttonBind
			// 
			this.buttonBind.Location = new System.Drawing.Point(207, 9);
			this.buttonBind.Name = "buttonBind";
			this.buttonBind.Size = new System.Drawing.Size(75, 23);
			this.buttonBind.TabIndex = 8;
			this.buttonBind.Text = "Bind";
			this.buttonBind.UseVisualStyleBackColor = true;
			this.buttonBind.Click += new System.EventHandler(this.buttonBind_Click);
			// 
			// buttonSend
			// 
			this.buttonSend.Location = new System.Drawing.Point(207, 63);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(75, 23);
			this.buttonSend.TabIndex = 9;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 120);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(290, 22);
			this.statusStrip.TabIndex = 10;
			this.statusStrip.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(46, 17);
			this.statusLabel.Text = "Loaded";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(290, 142);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.buttonBind);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDestPort);
			this.Controls.Add(this.textDestIP);
			this.Controls.Add(this.textSourcePort);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textSourceIP);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Main";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "UDP Test";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.Load += new System.EventHandler(this.Main_Load);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textSourceIP;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSourcePort;
		private System.Windows.Forms.TextBox textDestIP;
		private System.Windows.Forms.TextBox textDestPort;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonBind;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
	}
}

