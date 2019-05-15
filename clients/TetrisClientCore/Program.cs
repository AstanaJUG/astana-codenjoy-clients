using System;
using System.Threading;
using System.Threading.Tasks;
using TetrisClientCore.Bot;
using TetrisClientCore.Logging;
using TetrisClientCore.ServerInteractions;

namespace TetrisClientCore
{
    class Program
    {
        // Server name and port number -- ask orgs
        private const string ServerNameAndPort = "localhost:8080";

        // Register on the server, write down your registration name
        private const string UserName = "maximgorbatyuk191093@gmail.com";

        // Look up for the code in the browser url after the registration
        private const string UserCode = "6952362422072069772";

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
