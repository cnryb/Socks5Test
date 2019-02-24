using Socks5Test.Core;
using System;

namespace Socks5Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //TCPSocks5Server server = new TCPSocks5Server(8832, "test", "test");
            TCPSocks5Server server = new TCPSocks5Server();
            server.Start();
            Console.WriteLine("如果要停止服务,请按回车键!!");
            Console.ReadLine();
            server.Stop();
        }
    }
}
