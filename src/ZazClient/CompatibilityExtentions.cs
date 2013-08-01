using System;
using System.Net.Http.Headers;

namespace Zaz.Client
{
    public static class CompatibilityExtentions
    {
        [Obsolete("Use direct HttpRequestHeaders methods instead. This compatibility hack will be removed in the future versions.")]
        public static void AddWithoutValidation(this HttpRequestHeaders h, string name, string value)
        {
            h.Add(name, value);
        }
    }
}