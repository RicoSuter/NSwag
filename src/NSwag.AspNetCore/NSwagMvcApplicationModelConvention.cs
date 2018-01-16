using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace NSwag.AspNetCore
{
    internal class NSwagMvcApplicationModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            application.ApiExplorer.IsVisible = true;
        }
    }
}
