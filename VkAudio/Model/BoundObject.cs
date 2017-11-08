using System;
using System.IO;
using CefSharp;
using CefSharp.WinForms;

namespace VkAudio.Model
{
  public class BoundObject
  {
    public delegate void ItemClickedEventHandler(object sender, HtmlItemClickedEventArgs e);
    public event ItemClickedEventHandler HtmlItemClicked;

    private string _script;
    private string AudioButtonSelectorScript { get { return _script ?? (_script = LoadScript()); } }

    private readonly ChromiumWebBrowser _browser;

    public BoundObject(ChromiumWebBrowser br)
    {
      _browser = br;
    }

    private string LoadScript()
    {
      var path = Path.GetFullPath(@"..\..\..\Scripts\AudioButtonClickHandler.js");
      using (var reader = new StreamReader(path))
        return reader.ReadToEnd();
    }

    public void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
    {
      if (e.Frame.IsMain)
      {
        _browser.EvaluateScriptAsync(AudioButtonSelectorScript);
      }
    }

    public void OnClicked(string data)
    {
      if (HtmlItemClicked != null)
        HtmlItemClicked(this, new HtmlItemClickedEventArgs { Data = data });
    }
  }

  public class HtmlItemClickedEventArgs : EventArgs
  {
    public string Data { get; set; }
  }
}
