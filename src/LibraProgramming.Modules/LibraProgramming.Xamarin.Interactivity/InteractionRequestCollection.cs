using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public class InteractionRequestCollection : AttachableBindableObjectCollection<RequestTrigger>, ILifetimeTarget
    {
        private WeakLifetimeObjectObserver observer;
        private bool isLoaded;

        internal InteractionRequestCollection()
        {
        }

        public override void Attach(BindableObject bindable)
        {
            base.Attach(bindable);

            observer = new WeakLifetimeObjectObserver(bindable as Page, this);
        }

        internal override void DoItemAdded(RequestTrigger trigger)
        {
            if (null == AttachedObject)
            {
                return;
            }

            trigger.Attach(AttachedObject);
        }

        internal override void DoItemRemoved(RequestTrigger trigger)
        {
            if (null == trigger.AttachedObject)
            {
                return;
            }

            trigger.Detach();
        }

        void ILifetimeTarget.OnAppearing(Page page)
        {
            if (null != AttachedObject || isLoaded)
            {
                return;
            }

            isLoaded = true;

            /*if (!ApplicationModel.DesignMode.DesignModeEnabled)
            {
                AttachedObject = element;
            }*/

            Attach(page);
        }

        void ILifetimeTarget.OnDisappearing(Page page)
        {
            if (isLoaded)
            {
                isLoaded = false;
                Detach();
            }
        }

        protected override void DoAttach(BindableObject bindable)
        {
            foreach (var trigger in this)
            {
                trigger.Attach(bindable);
            }
        }

        protected override void DoDetach()
        {
            //observer = null;
        }
    }
}
