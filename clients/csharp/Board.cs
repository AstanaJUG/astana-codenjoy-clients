using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetrisClient
{
    internal class Board
    {
        public string Source { get; private set; }

        public void Parse(string input)
        {
            if (input.StartsWith("board=")) {
                Source = input.Substring(6);
            } else {
                Source = input;
            }
        }
    }
}
