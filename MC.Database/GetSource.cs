using System;
using System.IO;
using System.Net;
using System.Text;

namespace MC.Database {
    public class GetSource {
        public static string Get(string url) {
            try {
                var hwr = (HttpWebRequest) WebRequest.Create(url);
                hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
                hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
                hwr.KeepAlive = true;
                hwr.AutomaticDecompression = DecompressionMethods.Deflate |
                                             DecompressionMethods.GZip;
                var feed = "";
                using (var resp = (HttpWebResponse) hwr.GetResponse()) {
                    using (var s = resp.GetResponseStream()) {
                        var cs = string.IsNullOrEmpty(resp.CharacterSet) ? "UTF-8" : resp.CharacterSet;
                        var e = Encoding.GetEncoding(cs);
                        if (s == null) return feed;
                        using (var sr = new StreamReader(s, e)) {
                            feed = sr.ReadToEnd();
                        }
                    }
                }
                return feed;
            }
            catch (Exception e) {
                DebugText.Write($"{url}\n{e.Message}");
                return null;
            }
        }
    }
}