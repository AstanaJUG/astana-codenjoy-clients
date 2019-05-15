namespace TetrisClient.Bot
{
    public interface ICommandProcessor
    {
        string GetResponse(string commandString);
    }
}