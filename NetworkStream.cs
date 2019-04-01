using System;
using System.IO;
using System.Text;
namespace LiveSplit.NetworkSync {
	public sealed class NetworkStream {
		private Stream msgStream;
		public long BytesSent;
		public long BytesReceived;
		public int CallsSent;
		public int CallsReceived;
		private static string[] ACK = { "*" };
		private byte[] recBuffer = new byte[65536];

		public NetworkStream(Stream stream) {
			msgStream = stream;
			msgStream.ReadTimeout = System.Threading.Timeout.Infinite;
		}

		public void Send(params string[] parameters) {
			if (msgStream == null || parameters == null || parameters.Length == 0) { return; }

			if (!msgStream.CanRead || !msgStream.CanWrite) { throw new Exception("Underlying stream is closed."); }

			CallsSent++;

			if (parameters[0] == "*") {
				msgStream.WriteByte(255);
				BytesSent++;
			} else {
				byte[] data;
				using (MemoryStream packet = new MemoryStream()) {
					packet.WriteByte((byte)parameters.Length);
					for (int i = 0; i < parameters.Length; i++) {
						string item = parameters[i];
						short length = (short)item.Length;
						data = BitConverter.GetBytes(length);
						packet.Write(data, 0, data.Length);
						data = Encoding.ASCII.GetBytes(item);
						packet.Write(data, 0, length);
					}
					data = packet.ToArray();
				}

				msgStream.Write(data, 0, data.Length);
				BytesSent += data.Length + 4;
			}
			msgStream.Flush();
		}
		public string[] Receive() {
			if (msgStream == null) { return null; }

			if (!msgStream.CanRead || !msgStream.CanWrite) { throw new Exception("Underlying stream is closed."); }

			int total = 0;
			try { total = msgStream.ReadByte(); } catch { return null; }

			CallsReceived++;
			BytesReceived++;
			if (total < 1) {
				return null;
			} else if (total == 255) {
				return ACK;
			}

			string[] parameters = new string[total];
			for (int i = 0; i < total; i++) {
				msgStream.Read(recBuffer, 0, 2);
				int length = BitConverter.ToInt16(recBuffer, 0);
				msgStream.Read(recBuffer, 0, length);
				parameters[i] = Encoding.ASCII.GetString(recBuffer, 0, length);
				BytesReceived += length + 2;
			}

			return parameters;
		}
	}
}