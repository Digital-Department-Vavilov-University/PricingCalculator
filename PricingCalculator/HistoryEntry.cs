namespace PricingCalculator
{
    public class HistoryEntry
    {
        public string Name { get; set; }
        public double Cost { get; set; }
        public double Packaging { get; set; }
        public double Delivery { get; set; }
        public double OverheadPercentage { get; set; }
        public double Margin { get; set; }
        public double CompetitorPrice { get; set; }
        public double FinalPrice { get; set; }
        public int Stock { get; set; }         // Остаток на складе
        public double TotalValue { get; set; }   // Стоимость всех товаров
    }
}