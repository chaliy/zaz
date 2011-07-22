using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Zaz.Client.Avanced
{
    public static class JsonContentExtensions
    {
        public static T ReadAs<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(content.ReadAsString());
        }

        public static HttpContent Create(object cont)
        {
            return new StringContent(JsonConvert.SerializeObject(cont), Encoding.UTF8, "application/json");            
        }
    }
}
