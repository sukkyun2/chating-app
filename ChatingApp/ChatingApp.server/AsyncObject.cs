using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ChatingApp.server
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
            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}
