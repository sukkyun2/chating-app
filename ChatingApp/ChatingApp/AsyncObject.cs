using System;
using System.Net.Sockets;

namespace ChatingApp
{
    class AsyncObject
    {
        public Byte[] buffer;
        public Socket workingSocket;

        public AsyncObject(int bufferSize)
        {
            this.buffer = new Byte[bufferSize];
        }
        
        public void clearBuffer()
        {
            //buffer = Encoding.Unicode.GetBytes(string.Empty);
            Array.Clear(buffer, 0, buffer.Length);
        }

    }
}
