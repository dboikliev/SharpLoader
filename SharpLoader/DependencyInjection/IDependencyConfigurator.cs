using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader.DependencyInjection
{
    public interface IDependencyConfigurator
    {
        void InitializeContainer();
    }
}
