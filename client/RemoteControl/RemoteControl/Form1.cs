using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteControl
{
	public partial class Form1 : Form
	{
		const string SERVER_IP = "192.168.1.4";
		const Int32 PORT_NO = 12345;

		IPAddress localAdd;
		TcpListener server;
		TcpClient client;

		public Form1()
		{
			InitializeComponent();
			localAdd = IPAddress.Parse(SERVER_IP);
			server = new TcpListener(IPAddress.Any, PORT_NO);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			server.Start();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				String data = null;

				while (true)
				{
					client = server.AcceptTcpClient();
					txtConsoleOutput.AppendText("Połączono \n");

					NetworkStream nwStream = client.GetStream();
					byte[] buffer = new byte[client.ReceiveBufferSize];

					int i;

					// Loop to receive all the data sent by the client.
					while ((i = nwStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						// Translate data bytes to a ASCII string.
						data = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
						txtConsoleOutput.AppendText("Otrzymano: " + data + "\n");

						// Process the data sent by the client.
						data = data.ToUpper();

						byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

						// Send back a response.
						nwStream.Write(msg, 0, msg.Length);
						//txtConsoleOutput.AppendText("Sent: " + data + "\n");
					}

					client.Close();
				}

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
	}
}
