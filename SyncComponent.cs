using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.NetworkSync {
	public class SyncComponent : IComponent {
		public TimerModel Model { get; set; }
		public string ComponentName { get { return "Network Sync " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3); } }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private NetworkController controller = new NetworkController();
		private static string LOGFILE = "_NetworkSync.txt";
		private SyncSettings settings;
		private int currentSplit = -1, lastLogCheck;
		private bool hasLog = false, lastLoading = false, shouldSend = true;
		private TimeSpan? lastLoadingTime;
		public SyncComponent(LiveSplitState state) {
			settings = new SyncSettings(controller);
			controller.Received += Controller_Received;

			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;
			}
		}

		private void Controller_Received(string[] parameters) {
			try {
				string key = parameters[0].ToLower();
				shouldSend = false;
				switch (key) {
					case "start":
						Model.Start();
						break;
					case "reset":
						Model.Reset();
						break;
					case "split":
						Model.Split();
						TimeSpan? realTime = parameters[1] == "null" ? null : (TimeSpan?)TimeSpan.FromTicks(long.Parse(parameters[1]));
						TimeSpan? gameTime = parameters[2] == "null" ? null : (TimeSpan?)TimeSpan.FromTicks(long.Parse(parameters[2]));
						Model.CurrentState.Run[currentSplit - 1].SplitTime = new Time(realTime, gameTime);
						break;
					case "pause":
						Model.Pause();
						break;
					case "skip":
						Model.SkipSplit();
						break;
					case "undo":
						Model.UndoSplit();
						break;
					case "gametime":
						lastLoading = bool.Parse(parameters[1]);
						Model.CurrentState.IsGameTimePaused = lastLoading;
						lastLoadingTime = parameters[2] == "null" ? null : (TimeSpan?)TimeSpan.FromTicks(long.Parse(parameters[2]));
						Model.CurrentState.SetGameTime(lastLoadingTime);
						break;
				}
			} catch { }
			shouldSend = true;
			LogValues(parameters);
		}
		private void LogValues(string[] parameters) {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < parameters.Length; i++) {
					sb.Append(parameters[i]).Append(' ');
				}

				WriteLogWithTime(sb.ToString());
			}
		}
		private void WriteLog(string data) {
			lock (LOGFILE) {
				if (hasLog || !Console.IsOutputRedirected) {
					if (!Console.IsOutputRedirected) {
						Console.WriteLine(data);
					}
					if (hasLog) {
						using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
							wr.WriteLine(data);
						}
					}
				}
			}
		}
		private void WriteLogWithTime(string data) {
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
		}
		private void SendInfo(params string[] parameters) {
			if (!shouldSend) { return; }
			controller.Send(parameters);
		}
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			if (!controller.IsHosting) {
				if (lastLoading != Model.CurrentState.IsGameTimePaused || (Model.CurrentState.GameTimePauseTime.HasValue && lastLoadingTime.HasValue && lastLoadingTime.Value != Model.CurrentState.GameTimePauseTime.Value) || Model.CurrentState.GameTimePauseTime.HasValue != lastLoadingTime.HasValue) {
					lastLoading = Model.CurrentState.IsGameTimePaused;
					lastLoadingTime = Model.CurrentState.GameTimePauseTime;
					SendInfo("GameTime", lastLoading.ToString(), lastLoadingTime.HasValue ? lastLoadingTime.Value.Ticks.ToString() : "null");
				}
			}
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			SendInfo("Reset");
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			SendInfo("Pause");
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			SendInfo("Pause");
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			SendInfo("Start");
			WriteLog("---------New Game " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3) + "-------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			SendInfo("Undo");
			WriteLog("---------Undo-----------------------------------");
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			SendInfo("Skip");
			WriteLog("---------Skip-----------------------------------");
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			Time seg = Model.CurrentState.Run[currentSplit - 1].SplitTime;
			SendInfo("Split", seg.RealTime.Value.Ticks.ToString(), seg.GameTime.HasValue ? seg.GameTime.Value.Ticks.ToString() : "null");
			WriteLog("---------Split----------------------------------");
		}
		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public int GetSettingsHashCode() {
			return settings.CreateSettings(null, null);
		}
		public void Dispose() {
			if (Model != null) {
				Model.CurrentState.OnReset -= OnReset;
				Model.CurrentState.OnPause -= OnPause;
				Model.CurrentState.OnResume -= OnResume;
				Model.CurrentState.OnStart -= OnStart;
				Model.CurrentState.OnSplit -= OnSplit;
				Model.CurrentState.OnUndoSplit -= OnUndoSplit;
				Model.CurrentState.OnSkipSplit -= OnSkipSplit;
				Model = null;
			}
			settings.Terminate();
		}
	}
}