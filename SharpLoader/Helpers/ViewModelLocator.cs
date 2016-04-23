using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpLoader.DependencyInjection;
using SharpLoader.ViewModels;

namespace SharpLoader.Helpers
{
    partial class ViewModelLocator
    {
        public AllDownloadsViewModel AllDownloadsViewModel => DependencyResolver.Instance.Resolve<AllDownloadsViewModel>();
    }
}
