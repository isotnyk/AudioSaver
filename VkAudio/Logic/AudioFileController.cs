using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using VkAudio.Model;

namespace VkAudio.Logic
{
  public class AudioFileController : IDisposable
  {
    private const string DestinationFolderPath = @"D:\VkAudioNew";
    private const int WatcherTimeout = 500; //ms

    private readonly object _locker;
    private readonly DirectoryInfo _cacheDirInfo;
    private readonly string _whitespace = string.Intern(" ");
    private readonly string _ampersand = string.Intern("&");

    private Thread _fileWatcherThread;
    private AudioQueueItem _waitingItem;

    private delegate void FileCreatedHandler(string filePath);
    private event FileCreatedHandler OnFileCreated;

    public bool HasWaitingItem { get { return _waitingItem != null; } }

    public AudioFileController()
    {
      _locker = new object();
      _cacheDirInfo = new DirectoryInfo("Cache//Cache");
      _fileWatcherThread = new Thread(FileWatcherThreadProc) { IsBackground = true };
      OnFileCreated += FileWatcher_OnFileCreated;

      _fileWatcherThread.Start();
    }

    private void FileWatcherThreadProc()
    {
      while (true)
      {
        if (!HasWaitingItem || _waitingItem.DataLength == 0)
        {
          Thread.Sleep(WatcherTimeout);
          continue;
        }

        var suitableFile = _cacheDirInfo.GetFiles().FirstOrDefault(f => f.Length == _waitingItem.DataLength);
        if (suitableFile == null)
        {
          Thread.Sleep(WatcherTimeout);
          continue;
        }

        if (OnFileCreated != null)
          OnFileCreated(suitableFile.FullName);

        Thread.Sleep(WatcherTimeout);
      }
    }

    public void UpdateAudioLength(long length)
    {
      lock (_locker)
      {
        _waitingItem.DataLength = length;
      }
    }

    public bool QueueItem(string rawAuthor, string rawName)
    {
      lock (_locker)
      {
        if (_waitingItem != null)
          return false;

        _waitingItem = new AudioQueueItem
        {
          Author = ClearEscapes(rawAuthor),
          Name = ClearEscapes(rawName)
        };

        return true;
      }
    }

    private void FileWatcher_OnFileCreated(string filePath)
    {
      lock (_locker)
      {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length != _waitingItem.DataLength)
          return;

        var destinationFileName = string.Format("{0} - {1}.mp3", _waitingItem.Author, _waitingItem.Name);
        var destinationFilePath = Path.Combine(DestinationFolderPath, destinationFileName);
        if (!File.Exists(destinationFilePath))
          fileInfo.MoveTo(destinationFilePath);

        _waitingItem = null;
      }
    }

    private string ClearEscapes(string source)
    {
      //TODO: Write a method to remove forbidden symbols by one in the loop
      var cleanedString = source
        .Replace("/", string.Empty)
        .Replace("\"", string.Empty)
        .Replace("&amp;", _ampersand)
        .Replace("|", _whitespace);

      return Regex.Unescape(cleanedString);
    }

    public void Dispose()
    {
      _fileWatcherThread.Abort();
      _fileWatcherThread = null;
    }
  }
}
