using AudioBookPlayer.Data.Persistence.Entities;
using AudioBookPlayer.Domain.Models;

namespace AudioBookPlayer.Data.Persistence.Builders
{
    internal sealed class HistoryItemBuilder
    {
        public HistoryItem CreateHistoryItem(HistoryEntry entry)
        {
            return new HistoryItem();
        }
    }
}