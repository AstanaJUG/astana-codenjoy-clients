﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetrisClient.Bot;

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

        private static readonly object _consoleLocker = new object();
        private const int ReceiveChunkSize = 1024 * 10;
        private static readonly Encoding Encoder = new UTF8Encoding(false);

        private static readonly MyTetrisBot Mybot = new MyTetrisBot();

        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            Connect($"ws://{ServerNameAndPort}/contest/ws?user={UserName}&code={UserCode}").Wait();
        }

        public static async Task Connect(string uri)
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
                webSocket?.Dispose();
                Console.WriteLine();
            }
        }

        private static async Task Send(ClientWebSocket webSocket, TetrisMoveCommand command)
        {

            byte[] buffer = Encoder.GetBytes(command.ToString());
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            LogStatus(false, buffer, buffer.Length);
        }

        private static async Task Receive(ClientWebSocket webSocket)
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
                    TetrisMoveCommand command = Mybot.Process(Encoder.GetString(buffer, 0, result.Count));

                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: {command}");
                    await Send(webSocket, command);
                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
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
