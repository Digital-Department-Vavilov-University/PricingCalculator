using System.Collections.Generic;

namespace PricingCalculator
{
    public class HistoryManager
    {
        private List<HistoryEntry> history = new List<HistoryEntry>();

        public void AddToHistory(HistoryEntry entry)
        {
            history.Add(entry);
        }

        public List<HistoryEntry> GetHistory()
        {
            return history;
        }
    }
}