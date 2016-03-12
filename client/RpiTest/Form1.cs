using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace RpiTest
{
    public partial class Form1 : Form
    {
        TcpClient client = new TcpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            byte[] a1 = Encoding.ASCII.GetBytes("1");
            NetworkStream stream = client.GetStream();
            stream.Write(a1, 0, a1.Length);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.1.111"), int.Parse("12345")); // endpoint where server is listening
            client.Connect(ep);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] a1 = Encoding.ASCII.GetBytes("2");
            NetworkStream stream = client.GetStream();
            stream.Write(a1, 0, a1.Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] a1 = Encoding.ASCII.GetBytes("RED");
            NetworkStream stream = client.GetStream();
            stream.Write(a1, 0, a1.Length);

            textBox2.AppendText(stream.Read(a1, 0, a1.Length).ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] a1 = Encoding.ASCII.GetBytes("BLUE");
            NetworkStream stream = client.GetStream();
            stream.Write(a1, 0, a1.Length);

            textBox2.AppendText(stream.Read(a1, 0, a1.Length).ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] a2 = Encoding.ASCII.GetBytes("AAAAAAAAAAAAAAA");
            byte[] a1 = Encoding.ASCII.GetBytes("GREEN");
            NetworkStream stream = client.GetStream();
            stream.Write(a1, 0, a1.Length);

            textBox2.AppendText(stream.Read(a2, 0, a2.Length).ToString());
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
        }
    }
}
