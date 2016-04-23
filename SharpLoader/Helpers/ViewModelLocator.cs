using SharpLoader.DependencyInjection;
using SharpLoader.ViewModels;

namespace SharpLoader.Helpers
{
    partial class ViewModelLocator
    {
        public AllDownloadsViewModel AllDownloadsViewModel => DependencyResolver.Instance.Resolve<AllDownloadsViewModel>();
    }
}
