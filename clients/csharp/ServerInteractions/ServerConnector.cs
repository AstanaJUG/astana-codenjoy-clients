using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetrisClientCore.Bot;
using TetrisClientCore.Logging;

namespace TetrisClientCore.ServerInteractions
{
    public class ServerConnector
    {
        private const int ReceiveChunkSize = 1024 * 10;
        private static readonly Encoding Encoder = new UTF8Encoding(false);

        private readonly ICommandProcessor _commandProcessor;

        private readonly ILogger _logger;

        private readonly string _url;

        private bool _stop;

        public ServerConnector(ICommandProcessor commandProcessor, ILogger logger, string url)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
            _url = url;
            _stop = true;
        }

        public void Stop()
        {
            _stop = true;
        }

        public async Task Start()
        {
            _stop = false;
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(_url), CancellationToken.None);
                await StartListening(webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                webSocket?.Dispose();
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

            _logger.LogInfo("sent");
        }

        private async Task StartListening(ClientWebSocket webSocket)
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

                    _logger.LogInfo("received");

                    string commandString = Encoder.GetString(buffer, 0, result.Count);

                    await Send(webSocket, _commandProcessor.GetResponse(commandString));
                }
            }
        }
    }
}