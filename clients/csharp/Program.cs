﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TetrisClient
{
    internal class Program
    {
        // Server name and port number -- ask orgs
        private static string ServerNameAndPort = "codenjoy.astanajug.net:8080";
        // Register on the server, write down your registration name
        private static string UserName = "your_bot_email";
        // Look up for the code in the browser url after the registration
        private static string UserCode = "your_bot_code";

        private static readonly object consoleLock = new object();
        private const int receiveChunkSize = 1024 * 10;
        private const bool verbose = true;
        private static readonly Encoding encoder = new UTF8Encoding(false);

        private static readonly MyTetrisBot mybot = new MyTetrisBot();

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
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                lock (consoleLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WebSocket closed.");
                    Console.ResetColor();
                }
            }
        }

        private static async Task Send(ClientWebSocket webSocket, string command)
        {

            byte[] buffer = encoder.GetBytes(command);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            LogStatus(false, buffer, buffer.Length);
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[receiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    for (int i = result.Count; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }

                    LogStatus(true, buffer, result.Count);
                    string command = mybot.Process(encoder.GetString(buffer, 0, result.Count));

                    Send(webSocket, command);
                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (consoleLock)
            {
                //if (verbose && receiving)
                //{
                //    Console.WriteLine(encoder.GetString(buffer, 0, length));
                //}

                if (verbose && !receiving)
                {
                    Console.Clear();
                    Console.Write(DateTime.Now.ToString());
                    Console.Write("  ");

                    Console.WriteLine(mybot.HeadlineText);
                    Console.WriteLine(mybot.DisplayText);
                    Console.WriteLine(mybot.CommandText);
                }

                Console.ResetColor();
            }
        }
    }
}
