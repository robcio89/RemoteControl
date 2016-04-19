using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RemoteControl
{
	public partial class Form1 : Form
	{
		#region DLL_IMPORTS
		// Get a handle to an application window.
		[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(string lpClassName,
			string lpWindowName);

		// Activate an application window.
		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		// Activate an application window.
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		#endregion

		const string SERVER_IP = "192.168.1.4";
		const Int32 PORT_NO = 12345;

		IPAddress localAdd;
		TcpListener server;
		TcpClient client;

		IntPtr foobarHandler;
		IntPtr kmPlayer;

		//
		private TcpListener tcplist;
		private Thread lstThread;
		private delegate void WriteMessageDelegate(string msg);

		public Form1()
		{
			InitializeComponent();
			localAdd = IPAddress.Parse(SERVER_IP);
			server = new TcpListener(IPAddress.Any, PORT_NO);

			this.tcplist = new TcpListener(IPAddress.Any, PORT_NO);
			this.lstThread = new Thread(new ThreadStart(lstClients));
			this.lstThread.IsBackground = true;
			this.lstThread.Start();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			foobarHandler = FindWindow("{97E27FAA-C0B3-4b8e-A693-ED7881E99FC1}", (string)null);
			kmPlayer = FindWindow("TApplication", (string)null);

			foreach (Process pTarget in Process.GetProcesses())
			{
				if (pTarget.ProcessName == "KMPlayer")
				{
					WriteMessage(pTarget.ProcessName + ": " + pTarget.ProcessName.GetHashCode());
					
					StringBuilder ClassName = new StringBuilder(256);

					WriteMessage("MainWindowHandle: " + pTarget.MainWindowHandle.ToString());

					var x = GetClassName(pTarget.MainWindowHandle, ClassName, ClassName.Capacity);
					
					WriteMessage("Classname: " + ClassName.ToString());
				}

				if (pTarget.ProcessName == "foobar2000")
				{
					WriteMessage(pTarget.ProcessName + ": " + pTarget.ProcessName.GetHashCode());

					StringBuilder ClassName = new StringBuilder(256);

					WriteMessage("MainWindowHandle: " + pTarget.MainWindowHandle.ToString());

					var x = GetClassName(pTarget.MainWindowHandle, ClassName, ClassName.Capacity);

					WriteMessage("Classname: " + ClassName.ToString());
				}
			}

			if (foobarHandler == IntPtr.Zero)
			{
				MessageBox.Show("foobar is not running.");
			}
			if (kmPlayer == IntPtr.Zero)
			{
				MessageBox.Show("KMPlayer is not running.");
				//start KMPlayer
			}
		}

		private void lstClients()
		{
			this.tcplist.Start();

			while (true)
			{
				TcpClient client = this.tcplist.AcceptTcpClient();
				Thread clientThread = new Thread(() => HandleClientComm(client));
				clientThread.Start();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				server.Start();
				client = server.AcceptTcpClient();
				while (true)
				{
					WriteMessage("Połączono \n");

					NetworkStream nwStream = client.GetStream();
					byte[] buffer = new byte[client.ReceiveBufferSize];

					int i;

					// Loop to receive all the data sent by the client.
					while ((i = nwStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						// Translate data bytes to a ASCII string.
						var data = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
						WriteMessage("Otrzymano: " + data + "\n");

						if (data == "PROGRAM_UP")
						{
							SetForegroundWindow(foobarHandler);
							SendKeys.SendWait("b");
						}
						if (data == "VOLUME_UP")
						{
							SetForegroundWindow(foobarHandler);
							SendKeys.SendWait("{ADD}");
						}
						if (data == "VOLUME_DOWN")
						{
							SetForegroundWindow(foobarHandler);
							SendKeys.SendWait("{SUBTRACT}");
						}
						if (data == "MUTE")
						{
							SetForegroundWindow(foobarHandler);
							SendKeys.SendWait("{DELETE}");
						}

						// Process the data sent by the client.
						data = data.ToUpper();

						byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

						// Send back a response.
						nwStream.Write(msg, 0, msg.Length);
					}
				}
				client.Close();

			}
			catch (SocketException error)
			{
				WriteMessage("Wystąpił błąd " + error.Message + "\n");
			}
			finally
			{
				server.Stop();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			client.Close();
			server.Stop();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			foobarHandler = FindWindow("{97E27FAA-C0B3-4b8e-A693-ED7881E99FC1}", (string)null);

			// Verify that Calculator is a running process.
			if (foobarHandler == IntPtr.Zero)
			{
				MessageBox.Show("foobar is not running.");
				return;
			}
			SetForegroundWindow(foobarHandler);
			SendKeys.SendWait("b");
		}

		private void Echo(string msg, ASCIIEncoding encoder, NetworkStream clientStream)
		{
			byte[] buffer = encoder.GetBytes(msg);
			clientStream.Write(buffer, 0, buffer.Length);
			clientStream.Flush();
		}

		private void HandleClientComm(TcpClient client)
		{
			NetworkStream clientStream = client.GetStream();
			byte[] message = new byte[1024 * 25000];
			var ipAddr = client.Client.RemoteEndPoint.ToString();
			WriteMessage("Nawiązano połączenie z: " + ipAddr);

			while (true)
			{
				var bytesRead = 0;
				try
				{
					var bt = new byte[1024];
					var bytesRead2 = clientStream.Read(bt, 0, 1024);
					if (bytesRead2 == 0)
					{
						WriteMessage("Brak danych od zdalnego hosta");
						break;
					}
					bt.CopyTo(message, bytesRead);
					bytesRead += bytesRead2;
				}
				catch (Exception ex)
				{
					WriteMessage("Błąd: " + ex.Message);
					break;
				}

				ASCIIEncoding encoder = new ASCIIEncoding();
				string msg = encoder.GetString(message, 0, bytesRead);
				WriteMessage("Wciśniety przycisk: " + msg.ToString());

				if (msg == "PROGRAM_UP")
				{
					SetForegroundWindow(foobarHandler);
					SendKeys.SendWait("b");

					//SetForegroundWindow(kmPlayer);
					//SendKeys.SendWait("Up");
				}
			}
			WriteMessage("Zakończono połączenie (utracone) z: " + ipAddr);
			client.Close();
			GC.Collect();
		}

		private void WriteMessage(string msg)
		{
			if (this.txtConsoleOutput.InvokeRequired)
			{
				WriteMessageDelegate d = new WriteMessageDelegate(WriteMessage);
				this.txtConsoleOutput.Invoke(d, new object[] { msg });
			}
			else
			{
				txtConsoleOutput.AppendText( msg + "\n");
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			GC.Collect();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			GC.Collect();
		}
	}
}
