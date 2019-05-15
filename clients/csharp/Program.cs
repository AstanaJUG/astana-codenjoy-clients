using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetrisClient.Bot;
using TetrisClient.Logging;
using TetrisClient.ServerInteractions;

namespace TetrisClient
{
    internal class Program
    {
        // Server name and port number -- ask orgs
        private const string ServerNameAndPort = "localhost:8080";

        // Register on the server, write down your registration name
        private const string UserName = "your_bot_email";

        // Look up for the code in the browser url after the registration
        private const string UserCode = "your_bot_code";

        private static readonly MyTetrisBot _myBot = new MyTetrisBot();

        private static ServerConnector _serverConnector;

        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            _serverConnector = new ServerConnector(
                _myBot,
                new DefaultLogger(),
                $"ws://{ServerNameAndPort}/contest/ws?user={UserName}&code={UserCode}");

            Task.WaitAll(new[]
            {
                _serverConnector.Start()
            });

            Console.WriteLine("Program finished");
        }
    }
}
