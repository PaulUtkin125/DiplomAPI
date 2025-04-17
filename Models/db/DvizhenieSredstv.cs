namespace Finansu.Model
{
    public class DvizhenieSredstv
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now.Date;

        public InvestTools InvestTools { get; set; }
        public int InvestToolsId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public double Money { get; set; } = 0;
    }
}
