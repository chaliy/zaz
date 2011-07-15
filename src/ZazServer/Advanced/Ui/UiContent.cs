using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Zaz.Server.Advanced.Ui
{
    public static class UiContent
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

            var resourceName = "Zaz.Server.Advanced.Ui." + path.Replace("/", ".");
            var raw = typeof (UiContent).Assembly.GetManifestResourceStream(resourceName);
            var content = new StreamContent(raw);

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
