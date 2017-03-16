using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using NJsonSchema;
using NSwag;

namespace NSwagStudio.Views.CodeGenerators
{
    public abstract class CodeGeneratorViewBase : UserControl
    {
        public abstract string Title { get; }

        public string PropertyName => ConversionUtilities.ConvertToLowerCamelCase(GetType().Name, false);

        public abstract Task GenerateClientAsync(SwaggerDocument document, string documentPath);

        public abstract bool IsSelected { get; set; }

        public virtual bool IsPersistent => false;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}