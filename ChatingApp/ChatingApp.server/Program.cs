using System;
using System.Net;
using System.Net.Sockets;

namespace ChatingApp.server
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer();
            server.StartServer();

            while (true)
            {
                Console.WriteLine("서버실행중....");
                Console.WriteLine("---------------");
                Console.WriteLine("|이름 : 메시지|");
                Console.WriteLine("---------------");
                Console.ReadKey();
            }
        }
    }

   
}
