using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ChatingApp
{
    public partial class Form1 : Form
    {
        private Boolean g_Connected;
        private Socket socket;
        private string name;

        private AsyncCallback async_receive;
        private AsyncCallback async_send;

        delegate void AppendTextDelegate(Control ctrl, string name, string message);
        AppendTextDelegate _textAppender;

        public Form1()
        {
            async_receive = new AsyncCallback(HandleReceive);
            async_send = new AsyncCallback(HandleSend);

            InitializeComponent();
        }

        private void InitializeClient()
        {
            const string ip = "127.0.0.1";
            const int port = 1000;

            ConnectToServer(ip, port);

            if (Connected()) { MessageBox.Show("연결성공 !!"); }
            else { MessageBox.Show("연결 실패 !!"); }
        }

        public void AppendText(Control ctrl, string name, string message)
        {
            if (_textAppender == null) _textAppender = new AppendTextDelegate(AppendText);

            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, name, message);
            else
            {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + name + ":" + message;
            }
        }

        ///버튼 관련 
        private void button_send_Click(object sender, EventArgs e)
        {
            string message = send_text.Text;

            if (Connected()) { SendMessage(message); send_text.Text = ""; }
        }


        private void button_connect_Click(object sender, EventArgs e)
        {
            InitializeClient();
        }

        public Boolean Connected() { return g_Connected; }

        public void ConnectToServer(string ip, int port)
        {
            name = nameBox.Text;  // 사용자 이름

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            Boolean isConnected = false;

            try
            {
                // localhost 주소 === IPAddress.Loopback
                socket.Connect(IPAddress.Parse(ip), port);
                isConnected = true;
            }
            catch
            {
                isConnected = false;
            }
            finally
            {
                g_Connected = isConnected;
            }

            if (isConnected)
            {
                AsyncObject ao = new AsyncObject(4096);
                ao.workingSocket = socket;

                socket.BeginReceive(ao.buffer, 0,
                    ao.buffer.Length, SocketFlags.None, async_receive, ao);
            }
        }

        public void SendMessage(string message)
        {
            AsyncObject ao = new AsyncObject(4096);

            string sendMessage = string.Format("{0} : {1}", name, message);

            ao.buffer = Encoding.Unicode.GetBytes(sendMessage);

            ao.workingSocket = socket;

            socket.BeginSend(ao.buffer, 0, ao.buffer.Length,
                                  SocketFlags.None, async_send, ao);
        }

        private void HandleReceive(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;

            int bytes = ao.workingSocket.EndReceive(ar);

            string[] receiveMessage = Encoding.Unicode.GetString(ao.buffer).Split(':');

            AppendText(textBox, receiveMessage[0], receiveMessage[1]);
            ao.clearBuffer();

            socket.BeginReceive(ao.buffer, 0,
                   ao.buffer.Length, SocketFlags.None, async_receive, ao);

        }

        private void HandleSend(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;

            int bytes = ao.workingSocket.EndReceive(ar);

            string[] sendMessage = Encoding.Unicode.GetString(ao.buffer).Split(':');

            // name : message
            AppendText(textBox, sendMessage[0], sendMessage[1]);

            ao.clearBuffer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}