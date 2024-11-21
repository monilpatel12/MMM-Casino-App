using System.Net.Sockets;
using System.Net;
using System.Text;
using CasinoLibrary;

namespace Casino_Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                
                TCPClient MMMClient = new TCPClient();
                PlayerInfo player = new PlayerInfo();

                ApplicationConfiguration.Initialize();
                Application.Run(new User_Authentication(player, MMMClient));

                MMMClient.shutDown();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}