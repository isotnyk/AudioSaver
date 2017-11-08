using CefSharp;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace VkAudio
{
  public class VkRequestHandler : IRequestHandler
  {
    public delegate void AudioDataLoaded(Stream audioStream);
    public AudioDataLoaded OnAudioDataLoaded { get; set; }

    public delegate void AudioDataDownloaded(long length);
    public AudioDataDownloaded OnAudioDataDownoaded { get; set; }


    public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
    {
      return false;
    }

    public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
    {
      return false;
    }

    public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
    {
      callback.Dispose();
      return false;
    }

    public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
    {
    }

    public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
    {
      //if (!request.Url.Contains(".mp3?extra"))
      //{
      //  callback.Dispose();
      //  return CefReturnValue.Continue;
      //}

      //var task = RequestHelper.MakeAsyncRequest(request);
      //if (OnAudioDataLoaded != null)
      //   OnAudioDataLoaded(task.Result);

      //callback.Dispose();
      return CefReturnValue.Continue;
    }

    public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
    {
      return false;
    }

    public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
    {
      callback.Dispose();
      return false;
    }

    public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
    {
    }

    public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
    {
      callback.Dispose();
      return false;
    }

    public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response,
      ref string newUrl)
    {
    }

    public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
    {
      return false;
    }

    public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
    {
    }

    public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
    {
      return false;
    }

    public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame,
    IRequest request, IResponse response)
    {
      return null;
    }

    public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
    IResponse response, UrlRequestStatus status, long receivedContentLength)
    {
      if (!request.Url.Contains(".mp3?extra"))
        return;

      if (OnAudioDataDownoaded != null)
        OnAudioDataDownoaded(receivedContentLength);
    }
  }
}
