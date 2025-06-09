namespace Finansu.Model
{
    public class Portfolio
    {
        public User User { get; set; }
        public int UserId { get; set; }

        public InvestTools InvestTool { get; set; }
        public int InvestToolId { get; set; }

        public double AllManey { get; set; }
        public int Quentity { get; set; }
    }
}
