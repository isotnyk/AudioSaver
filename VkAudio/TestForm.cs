using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;

namespace VkAudio
{
  public partial class TestForm : Form
  {
    private ChromiumWebBrowser _browser;
    private AudioFileController _audioFileController;

    public TestForm()
    {
      InitializeComponent();
      InitializeChromium();
      InitializeFileManager();
    }

    private void InitializeChromium()
    {
      var settings = new CefSettings{CachePath = "Cache"};
      Cef.Initialize(settings);
      _browser = new ChromiumWebBrowser("https://vk.com/audios13087020") {RequestHandler = new VkRequestHandler{OnAudioDataDownoaded = OnAudioDataDownloaded}};
      Controls.Add(_browser);
      _browser.Dock = DockStyle.Fill;

      var obj = new BoundObject(_browser);
      obj.HtmlItemClicked += Obj_HtmlItemClicked;
      _browser.RegisterJsObject("bound", obj);
      _browser.FrameLoadEnd += obj.OnFrameLoadEnd;
    }

    private void InitializeFileManager()
    {
      _audioFileController = new AudioFileController();
    }

    private void OnAudioDataDownloaded(long length)
    {
      if (!_audioFileController.HasWaitingItem)
      {
        _browser.ExecuteScriptAsync("alert(\"Waiting item is null. Reload the application.\"");
        return;
      }

      _audioFileController.UpdateAudioLength(length);
    }

    private void Obj_HtmlItemClicked(object sender, HtmlItemClickedEventArgs e)
    {
      if (_audioFileController.HasWaitingItem)
      {
        _browser.ExecuteScriptAsync("alert(\"Please wait while the audio will be downloaded\"");
        return;
      }

      var dataArray = e.Data.Split(new [] {","}, StringSplitOptions.None);
      _audioFileController.QueueItem(dataArray[4], dataArray[3]);
    }

    private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Cef.Shutdown();
    }

    //private async void OnAudioDataLoaded(Stream stream)
    //{
    //  await Task.Run(() => Do(stream));
    //}

    //private void Do(Stream stream)
    //{
    //  if (_audioInfoObject == null)
    //  {
    //    MessageBox.Show(@"_audio object is null");
    //    return;
    //  }

    //  var buffer = new byte[32768];
    //  using (var fileStream = File.Create(String.Format(@"D:\VkAudio\{0} - {1}.mp3", _audioInfoObject.Author, _audioInfoObject.Name)))
    //  {
    //    while (true)
    //    {
    //      var read = stream.Read(buffer, 0, buffer.Length);
    //      if (read <= 0)
    //        break;
    //      fileStream.Write(buffer, 0, read);
    //    }
    //  }

    //  _audioInfoObject = null;
    //}
  }
}
