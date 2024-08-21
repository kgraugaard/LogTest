// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory();

using (FileLog fileLogger = new FileLog(path + "\\log.txt"))
{
    for (int i = 0; i <= 10000; i++)
    {
        fileLogger.Add($"Log Item {i}");
    }
}

