using System;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal sealed class AppDomainIsolation<T> : IDisposable where T : MarshalByRefObject
    {
        private AppDomain _domain;
        private readonly T _object;

        public AppDomainIsolation()
        {
            var setup = new AppDomainSetup { ShadowCopyFiles = "true" }; 
            _domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);

            var type = typeof(T);
            _object = (T)_domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
        }

        public T Object
        {
            get { return _object; }
        }

        public void Dispose()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }
    }
}
