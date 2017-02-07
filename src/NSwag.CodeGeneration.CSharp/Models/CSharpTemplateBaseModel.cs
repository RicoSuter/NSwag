namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>Base class for the CSharp models</summary>
    public abstract class CSharpTemplateBaseModel
    {
        /// <summary>Gets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapSuccessResponses { get; set; }

        /// <summary>Gets the response class name.</summary>
        public string ResponseClass { get; protected set; }
    }
}
