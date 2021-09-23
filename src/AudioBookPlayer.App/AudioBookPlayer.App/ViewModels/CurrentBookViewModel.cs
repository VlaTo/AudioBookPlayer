using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class CurrentBookViewModel : ViewModelBase
    {
        public EntityId Id { get; }

        public CurrentBookViewModel(EntityId id)
        {
            Id = id;
        }
    }
}