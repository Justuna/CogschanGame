using Cysharp.Threading.Tasks;
using System.Threading;

public struct OneShotTask
{
    public delegate void TaskDelegate(CancellationToken token);
    public delegate UniTask UniTaskDelegate(CancellationToken token);
    public TaskDelegate TaskFunc { get; private set; }
    public UniTaskDelegate UniTaskFunc { get; private set; }
    private CancellationTokenSource _cancellationTokenSource;

    public OneShotTask(UniTaskDelegate taskFunc = null)
    {
        TaskFunc = null;
        UniTaskFunc = taskFunc;
        _cancellationTokenSource = null;
    }

    public OneShotTask(TaskDelegate taskFunc = null)
    {
        TaskFunc = taskFunc;
        UniTaskFunc = null;
        _cancellationTokenSource = null;
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void Run()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        if (UniTaskFunc != null)
            UniTaskFunc(_cancellationTokenSource.Token);
        else
            TaskFunc(_cancellationTokenSource.Token);
    }

    public async UniTask RunAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        await UniTaskFunc(_cancellationTokenSource.Token);
    }
}
