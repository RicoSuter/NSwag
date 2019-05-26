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

namespace NSwag.Commands.Generation
{
    /// <summary>Reads a Swagger specification from JSON or an URL.</summary>
    public class FromDocumentCommand : OutputCommandBase, INotifyPropertyChanged
    {
        private string _json;
        private string _url = "http://petstore.swagger.io/v2/swagger.json";

        /// <summary>Gets or sets the input Swagger specification.</summary>
        [JsonProperty("json", NullValueHandling = NullValueHandling.Ignore)]
        public string Json
        {
            get { return _json; }
            set
            {
                _json = value;
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
            await TryWriteDocumentOutputAsync(host, () => document).ConfigureAwait(false);
            return document;
        }

        /// <summary>Loads the Swagger spec.</summary>
        public async Task<OpenApiDocument> RunAsync()
        {
            var input = !string.IsNullOrEmpty(Json) ? Json : Url;
            return await ReadSwaggerDocumentAsync(input);
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