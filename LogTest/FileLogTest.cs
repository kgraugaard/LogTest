using System.Reflection;
using Xunit;
public class FileLogTest
{
    private readonly string _testFile;
    private const int expected = 10000;
    public FileLogTest()
    {
        _testFile = GetTestPath("test_log.txt");

        if (File.Exists(_testFile))
            File.Delete(_testFile);

        //Prepare Test File
        var sut = new FileLog(_testFile);

        //log 10,000 entries
        for (int i = 1; i <= expected; i++)
        {
            sut.Add($"Log Item {i}");
        }

        sut.Dispose();
    }

    [Fact]
    public void Check_Entries_Count_And_Are_In_Order()
    {

        // Act: Read all lines from the log file
        var logItems = File.ReadAllLines(_testFile);

        // Assert: Verify the number of log entries
        Assert.Equal(expected, logItems.Length);

        // Assert: Verify the order of log entries
        for (int i = 1; i <= logItems.Length; i++)
        {
            string expectedMessage = $"Log Item {i}";
            Assert.Contains(expectedMessage, logItems[i-1]);
        }
    }

    public static string GetTestPath(string relativePath)
    {
        var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
        var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
        var dirPath = Path.GetDirectoryName(codeBasePath);
        return Path.Combine(dirPath, relativePath);
    }
}