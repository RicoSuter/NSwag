namespace NSwag.Integration.WebAPI.Models
{
    public class GenericRequest<TItem1, TItem2>
    {
        public TItem1 Item1 { get; set; }

        public TItem2 Item2 { get; set; }
    }
}