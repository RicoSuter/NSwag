namespace NSwag.AspNetCore.Middlewares
{
    internal class SwaggerDocumentKey
    {
        private readonly string _basePath;
        private readonly string _host;

        public SwaggerDocumentKey(string basePath, string host)
        {
            _basePath = basePath;
            _host = host;
        }

        protected bool Equals(SwaggerDocumentKey other)
        {
            return string.Equals(_basePath, other._basePath) && string.Equals(_host, other._host);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SwaggerDocumentKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_basePath != null ? _basePath.GetHashCode() : 0) * 397) ^ (_host != null ? _host.GetHashCode() : 0);
            }
        }
    }
}