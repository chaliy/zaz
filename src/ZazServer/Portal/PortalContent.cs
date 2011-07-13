using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Zaz.Server.Portal
{
    public static class PortalContent
    {
        public static StreamContent Get(string path)
        {
            // index.html            
            // ccs/style.css

            if (String.IsNullOrEmpty(path))
            {
                path = "index.html";
            }

            var extension = Regex.Match(path, "\\.[^\\.\\?]*[^\\?]*").Value.ToLower();
            
            var content = new StreamContent(typeof (PortalContent).Assembly.GetManifestResourceStream("Zaz.Server.Portal." + path.Replace("/", ".")));

            switch (extension)
            {
                case ".css":
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/css");
                    break;

                case ".js":
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
                    break;
            }

            return content;
        }
    }
}
