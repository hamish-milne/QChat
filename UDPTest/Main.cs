using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDPTest
{
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();
		}

		Socket socket;
		Thread receiveThread;

		private void buttonBind_Click(object sender, EventArgs e)
		{
			try
			{
				var ip = IPAddress.Parse(textSourceIP.Text);
				ip = QChatLib.Util.ToIPv6(ip);
				var port = int.Parse(textSourcePort.Text);
				lock(socket)
					socket.Bind(new IPEndPoint(ip, port));
			} catch(Exception err)
			{
				lock (statusLabel)
					statusLabel.Text = err.Message;
			}
		}

		void ReceiveThread()
		{
			var buf = new byte[8];
			var ep = new IPEndPoint(IPAddress.IPv6Any, 0);
			while(true)
			{
				if (!socket.IsBound)
				{
					Thread.Sleep(100);
					continue;
				}
				try
				{
					ep.Address = IPAddress.IPv6Any;
					ep.Port = 0;
					EndPoint refEP = ep;
					lock (socket)
						socket.ReceiveFrom(buf, ref refEP);
					ep = (IPEndPoint)refEP;
					statusLabel.Text = "Received: " + ep.Address + ":" + ep.Port + " " + DateTime.Now.ToLongTimeString();
				} catch(SocketException e)
				{
					if(e.SocketErrorCode == SocketError.TimedOut)
						continue;
					lock (statusLabel)
						statusLabel.Text = e.Message;
				}
			}
		}

		private void Main_Load(object sender, EventArgs e)
		{
			try
			{
				socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				socket.ReceiveTimeout = 100;
				receiveThread = new Thread(ReceiveThread);
				receiveThread.Start();
			} catch(Exception err)
			{
				MessageBox.Show(err.Message, err.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			try
			{
				var ep = new IPEndPoint(IPAddress.Parse(textDestIP.Text), int.Parse(textDestPort.Text));
				lock (socket)
					socket.SendTo(new byte[0], ep);
			} catch(Exception err)
			{
				lock (statusLabel)
					statusLabel.Text = err.Message;
			}
		}

		private void Main_FormClosed(object sender, FormClosedEventArgs e)
		{
			receiveThread.Abort();
		}
	}
}
