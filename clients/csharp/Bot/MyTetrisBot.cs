using System;

namespace TetrisClient.Bot
{
    internal class MyTetrisBot
    {
        public string HeadlineText { get; private set; }

        public string DisplayText { get; private set; }

        public TetrisMoveCommand CommandText { get; private set; }

        public TetrisMoveCommand Process(string input)
        {
            HeadlineText = "";
            DisplayText = "";

            var board = new Board();
            board.Parse(input);
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

            return CommandText;
        }
    }
}
