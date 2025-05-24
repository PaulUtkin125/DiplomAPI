namespace DiplomAPI.Models.Support
{
    public class CreateBrokerRequest
    {
        public IFormFile? file {  get; set; }
        public string NameBroker { get; set; }
        public string FullNameOfTheDirector { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string OKTMO { get; set; }
        public string Phone { get; set; }
        public string BusinessAddress { get; set; }
        public string Email { get; set; }
        public int UrisidikciiyId { get; set; }
    }
}
