namespace NSwag.Annotations
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class WillReadBodyAttribute : Attribute
    {
        public WillReadBodyAttribute(bool willReadBody = false)
        {
            WillReadBody = willReadBody;
        }

        public bool WillReadBody { get; }
    }
}