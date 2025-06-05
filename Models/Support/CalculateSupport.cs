namespace DiplomAPI.Model.Support
{
    public class CalculateSupport
    {
        public int Id { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateFinish { get; set; }
        public int toolId { get; set; } = 0;
    }
}
