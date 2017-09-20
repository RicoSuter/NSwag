//-----------------------------------------------------------------------
// <copyright file="FromSwaggerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    /// <summary>Reads a Swagger specification from JSON or an URL.</summary>
    public class FromSwaggerCommand : OutputCommandBase, INotifyPropertyChanged
    {
        private string _swagger;
        private string _url = "http://petstore.swagger.io/v2/swagger.json";

        /// <summary>Gets or sets the input Swagger specification.</summary>
        [JsonProperty("json", NullValueHandling = NullValueHandling.Ignore)]
        public string Swagger
        {
            get { return _swagger; }
            set
            {
                _swagger = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the input Swagger specification URL.</summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Runs the asynchronous.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var document = await RunAsync();
            await TryWriteFileOutputAsync(host, () => document.ToJson()).ConfigureAwait(false);
            return document;
        }

        /// <summary></summary>
        public async Task<SwaggerDocument> RunAsync()
        {
            if (!string.IsNullOrEmpty(Swagger))
                return await SwaggerDocument.FromJsonAsync(Swagger).ConfigureAwait(false);
            else if (Url.StartsWith("http://") || Url.StartsWith("https://"))
                return await SwaggerDocument.FromUrlAsync(Url).ConfigureAwait(false);
            else
                return await SwaggerDocument.FromFileAsync(Url).ConfigureAwait(false);
        }

        /// <summary>Occurs when property changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Called when property changed.</summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}