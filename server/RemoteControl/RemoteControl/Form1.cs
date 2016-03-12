using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
		#endregion

		const string SERVER_IP = "192.168.1.4";
		const Int32 PORT_NO = 12345;

		IPAddress localAdd;
		TcpListener server;
		TcpClient client;

		IntPtr calculatorHandle;

		public Form1()
		{
			InitializeComponent();
			localAdd = IPAddress.Parse(SERVER_IP);
			server = new TcpListener(IPAddress.Any, PORT_NO);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			calculatorHandle = FindWindow("{97E27FAA-C0B3-4b8e-A693-ED7881E99FC1}", (string)null);

			// Verify that Calculator is a running process.
			if (calculatorHandle == IntPtr.Zero)
			{
				MessageBox.Show("foobar is not running.");
				return;
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
					
					txtConsoleOutput.AppendText("Połączono \n");

					NetworkStream nwStream = client.GetStream();
					byte[] buffer = new byte[client.ReceiveBufferSize];

					int i;

					// Loop to receive all the data sent by the client.
					while ((i = nwStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						// Translate data bytes to a ASCII string.
						var data = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
						txtConsoleOutput.AppendText("Otrzymano: " + data + "\n");

						if (data == "PROGRAM_UP")
						{
							SetForegroundWindow(calculatorHandle);
							SendKeys.SendWait("b");
						}
						if (data == "VOLUME_UP")
						{
							SetForegroundWindow(calculatorHandle);
							SendKeys.SendWait("{ADD}");
						}
						if (data == "VOLUME_DOWN")
						{
							SetForegroundWindow(calculatorHandle);
							SendKeys.SendWait("{SUBTRACT}");
						}
						if (data == "MUTE")
						{
							SetForegroundWindow(calculatorHandle);
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
				txtConsoleOutput.AppendText("Wystąpił błąd " + error.Message + "\n");
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
			calculatorHandle = FindWindow("{97E27FAA-C0B3-4b8e-A693-ED7881E99FC1}", (string)null);

			// Verify that Calculator is a running process.
			if (calculatorHandle == IntPtr.Zero)
			{
				MessageBox.Show("foobar is not running.");
				return;
			}

			// Make Calculator the foreground application and send it 
			// a set of calculations.
			SetForegroundWindow(calculatorHandle);
			SendKeys.SendWait("b");
			//SendKeys.SendWait("*");
			//SendKeys.SendWait("111");
			//SendKeys.SendWait("=");
		}
	}
}
