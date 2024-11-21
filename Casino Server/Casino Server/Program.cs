using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CasinoLibrary;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            TCPServer MMMServer = new TCPServer();
            PlayerInfo player = new PlayerInfo();

            while (MMMServer.connection)
            {
                MMMServer.packet = MMMServer.receivePacket();
                MMMServer.runProtocol(player);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }
}