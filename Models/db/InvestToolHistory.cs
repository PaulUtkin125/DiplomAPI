namespace Finansu.Model
{
    public class InvestToolHistory
    {
        public int Id { get; set; }
        public DateTime DataIzmrneniiy { get; set; } = DateTime.Now;

        public InvestTools InvestTools { get; set; }
        public int InvestToolsId { get; set; }
        public double Price { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public double Quentity { get; set; }
    }
}
