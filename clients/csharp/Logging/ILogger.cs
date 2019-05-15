namespace TetrisClientCore.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);

        void LogError(string error);
    }
}