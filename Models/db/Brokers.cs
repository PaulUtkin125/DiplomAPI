namespace Finansu.Model
{
    public class Brokers
    {
        public int Id { get; set; }
        public Urisidikciiy Urisidikciiy { get; set; }
        public int UrisidikciiyId { get; set; }
        public string NameBroker { get; set; }

        public string SourseFile { get; set; }
        public bool IsClosing { get; set; } = false;

    }
}
