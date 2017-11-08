using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;
using VkAudio.Logic;
using VkAudio.Model;

namespace VkAudio
{
  public partial class MainForm : Form
  {
    private ChromiumWebBrowser _browser;
    private AudioFileController _audioFileController;

    public MainForm()
    {
      InitializeComponent();
      InitializeChromium();
      InitializeFileManager();
    }

    private void InitializeChromium()
    {
      var settings = new CefSettings{CachePath = "Cache"};
      Cef.Initialize(settings);

      //TODO: resolve URL dynamically
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
        return;

      _audioFileController.UpdateAudioLength(length);
    }

    private void Obj_HtmlItemClicked(object sender, HtmlItemClickedEventArgs e)
    {
      if (_audioFileController.HasWaitingItem)
      {
        _browser.ExecuteScriptAsync("alert('Previous audio file is still downloading. Please wait until the file will be copied to the destination folder and try again.')");
        return;
      }

      var dataArray = e.Data.Split(new [] {","}, StringSplitOptions.None);
      _audioFileController.QueueItem(dataArray[4], dataArray[3]);
    }

    private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Cef.Shutdown();
      _audioFileController.Dispose();
    }
  }
}
