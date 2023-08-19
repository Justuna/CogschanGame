using Cysharp.Threading.Tasks;
using System.Threading;

public class OneShotTask
{
    public delegate void TaskDelegate(CancellationToken token);
    public delegate UniTask UniTaskDelegate(CancellationToken token);
    public TaskDelegate TaskFunc { get; private set; }
    public bool IsRunning { get; private set; }
    public UniTaskDelegate UniTaskFunc { get; private set; }
    private CancellationTokenSource _cancellationTokenSource;

    public OneShotTask(UniTaskDelegate taskFunc = null)
    {
        TaskFunc = null;
        UniTaskFunc = taskFunc;
        _cancellationTokenSource = null;
        IsRunning = false;
    }

    public OneShotTask(TaskDelegate taskFunc = null)
    {
        TaskFunc = taskFunc;
        UniTaskFunc = null;
        _cancellationTokenSource = null;
        IsRunning = false;
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void Run()
    {
        IsRunning = true;
        RunAsync().Forget();
    }

    public async UniTask RunAsync()
    {
        IsRunning = true;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        await UniTaskFunc(_cancellationTokenSource.Token);
        IsRunning = false;
    }
}
