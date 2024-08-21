using System.Collections.Concurrent;
public class FileLog : IDisposable
{
    private readonly StreamWriter _fileLogWriter;
    private readonly BlockingCollection<string> _fileLogQueue = new BlockingCollection<string>();
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly Task _fileLogTask;

    public FileLog(string filePath)
    {
        // Autoflush = false optimziimng performance
        _fileLogWriter = new StreamWriter(filePath, append: true) { AutoFlush = false };
        _fileLogTask = Task.Factory.StartNew(() => GenerateLogQueue(_cancellationTokenSource.Token),
                                         TaskCreationOptions.LongRunning);
    }
    public void Add(string message)
    {
        _fileLogQueue.Add($"{DateTime.UtcNow:O} - {message}");
    }
    private void GenerateLogQueue(CancellationToken token)
    {
        try
        {
            foreach (var item in _fileLogQueue.GetConsumingEnumerable(token))
            {
                _fileLogWriter.WriteLine(item);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Opereation is cancelled");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An exception occured", ex.Message);
        }
    }

    public void Dispose()
    {
        //Done adding
        _fileLogQueue.CompleteAdding();
        // Wait for task to complete
        _fileLogTask.Wait();
        // Flush to disk
        _fileLogWriter.Flush();
        _fileLogWriter.Dispose();
        _fileLogQueue.Dispose();
    }
}