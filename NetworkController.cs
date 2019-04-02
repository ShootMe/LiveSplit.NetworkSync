using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace LiveSplit.NetworkSync {
	public class NetworkController {
		private const int TIMEOUT = 3000;
		public string IP { get; set; }
		public int Port { get; set; }
		public bool IsHosting { get; set; }
		public bool IsRunning { get; set; }
		public int TotalConnections { get; set; }
		public delegate void MessageReceived(string[] parameters);
		public event MessageReceived Received;
		private Thread serverThread;
		private TcpListener server;
		private ManualResetEvent waitEvent = new ManualResetEvent(false);
		private Queue<string[]> parameterQueue = new Queue<string[]>();
		public string GetLocalIPAddress() {
			try {
				using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
					socket.Connect("8.8.8.8", 65530);
					IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
					return endPoint.Address.ToString();
				}
			} catch {
				return "0.0.0.0";
			}
		}
		public bool Start() {
			try {
				if (IsRunning) { return true; }
				Stop();

				server = new TcpListener(IPAddress.Parse("0.0.0.0"), Port);
				server.Start();

				serverThread = new Thread(StartService);
				serverThread.Name = "Server Connection Handler";
				serverThread.IsBackground = true;
				serverThread.Start();

				return true;
			} catch {
				Stop();
				return false;
			}
		}
		private void StartService() {
			IsRunning = true;

			while (IsRunning) {
				try {
					Thread newConnection = new Thread(Sync);
					newConnection.IsBackground = true;
					TcpClient client = server.AcceptTcpClient();
					lock (waitEvent) {
						TotalConnections++;
						newConnection.Name = TotalConnections.ToString();
					}
					newConnection.Start(client);
				} catch {
					Thread.Sleep(100);
				}
			}
			serverThread = null;
			Stop();
		}
		private void Sync(object tcpClient) {
			TcpClient client = (TcpClient)tcpClient;
			try {
				NetworkStream local = new NetworkStream(client.GetStream());
				while (IsRunning) {
					string[] parameters = local.Receive();
					if (parameters != null && parameters[0] != "*") {
						Received?.Invoke(parameters);
					}
					if (parameterQueue.Count > 0) {
						local.Send(parameterQueue.Dequeue());
					} else {
						local.Send("*");
					}
				}
			} catch {
			} finally {
				try {
					lock (waitEvent) {
						TotalConnections--;
					}
					if (client != null) {
						client.Close();
					}
				} catch { }
				Stop();
			}
		}
		public void Stop() {
			IsRunning = false;
			if (server != null) {
				try {
					server.Stop();
				} catch { }
				server = null;
			}
			if (serverThread != null) {
				try {
					serverThread.Abort();
				} catch { }
			}
		}
		public void Send(params string[] parameters) {
			if (IsRunning) {
				parameterQueue.Enqueue(parameters);
			}
		}
		public bool TestConnection() {
			TcpClient client = null;
			try {
				client = new TcpClient() { ReceiveTimeout = TIMEOUT, SendTimeout = TIMEOUT };
				client.Connect(IP, Port);
				return true;
			} catch {
			} finally {
				if (client != null) {
					try {
						client.Close();
					} catch { }
					client = null;
				}
			}
			return false;
		}
		public bool Connect() {
			try {
				if (IsRunning) { return true; }
				Stop();

				TcpClient client = new TcpClient() { ReceiveTimeout = TIMEOUT, SendTimeout = TIMEOUT };
				client.Connect(IP, Port);

				serverThread = new Thread(ConnectService);
				serverThread.Name = "Server Connection Handler";
				serverThread.IsBackground = true;
				serverThread.Start(client);

				return true;
			} catch {
				Stop();
				return false;
			}
		}
		public void ConnectService(object tcpClient) {
			TcpClient client = (TcpClient)tcpClient;
			IsRunning = true;
			try {
				NetworkStream local = new NetworkStream(client.GetStream());
				while (IsRunning) {
					if (parameterQueue.Count > 0) {
						local.Send(parameterQueue.Dequeue());
					} else {
						local.Send("*");
					}
					string[] parameters = local.Receive();
					if (parameters != null && parameters[0] != "*") {
						Received?.Invoke(parameters);
					}
					if (parameterQueue.Count <= 0) {
						waitEvent.WaitOne(8);
					}
				}
			} catch {
			} finally {
				if (client != null) {
					try {
						client.Close();
					} catch { }
					client = null;
				}
				Stop();
			}
		}
	}
}