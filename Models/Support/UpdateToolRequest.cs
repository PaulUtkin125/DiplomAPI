namespace DiplomAPI.Models.Support
{
    public class UpdateToolRequest
    {
        public int id {  get; set; }
        public string? name { get; set; }
        public double price { get; set; }
        public IFormFile? file { get; set; }

    }
}
