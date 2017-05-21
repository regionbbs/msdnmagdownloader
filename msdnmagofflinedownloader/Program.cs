using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace msdnmagofflinedownloader
{
    class Program
    {
        private static readonly WebClient webclient = new WebClient();

        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(webclient.DownloadString("https://msdn.microsoft.com/magazine/msdn-magazine-issues"));

            // find pdfs and chms.
            var items = doc.DocumentNode.SelectNodes("//a[@href]");

            foreach (var item in items)
            {
                Uri url;

                try
                {
                    url = new Uri(item.Attributes["href"].Value);
                }
                catch (Exception)
                {
                    Console.WriteLine($"URL format is invalid: {item.Attributes["href"].Value}");
                    continue;
                }

                if (item.Attributes["href"].Value.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                    Download(url);
                if (item.Attributes["href"].Value.EndsWith(".chm", StringComparison.InvariantCultureIgnoreCase))
                    Download(url);
            }

            Console.WriteLine("Download completed.");
        }

        private static void Download(Uri url)
        {
            Console.WriteLine($"Start downloading file {url}");

            try
            {
                webclient.DownloadFile(url, Path.GetFileName(url.LocalPath));
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
                Console.WriteLine($"Can't download file {url} with HTTP status code {response.StatusCode}");
            }
        }
    }
}
