using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace ChatingApp.server
{
    class ChatServer
    {
        private const int port = 1000;
        private Socket socket; // server socket
        private List<Socket> sockets = new List<Socket>(); // client sockets

        private AsyncCallback async_receive;
        private AsyncCallback async_send;
        private AsyncCallback async_connect;

        public ChatServer()
        {
            async_receive = new AsyncCallback(HandleReceive);
            async_send = new AsyncCallback(HandleSend);
            async_connect = new AsyncCallback(HandleConnect);
        }

        public void StartServer()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));  //ip와 port번호를 socket에 바인딩

            socket.Listen(10); //client의 요청을 몇개까지 받을 것인가? :: backlog queue의 최대 길이

            socket.BeginAccept(async_connect, null); // client와의 연결을 비동기적으로 처리
        }

        
        public void SendMessage(string message)
        {
            AsyncObject ao = new AsyncObject(4096);
            ao.buffer = Encoding.Unicode.GetBytes(message);
            ao.workingSocket = socket;
            socket.BeginSend(ao.buffer, 0, ao.buffer.Length, SocketFlags.None, async_send, ao);
        }


        /// CallBack Methods ///
        public void HandleSend(IAsyncResult ar)
        {
           AsyncObject ao = (AsyncObject)ar.AsyncState;

           int bytes = ao.workingSocket.EndReceive(ar);

           socket.BeginSend(ao.buffer, 0, ao.buffer.Length,
                               SocketFlags.None, async_send, ao);
        }
        

        public void HandleConnect(IAsyncResult ar)
        {
            Socket socketClient = socket.EndAccept(ar); // EndAccept 비동기적으로 오는 연결요청을 받는다.

            AsyncObject ao = new AsyncObject(4096);
            ao.workingSocket = socketClient;

            sockets.Add(socketClient);

            socketClient.BeginReceive(ao.buffer,0,ao.buffer.Length,
                    SocketFlags.None, async_receive, ao);

            socket.BeginAccept(async_connect,null); // 다른 클라이언트의 연결요청을 받음
        }

        public void HandleReceive(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject) ar.AsyncState;

            string message = Encoding.Unicode.GetString(ao.buffer);
            string[] receiveMessage = message.Split(":");


            int bytes = ao.workingSocket.EndReceive(ar);

            foreach (var socket in sockets)
            {
                if (socket.Handle != ao.workingSocket.Handle)
                {
                    socket.Send(ao.buffer);
                }
            }
            
            Console.WriteLine("{0} : {1}",receiveMessage[0], receiveMessage[1]);
            
            ao.clearBuffer();
       
            ao.workingSocket.BeginReceive(ao.buffer, 0, ao.buffer.Length,
                   SocketFlags.None, async_receive, ao);
        }
    }

}
