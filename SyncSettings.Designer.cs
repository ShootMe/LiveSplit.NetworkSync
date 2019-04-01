namespace LiveSplit.NetworkSync {
	partial class SyncSettings {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.txtIP = new System.Windows.Forms.TextBox();
			this.lblIP = new System.Windows.Forms.Label();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.lblPort = new System.Windows.Forms.Label();
			this.chkServer = new System.Windows.Forms.CheckBox();
			this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.lblStatus = new System.Windows.Forms.Label();
			this.flowLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtIP
			// 
			this.txtIP.Location = new System.Drawing.Point(3, 39);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(160, 20);
			this.txtIP.TabIndex = 1;
			this.txtIP.Validated += new System.EventHandler(this.txtIP_Validated);
			// 
			// lblIP
			// 
			this.lblIP.AutoSize = true;
			this.lblIP.Location = new System.Drawing.Point(3, 23);
			this.lblIP.Name = "lblIP";
			this.lblIP.Size = new System.Drawing.Size(51, 13);
			this.lblIP.TabIndex = 0;
			this.lblIP.Text = "Server IP";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(3, 78);
			this.txtPort.MaxLength = 5;
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(45, 20);
			this.txtPort.TabIndex = 2;
			this.txtPort.Validated += new System.EventHandler(this.txtPort_Validated);
			// 
			// lblPort
			// 
			this.lblPort.AutoSize = true;
			this.lblPort.Location = new System.Drawing.Point(3, 62);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new System.Drawing.Size(26, 13);
			this.lblPort.TabIndex = 3;
			this.lblPort.Text = "Port";
			// 
			// chkServer
			// 
			this.chkServer.AutoSize = true;
			this.chkServer.Location = new System.Drawing.Point(3, 3);
			this.chkServer.Name = "chkServer";
			this.chkServer.Size = new System.Drawing.Size(96, 17);
			this.chkServer.TabIndex = 5;
			this.chkServer.Text = "Host Computer";
			this.chkServer.UseVisualStyleBackColor = true;
			this.chkServer.CheckedChanged += new System.EventHandler(this.chkServer_CheckedChanged);
			// 
			// flowLayout
			// 
			this.flowLayout.Controls.Add(this.chkServer);
			this.flowLayout.Controls.Add(this.lblIP);
			this.flowLayout.Controls.Add(this.txtIP);
			this.flowLayout.Controls.Add(this.lblPort);
			this.flowLayout.Controls.Add(this.txtPort);
			this.flowLayout.Controls.Add(this.lblStatus);
			this.flowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayout.Location = new System.Drawing.Point(0, 0);
			this.flowLayout.Name = "flowLayout";
			this.flowLayout.Size = new System.Drawing.Size(240, 151);
			this.flowLayout.TabIndex = 6;
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(3, 101);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(37, 13);
			this.lblStatus.TabIndex = 6;
			this.lblStatus.Text = "Status";
			// 
			// SyncSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.flowLayout);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "SyncSettings";
			this.Size = new System.Drawing.Size(240, 151);
			this.Load += new System.EventHandler(this.Settings_Load);
			this.flowLayout.ResumeLayout(false);
			this.flowLayout.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtIP;
		private System.Windows.Forms.Label lblIP;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.Label lblPort;
		private System.Windows.Forms.CheckBox chkServer;
		private System.Windows.Forms.FlowLayoutPanel flowLayout;
		private System.Windows.Forms.Label lblStatus;
	}
}
