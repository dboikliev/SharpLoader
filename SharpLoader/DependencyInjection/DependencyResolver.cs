using Microsoft.Practices.Unity;

namespace SharpLoader.DependencyInjection
{
    public sealed class DependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public static DependencyResolver Instance { get; private set; }

        static DependencyResolver()
        {
            Instance = new DependencyResolver();
        }

        private DependencyResolver()
        {
            container = new UnityContainer();
        }

        public T Resolve<T>()
        {
            var resolvedObject = container.Resolve<T>();
            return resolvedObject;
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>();
        }
    }
}
