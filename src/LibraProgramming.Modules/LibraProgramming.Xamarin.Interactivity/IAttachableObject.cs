using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public interface IAttachableObject
    {
        BindableObject AttachedObject
        {
            get;
        }

        void Attach(BindableObject bindable);

        void Detach();
    }
}
