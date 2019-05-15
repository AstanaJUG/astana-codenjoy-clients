namespace TetrisClientCore.Game
{
    internal class Board
    {
        public string Source { get; private set; }

        public void Parse(string input)
        {
            Source = input.StartsWith("board=") ? input.Substring(6) : input;
        }
    }
}
