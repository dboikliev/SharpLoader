using System.Net;
using System.Windows;

namespace SharpLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DependencyInjection.DependencyConfigurator.Instance.InitializeContainer();
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            base.OnStartup(e);
        }
    }
}
