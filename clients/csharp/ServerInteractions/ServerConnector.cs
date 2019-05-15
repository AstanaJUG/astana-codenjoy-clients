using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetrisClient.Bot;
using TetrisClient.Logging;

namespace TetrisClient.ServerInteractions
{
    public class ServerConnector
    {
        private const int ReceiveChunkSize = 1024 * 10;
        private static readonly Encoding Encoder = new UTF8Encoding(false);

        private readonly ICommandProcessor _commandProcessor;

        private readonly ILogger _logger;

        public ServerConnector(ICommandProcessor commandProcessor, ILogger logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        public async Task Connect(string uri)
        {
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                await Receive(webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                _logger.LogError("Websocket closed");
            }
        }

        private async Task Send(ClientWebSocket webSocket, string response)
        {
            byte[] buffer = Encoder.GetBytes(response);
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            LogStatus(false, buffer, buffer.Length);
        }

        private async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[ReceiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        closeStatus: WebSocketCloseStatus.NormalClosure,
                        statusDescription: string.Empty,
                        cancellationToken: CancellationToken.None);
                }
                else
                {
                    for (int i = result.Count; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }

                    LogStatus(true, buffer, result.Count);

                    var commandString = Encoder.GetString(buffer, 0, result.Count);
                    if (Enum.TryParse(commandString, out TetrisMoveCommand command))
                    {
                        await Send(webSocket, _commandProcessor.GetResponse(command));
                    }
                    else
                    {
                        _logger.LogError($"Could not parse command: {commandString}");
                    }
                }
            }
        }

        private void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (_consoleLocker)
            {
                if (!receiving)
                {
                    Console.Clear();
                    Console.Write(DateTime.Now);
                    Console.Write("  ");

                    Console.WriteLine(Mybot.HeadlineText);
                    Console.WriteLine(Mybot.DisplayText);
                    Console.WriteLine(Mybot.CommandText);
                }

                Console.ResetColor();
            }
        }
    }
}