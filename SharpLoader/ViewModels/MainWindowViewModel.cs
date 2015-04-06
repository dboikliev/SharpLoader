using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SharpLoader.Commands;
using SharpLoader.Services;

namespace SharpLoader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //private readonly IDialogService dialogService;
        //private readonly IUrlService urlService;
        
        //private string videoUrl;

        //public string VideoUrl
        //{
        //    get { return videoUrl; }
        //    set
        //    {
        //        videoUrl = value;
        //        OnPropertyChanged("VideoUrl");
        //    }
        //}

        //public ObservableCollection<DownloadViewModel> Downloads { get; set; } 

        //public ICommand AddNewDownload { get; set; }

        //public MainWindowViewModel()
        //{
        //    dialogService = new DialogService();
        //    urlService = new UrlService();

        //    AddNewDownload = new RelayCommandWithParameter<string>(ExecuteAddNewDownload, CanExecutedAddNewDownload);
        //}

        //private void ExecuteAddNewDownload(string videoUrl)
        //{
        //    var downloadLocation = dialogService.ShowSaveFileDialog();
        //    var download = new DownloadViewModel();
        //    Downloads.Add(download);
        //    download.DownloadCommand.Execute(videoUrl);
        //}

        //private bool CanExecutedAddNewDownload(string videoUrl)
        //{
        //    var isValidUrl = urlService.IsValidUrl(videoUrl);
        //    return isValidUrl;
        //}
    }
}
