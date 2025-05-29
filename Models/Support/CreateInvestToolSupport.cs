namespace DiplomAPI.Models.Support
{
    public class CreateInvestToolSupport
    {
        public IFormFile file { get; set; }
        public string BrokersId { get; set; }
        public string NameInvestTool { get; set; }
        public string Price { get; set; }
    }
}
