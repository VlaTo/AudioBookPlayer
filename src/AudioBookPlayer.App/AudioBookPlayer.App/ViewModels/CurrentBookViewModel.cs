using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class CurrentBookViewModel : ViewModelBase
    {
        public BookId Id { get; }

        public CurrentBookViewModel(BookId id)
        {
            Id = id;
        }
    }
}