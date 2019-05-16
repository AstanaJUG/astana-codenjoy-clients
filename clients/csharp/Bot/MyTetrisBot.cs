using System;
using TetrisClientCore.Game;

namespace TetrisClientCore.Bot
{
    internal class MyTetrisBot : ICommandProcessor
    {
        public string HeadlineText { get; private set; }

        public string DisplayText { get; private set; }

        public TetrisMoveCommand CommandText { get; private set; }

        public string GetResponse(string commandString)
        {
            HeadlineText = "";
            DisplayText = "";

            var board = new Board();
            board.Parse(commandString);
            DisplayText = board.Source;

            var random = new Random((int)DateTime.Now.Ticks);
            int move = random.Next(0, 3);
            switch (move)
            {
                case 0:
                    CommandText = TetrisMoveCommand.Right;
                    break;
                case 1:
                    CommandText = TetrisMoveCommand.Left;
                    break;
                case 2:
                    CommandText = TetrisMoveCommand.Down;
                    break;
                case 3:
                    CommandText = TetrisMoveCommand.Act;
                    break;
            }

            return CommandText.ToString();
        }
    }
}
