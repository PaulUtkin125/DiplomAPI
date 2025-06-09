namespace DiplomAPI.Models.db
{
    public class ApplicationHistory
    {
        public int Id { get; set; }
        public int mainId {  get; set; }
        public DateOnly dateEdit { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public TimeOnly timeEdit { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        public int UrisidikciiyId { get; set; }
        public string NameBroker { get; set; }
        public string SourseFile { get; set; }
        public bool IsClosing { get; set; }
        public string FullNameOfTheDirector { get; set; }
        public long INN { get; set; }
        public long KPP { get; set; }
        public long OKTMO { get; set; }
        public string Phone { get; set; }
        public string BusinessAddress { get; set; }
        public string Email { get; set; }
        public bool isAdmitted { get; set; } = false;
        public int TypeOfRequestId { get; set; }
        public DateOnly dateSubmitted { get; set; }
        public string? Password { get; set; }
    }
}
