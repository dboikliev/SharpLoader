using Microsoft.Practices.Unity;

namespace SharpLoader.DependencyInjection
{
    public sealed class DependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        public static DependencyResolver Instance { get; private set; }

        static DependencyResolver()
        {
            Instance = new DependencyResolver();
        }

        private DependencyResolver()
        {
            _container = new UnityContainer();
        }

        public T Resolve<T>()
        {
            var resolvedObject = _container.Resolve<T>();
            return resolvedObject;
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>();
        }
    }
}
