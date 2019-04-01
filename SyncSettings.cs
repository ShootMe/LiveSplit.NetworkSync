using LiveSplit.UI;
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.NetworkSync {
	public partial class SyncSettings : UserControl {
		public string IP { get; set; }
		public string Port {
			get { return DataPort.ToString(); }
			set {
				if (int.TryParse(value, out DataPort)) {
					if (DataPort < 1 || DataPort > 65536) {
						DataPort = 7777;
					}
				} else {
					DataPort = 7777;
				}
			}
		}
		private int DataPort;
		public bool IsHost { get; set; }
		private NetworkController controller;
		private Thread startThread, updateThread;
		private string localIP;
		public SyncSettings(NetworkController controller) {
			InitializeComponent();

			this.controller = controller;
			IsHost = false;
			IP = "localhost";
			Port = "7777";
			localIP = controller.GetLocalIPAddress();

			chkServer.DataBindings.Add("Checked", this, "IsHost", false, DataSourceUpdateMode.OnPropertyChanged);
			txtIP.DataBindings.Add("Text", this, "IP", false, DataSourceUpdateMode.OnPropertyChanged);
			txtPort.DataBindings.Add("Text", this, "Port", false, DataSourceUpdateMode.OnPropertyChanged);

			updateThread = new Thread(UpdateStatus);
			updateThread.IsBackground = true;
			updateThread.Name = "Status Updater";
			updateThread.Start();
		}

		public void Terminate() {
			if (updateThread != null) {
				updateThread.Abort();
				updateThread = null;
			}
			if (startThread != null) {
				startThread.Abort();
				startThread = null;
			}
		}
		private void UpdateStatus() {
			try {
				while (true) {
					if (IsHost && controller.IsRunning) {
						SetStatus("Running with " + controller.TotalConnections + " connections\r\nLocal IP: " + localIP);
					}
					Thread.Sleep(100);
				}
			} catch { }
		}
		private void Settings_Load(object sender, EventArgs e) {
			FindForm().Text = "Network Sync v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

			chkServer_CheckedChanged(null, null);
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			XmlElement parent = document.CreateElement("Settings");
			CreateSettings(document, parent);
			UpdateController();
			return parent;
		}
		public int CreateSettings(XmlDocument document, XmlElement parent) {
			return SettingsHelper.CreateSetting(document, parent, "IsHost", IsHost) ^
			SettingsHelper.CreateSetting(document, parent, "ServerIP", IP) ^
			SettingsHelper.CreateSetting(document, parent, "Port", Port);
		}
		public void SetSettings(XmlNode settings) {
			XmlElement element = (XmlElement)settings;

			IsHost = SettingsHelper.ParseBool(element["IsHost"], false);
			IP = SettingsHelper.ParseString(element["ServerIP"], "localhost");
			Port = SettingsHelper.ParseString(element["Port"], "7777");

			UpdateController();
		}
		private void SetStatus(string status) {
			if (this.InvokeRequired) {
				this.Invoke((Action<string>)SetStatus, status);
			} else {
				lblStatus.Text = status;
			}
		}
		public void UpdateController() {
			if (controller.IsHosting != IsHost || controller.IP != IP || controller.Port.ToString() != Port) {
				if (startThread != null) {
					try {
						startThread.Abort();
					} catch { }
					startThread = null;
				}
				startThread = new Thread(StopStartController);
				startThread.IsBackground = true;
				startThread.Start();
			}
		}
		private void StopStartController() {
			try {
				SetStatus("Stopping...");
				controller.Stop();
				controller.IsHosting = IsHost;
				controller.IP = IP;
				controller.Port = DataPort;

				if (IsHost) {
					TryStart();
				} else {
					TryConnect();
				}
			} catch { }
		}
		private void TryStart() {
			while (true) {
				try {
					if (!controller.IsRunning) {
						SetStatus("Trying to start server on port " + Port);
					}
					bool started = controller.Start();
					if (!started) {
						SetStatus("Failed to start server on port " + Port + "\r\nTry running LiveSplit as administrator.");
					}
					Thread.Sleep(1000);
				} catch { }
			}
		}
		private void TryConnect() {
			while (true) {
				try {
					if (!controller.IsRunning) {
						SetStatus("Trying to connect to " + IP + ":" + Port);
					}
					bool connected = controller.Connect();
					if (connected) {
						SetStatus("Connected to " + IP + ":" + Port);
					} else {
						SetStatus("Failed to connect to " + IP + ":" + Port);
					}
					Thread.Sleep(1000);
				} catch { }
			}
		}
		private void chkServer_CheckedChanged(object sender, EventArgs e) {
			IsHost = chkServer.Checked;
			lblIP.Visible = !IsHost;
			txtIP.Visible = !IsHost;
			UpdateController();
		}
		private void txtIP_Validated(object sender, EventArgs e) {
			UpdateController();
		}
		private void txtPort_Validated(object sender, EventArgs e) {
			UpdateController();
		}
	}
}