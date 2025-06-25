using System;

namespace PricingCalculator
{
    public class PricingEngine
    {
        public double CalculateFinalPrice(Product product)
        {
            double totalCost = product.Cost + product.Packaging + product.Delivery;
            double overhead = totalCost * product.OverheadPercentage / 100;
            double priceWithOverhead = totalCost + overhead;
            return priceWithOverhead * (1 + product.Margin / 100);
        }

        public double AdjustForMarket(Product product, double calculatedPrice)
        {
            if (product.CompetitorPrice <= 0) return calculatedPrice;

            if (calculatedPrice < product.CompetitorPrice)
            {
                return Math.Min(calculatedPrice * 1.05, product.CompetitorPrice);
            }
            else if (calculatedPrice > product.CompetitorPrice)
            {
                return Math.Max(calculatedPrice * 0.95, product.CompetitorPrice);
            }
            else
            {
                return calculatedPrice;
            }
        }
    }
}