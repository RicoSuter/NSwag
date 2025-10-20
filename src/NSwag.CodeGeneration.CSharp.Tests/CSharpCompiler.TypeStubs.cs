namespace System.Web.Http
{
    public class ApiController { }
    public abstract class ParameterBindingAttribute : global::System.Attribute
    {
        public abstract System.Web.Http.Controllers.HttpParameterBinding GetBinding(global::System.Web.Http.Controllers.HttpParameterDescriptor parameter);
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class HttpGetAttribute : global::System.Attribute { }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class HttpPostAttribute : global::System.Attribute { }
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromUriAttribute : global::System.Attribute { }
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromBodyAttribute : global::System.Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RouteAttribute : global::System.Attribute
    {
        public RouteAttribute(string template) { }
    }
}

namespace System.Web.Http.Controllers
{
    public abstract class HttpParameterBinding
    {
        protected HttpParameterBinding(global::System.Web.Http.Controllers.HttpParameterDescriptor parameter) { }
        public dynamic ActionArguments { get; }
        public dynamic Descriptor { get; }
        public abstract System.Threading.Tasks.Task ExecuteBindingAsync(global::System.Web.Http.Metadata.ModelMetadataProvider metadataProvider, global::System.Web.Http.Controllers.HttpActionContext actionContext, global::System.Threading.CancellationToken cancellationToken);
    }
    public abstract class HttpParameterDescriptor
    {
        public string ParameterName { get; }
        public dynamic ActionArguments { get; }
    }
    public abstract class HttpActionContext
    {
        public dynamic ActionArguments { get; }
        public dynamic Request { get; }
    }
}

namespace System.Web.Http.Metadata
{
    public abstract class ModelMetadataProvider { }
}
