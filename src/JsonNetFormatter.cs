using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebApiContrib.Formatters.JsonNet
{
    /// Used this implementation: http://code.msdn.microsoft.com/Using-JSONNET-with-ASPNET-b2423706
    public class JsonNetFormatter2 : JsonMediaTypeFormatter
    {
        readonly Encoding _defaultEncoding = new UTF8Encoding();

        public JsonNetFormatter2()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() =>
            {
                var serializer = CreateSerializer();
                var streamReader = new StreamReader(readStream, _defaultEncoding);
                var jsonTextReader = new JsonTextReader(streamReader);
                var deserialized = serializer.Deserialize(jsonTextReader, type);
                return deserialized;
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var serializer = CreateSerializer();

                var streamWriter = new StreamWriter(writeStream, _defaultEncoding);
                var jsonTextWriter = new JsonTextWriter(streamWriter);

                serializer.Serialize(jsonTextWriter, value);

                jsonTextWriter.Flush();
                streamWriter.Flush();
            });
        }

        static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            serializer.Converters.Add(new StringEnumConverter());
            serializer.Converters.Add(new IsoDateTimeConverter());
            return serializer;
        }
    }


    /// <summary>
    /// Formats requests for text/json and application/json using Json.Net.
    /// </summary>
    /// <remarks>
    /// Christian Weyer is the author of this MediaTypeProcessor.
    /// <see href="http://weblogs.thinktecture.com/cweyer/2010/12/using-jsonnet-as-a-default-serializer-in-wcf-httpwebrest-vnext.html"/>
    /// Daniel Cazzulino (kzu): 
    ///		- updated to support in a single processor both binary and text Json. 
    ///		- fixed to support query composition services properly.
    /// Darrel Miller
    ///     - Converted to Preview 4 MediaTypeFormatter
    /// </remarks>
    [Obsolete("Use v2 instead")]
    class JsonNetFormatter : JsonMediaTypeFormatter
    {
        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        //        protected override 
        object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            var serializer = CreateSerializer();
            var reader = new JsonTextReader(new StreamReader(stream));

            var result = serializer.Deserialize(reader, type);

            return result;
        }

        //        protected override 
        void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            var serializer = CreateSerializer();
            // NOTE: we don't dispose or close these as they would 
            // close the stream, which is used by the rest of the pipeline.
            var writer = new JsonTextWriter(new StreamWriter(stream));

            serializer.Serialize(writer, value);
            writer.Flush();

        }

        private static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            serializer.Converters.Add(new StringEnumConverter());
            serializer.Converters.Add(new IsoDateTimeConverter());
            return serializer;
        }
    }
}
