namespace TetrisClient.Bot
{
    public interface ICommandProcessor
    {
        string GetResponse(TetrisMoveCommand command);
    }
}