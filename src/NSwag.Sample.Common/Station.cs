namespace NSwag.Sample.Common
{
    public class Station
    {
        public string Name { get; set; }

        public ExtensionData Data { get; } = new ExtensionData();
    }
}