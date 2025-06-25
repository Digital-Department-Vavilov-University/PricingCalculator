namespace PricingCalculator
{
    public class Product
    {
        public string Name { get; set; }
        public double Cost { get; set; }
        public double Packaging { get; set; }
        public double Delivery { get; set; }
        public double OverheadPercentage { get; set; }
        public double Margin { get; set; }
        public double CompetitorPrice { get; set; }
    }
}