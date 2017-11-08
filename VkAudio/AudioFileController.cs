using System;
using System.IO;
using System.Text.RegularExpressions;
using VkAudio.Model;

namespace VkAudio
{
  public class AudioFileController
  {
    private AudioQueueItem _waitingItem;
    private FileSystemWatcher _watcher ;

    public bool HasWaitingItem { get { return _waitingItem != null; } }

    public AudioFileController()
    {
      _watcher = new FileSystemWatcher("Cache//Cache");
      _watcher.NotifyFilter = NotifyFilters.CreationTime;
      _watcher.Created += FileWatcher_OnFileCreated;
    }

    public void UpdateAudioLength(long length)
    {
      _waitingItem.DataLength = length;
    }

    public void QueueItem(string rawAuthor, string rawName)
    {
      _waitingItem = new AudioQueueItem
      {
        Author = ClearEscapes(rawAuthor),
        Name = ClearEscapes(rawName)
      };
    }

    private void FileWatcher_OnFileCreated(object sender, FileSystemEventArgs e)
    {
      var fileInfo = new FileInfo(e.FullPath);
      if (fileInfo.Length != _waitingItem.DataLength)
        return;

      fileInfo.MoveTo(String.Format(@"D:\VkAudioNew\{0} - {1}.mp3", _waitingItem.Author, _waitingItem.Name));
      _waitingItem = null;
    }

    private static string ClearEscapes(string source)
    {
      return Regex.Unescape(source.Replace("\"", String.Empty).Replace("&amp;", "&").Replace("|", " "));
    }
  }
}
