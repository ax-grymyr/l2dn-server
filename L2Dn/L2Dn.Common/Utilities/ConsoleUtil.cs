namespace L2Dn.Utilities;

public static class ConsoleUtil
{
    public static Task WaitForCtrlC()
    {
        TaskCompletionSource taskCompletionSource = new TaskCompletionSource();

        Console.CancelKeyPress += (_, args) =>
        {
            args.Cancel = true;
            taskCompletionSource.SetResult();
        };

        return taskCompletionSource.Task;
    }
}