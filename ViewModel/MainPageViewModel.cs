using MediaApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MediaApp.ViewModel
{
    class MainPageViewModel
    {
        public static ObservableCollection<MusicFile> PlayList = new ObservableCollection<MusicFile>();
        public static MusicFile CurrentFile;
        public static int CurrentPosition = 0;
        public static Boolean IsPaused = false;
        public static Boolean StartUp = true;
        public static MediaElement RootMediaElement;
    }
}
