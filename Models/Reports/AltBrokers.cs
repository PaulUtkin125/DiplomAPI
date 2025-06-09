namespace DiplomAPI.Models.Reports
{
    public class AltBrokers
    {
        public int Id { get; set; }
        public string NameBroker { get; set; }

        public string SourseFile { get; set; }
        public bool IsClosing { get; set; } = false;

        public string FullNameOfTheDirector { get; set; }

        public long INN { get; set; }
        public long KPP { get; set; }
        public long OKTMO { get; set; }
        public string Phone { get; set; }
        public string BusinessAddress { get; set; }
        public string Email { get; set; }
        public bool isAdmitted { get; set; } = false;
        public DateOnly dateSubmitted { get; set; }

        public string TypeOfRequest { get; set; }

        public string? Password { get; set; }
    }
}
