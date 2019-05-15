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
        private const string UserName = "your_bot_email";

        // Look up for the code in the browser url after the registration
        private const string UserCode = "your_bot_code";

        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            var serverConnector = new ServerConnector(
                new MyTetrisBot(),
                new DefaultLogger(),
                $"ws://{ServerNameAndPort}/contest/ws?user={UserName}&code={UserCode}");

            Task.WaitAll(new[]
            {
                serverConnector.Start()
            });

            Console.WriteLine("Program finished");
        }
    }
}
