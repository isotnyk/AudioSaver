using CefSharp;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VkAudio
{
  public static class RequestHelper
  {
    public static async Task<Stream> MakeAsyncRequest(IRequest request)
    {
      var r = (HttpWebRequest)WebRequest.Create(request.Url);
      r.Method = "Get";
      r.MediaType = "audio";

      for (var i = 0; i < request.Headers.Count; i++)
      {
        var key = request.Headers.GetKey(i);
        var value = request.Headers[key];

        bool assigned = true;
        switch (key)
        {
          case "Accept":
            r.Accept = value;
            break;
          case "User-Agent":
            r.UserAgent = value;
            break;
          case "Range":
            {
              var startRegex = new Regex("=[0-9]+");
              var endRegex = new Regex("=[0-9]+");

              var startStr = startRegex.IsMatch(value) ? startRegex.Match(value).Value.Remove(0, 1) : "";
              var endStr = endRegex.IsMatch(value) ? endRegex.Match(value).Value.Remove(0, 1) : "";

              if (!string.IsNullOrEmpty(endStr))
                r.AddRange(Convert.ToInt32(startStr), Convert.ToInt32(endStr));
              else
                r.AddRange(Convert.ToInt32(startStr));

              break;
            }
          default:
            assigned = false;
            break;
        }

        if (!assigned)
          r.Headers.Add(key, value);
      }

      var task = Task.Factory.FromAsync(r.BeginGetResponse, asyncResult => r.EndGetResponse(asyncResult), null);
      return await task.ContinueWith(t => ReadStreamFromResponse(t.Result));
    }

    private static Stream ReadStreamFromResponse(WebResponse response)
    {
      if (response.Headers["Content-Type"] != null && response.Headers["Content-Type"].Contains("audio/mpeg"))
      {
        return response.GetResponseStream();
      }

      return null;
    }
  }
}
