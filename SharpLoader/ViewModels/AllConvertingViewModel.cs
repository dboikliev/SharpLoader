using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace SharpLoader.ViewModels
{
    public class AllConvertingViewModel : ViewModelBase
    {

        public ObservableCollection<ConvertViewModel> Converting { get; } = new ObservableCollection<ConvertViewModel>();

        
        public async void BeginConverting(string title, BitmapImage thumbnail, string fileName, string resultFileName, string format)
        {
            var convert = new ConvertViewModel() { Title = title, Thumbnail = thumbnail};
            Converting.Add(convert);
            convert.Convert(fileName, resultFileName);
        }

    }
}
