using LibraProgramming.Xamarin.Dependency.Container;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    public class BaseApplication : Application
    {
        private readonly IPlatformInitializer platformInitializer;

        public DependencyContainer DependencyContainer
        {
            get;
            private set;
        }

        protected BaseApplication(IPlatformInitializer platformInitializer)
            : base()
        {
            this.platformInitializer = platformInitializer;
        }

        protected virtual void Initialize()
        {
            DependencyContainer = new DependencyContainer();

            if (null != platformInitializer)
            {
                platformInitializer.RegisterTypes(DependencyContainer);
            }
        }
    }
}
