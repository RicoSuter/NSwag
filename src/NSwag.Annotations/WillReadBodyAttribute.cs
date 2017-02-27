namespace NSwag.Annotations
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class WillReadBodyAttribute : Attribute
    {
        public WillReadBodyAttribute(bool willReadBody)
        {
            WillReadBody = willReadBody;
        }

        public bool WillReadBody { get; }
    }
}