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

            DoInitialize();
        }

        protected virtual void Initialize()
        {
            DependencyContainer = new DependencyContainer();

            RegisterTypesCore(DependencyContainer);

            if (null != platformInitializer)
            {
                platformInitializer.RegisterTypes(DependencyContainer);
            }
        }

        protected virtual void RegisterTypesCore(DependencyContainer container)
        {
        }

        private void DoInitialize()
        {
            Initialize();
        }
    }
}
