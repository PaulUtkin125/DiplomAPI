namespace Finansu.Model
{
    public class InvestTools
    {
        public int Id { get; set; }

        public Brokers Brokers { get; set; }
        public int BrokersId { get; set; }

        public string NameInvestTool { get; set; }
        public bool isClosed { get; set; } = false;

        public bool isFrozen { get; set; } = false;

        public double Price { get; set; }
        public string ImageSource { get; set; }
        public string TypeTool { get; set; }

    }
}
