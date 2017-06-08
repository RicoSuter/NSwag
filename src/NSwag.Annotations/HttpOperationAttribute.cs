using System;

namespace NSwag.Annotations
{
    public class HttpOperationAttribute: Attribute
    {
        public string Method { get; }
        public string Path { get; }

        public HttpOperationAttribute(string method = null, string path = null)
        {
            Method = method;
            Path = path;
        }
    }
}