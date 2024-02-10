namespace L2Dn;

public static class ConsoleUtility
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
