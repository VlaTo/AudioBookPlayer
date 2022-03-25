using Android.Views;

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class ChangeCoverDialogPresenter : DialogPresenter
    {
        public ChangeCoverDialogPresenter(DialogAccessor dialogAccessor)
            : base(dialogAccessor)
        {
        }

        public override void AttachView(View? view)
        {
            base.AttachView(view);
        }

        public override void DetachView()
        {
            base.DetachView();
        }
    }
}