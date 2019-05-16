namespace TetrisClientCore.Bot
{
    public interface ICommandProcessor
    {
        string GetResponse(string commandString);
    }
}