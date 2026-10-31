using System;
using System.IO;

namespace LSlife;

/// <summary>Journal centralise, independant du HUD DEBUG.</summary>
internal static class LSLifeLog
{
    private static readonly object Sync = new object();
    private const string DirectoryPath = @"scripts\LSLife";
    private const string LogPath = @"scripts\LSLife\LSLife.log";
    private const long MaxBytes = 5 * 1024 * 1024;

    public static void Info(string category, string message) { /* Les evenements normaux ne sont pas journalises. */ }
    public static void Warning(string category, string message) { Write("WARN", category, message, null); }
    public static void Error(string category, string message, Exception exception = null) { Write("ERROR", category, message, exception); }

    private static void Write(string level, string category, string message, Exception exception)
    {
        try
        {
            lock (Sync)
            {
                Directory.CreateDirectory(DirectoryPath);
                if (File.Exists(LogPath) && new FileInfo(LogPath).Length >= MaxBytes)
                {
                    string previous = Path.Combine(DirectoryPath, "LSLife.previous.log");
                    if (File.Exists(previous)) File.Delete(previous);
                    File.Move(LogPath, previous);
                }
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    " [" + level + "] [" + category + "] " + message;
                if (exception != null) line += Environment.NewLine + exception;
                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
