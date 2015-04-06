namespace SharpLoader.DependencyInjection
{
    public interface IDependencyResolver
    {
        T Resolve<T>();
        void RegisterType<TFrom, TTo>() where TTo : TFrom;
    }
}
