using MediaApp.Model;
using MediaApp.View;
using MediaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MediaElement RootMediaElement;
        MusicProperties SongProperties;
        DispatcherTimer Timer = new DispatcherTimer();
        string NavigationParameter;
        
        public MainPage()
        {
            this.InitializeComponent();
            itemGridView.ItemsSource = MainPageViewModel.PlayList;
            Timer.Tick += Timer_Tick;
            Timer.Start();
            if (MainPageViewModel.StartUp)
            {
                Enabler(MainPageViewModel.StartUp);
            }
        }

        private void Enabler(Boolean Z)
        {

            btnPlay.IsEnabled = !Z;
            btnLyrics.IsEnabled = !Z;
            btnLyrics.IsEnabled = !Z;
            btnSearchBox.IsEnabled = !Z;
            btnFastForward.IsEnabled = !Z;
            btnNext.IsEnabled = !Z;
            btnRewind.IsEnabled = !Z;
            btnPrevious.IsEnabled = !Z;
        }

        private void Timer_Tick(object sender, object e)
        {
            if (RootMediaElement.CurrentState == MediaElementState.Paused && !MainPageViewModel.IsPaused)
            {
                btnNext_Click(sender, new RoutedEventArgs());

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationParameter = e.Parameter as string;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject rootGrid = VisualTreeHelper.GetChild(Window.Current.Content, 0);
            RootMediaElement = (MediaElement)VisualTreeHelper.GetChild(rootGrid, 0);
            if (NavigationParameter == "Search")
                ChangeMediaPlayerSongFromSearch();
        }

        private void btnSearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            Frame.Navigate(typeof(SearchResultsPage), args.QueryText);
        }

        #region Adding and Changing Songs
        private async void AddSongs()
        {
            FileOpenPicker OpenMusicFiles = new FileOpenPicker();
            OpenMusicFiles.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            OpenMusicFiles.FileTypeFilter.Add(".mp3");
            OpenMusicFiles.FileTypeFilter.Add(".wav");
            OpenMusicFiles.FileTypeFilter.Add(".mp4");
            OpenMusicFiles.FileTypeFilter.Add(".avi");
            var MusicFiles = await OpenMusicFiles.PickMultipleFilesAsync();
            if (MusicFiles.Count != 0)
            {
                foreach (var MusicFile in MusicFiles)
                {
                    BitmapImage alburmArt = new BitmapImage();
                    using (StorageItemThumbnail thumbNail = await MusicFile.GetThumbnailAsync(ThumbnailMode.MusicView, 300))
                    {
                        if (thumbNail != null && thumbNail.Type == ThumbnailType.Image)
                        {

                            alburmArt.SetSource(thumbNail);
                        }
                        else
                        {
                            Uri ImageName = new Uri("ms-appx:///Assets/NoAlbumArt.png");
                            alburmArt = new BitmapImage(ImageName);
                        }
                    }
                    SongProperties = await MusicFile.Properties.GetMusicPropertiesAsync();
                    MainPageViewModel.PlayList.Add(new MusicFile(MusicFile, MusicFile.Name, SongProperties.Title, SongProperties.Artist, SongProperties.Album, alburmArt, SongProperties.Year.ToString()));
                }
                itemGridView.ItemsSource = MainPageViewModel.PlayList;
                itemGridView.Focus(FocusState.Pointer);
                if (RootMediaElement.CurrentState == MediaElementState.Closed)
                    ChangeMediaPlayerSong();
                if (MainPageViewModel.StartUp && MainPageViewModel.PlayList.Count != 0)
                {
                    MainPageViewModel.StartUp = false;
                    Enabler(MainPageViewModel.StartUp);
                }
            }
        }

        private async void ChangeMediaPlayerSong()
        {
            var file = (MusicFile)itemGridView.Items.ElementAt(MainPageViewModel.CurrentPosition);
            MainPageViewModel.CurrentFile = file;
            var MyStream = await file.SongFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            RootMediaElement.SetSource(MyStream, file.SongFile.ContentType);
            PauseIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
            PlayIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void ChangeMediaPlayerSongFromSearch()
        {
            var MyStream = await MainPageViewModel.CurrentFile.SongFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            RootMediaElement.SetSource(MyStream, MainPageViewModel.CurrentFile.SongFile.ContentType);
        }
        #endregion

        #region Button Click Events
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPageViewModel.CurrentFile = (MusicFile)e.ClickedItem;
            ChangeMediaPlayerSongFromSearch();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            MainPageViewModel.CurrentPosition--;
            if (MainPageViewModel.CurrentPosition >= 0)
            {
                ChangeMediaPlayerSong();
            }
            else
                MainPageViewModel.CurrentPosition++;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (MainPageViewModel.PlayList.Count() != 1 && MainPageViewModel.PlayList.Count() != 0)
            {
                MainPageViewModel.CurrentPosition++;
                if (MainPageViewModel.CurrentPosition > MainPageViewModel.PlayList.Count() - 1)
                {
                    MainPageViewModel.CurrentPosition = 0;
                    ChangeMediaPlayerSong();
                }
                else
                {
                    ChangeMediaPlayerSong();
                }
            }
        }

        private void btnRewind_Click(object sender, RoutedEventArgs e)
        {
            RootMediaElement.Position -= TimeSpan.FromSeconds(3);
        }

        private void btnFastForward_Click(object sender, RoutedEventArgs e)
        {
            RootMediaElement.Position += TimeSpan.FromSeconds(3);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (RootMediaElement.CurrentState == MediaElementState.Playing)
            {
                RootMediaElement.Pause();
                MainPageViewModel.IsPaused = true;
                PlayIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PauseIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
            else
            {
                RootMediaElement.Play();
                MainPageViewModel.IsPaused = false;
                PauseIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PlayIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void btnVolumeUp_Click(object sender, RoutedEventArgs e)
        {
            RootMediaElement.Volume += 0.5;
        }

        private void btnVolumeDown_Click(object sender, RoutedEventArgs e)
        {
            RootMediaElement.Volume -= 0.5;
        }

        private void btnPlayList_Click(object sender, RoutedEventArgs e)
        {
            AddSongs();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var MusicFiles = itemGridView.SelectedItems; 
            foreach (var File in MusicFiles.ToList())
            {
                MainPageViewModel.PlayList.Remove((MusicFile)File);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnSearchBox.IsEnabled = true;
            btnSearchBox.Focus(FocusState.Pointer);
        }

        private void btnLyrics_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewLyricPage));
        }

        private void btnArtist_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ArtistBio));
        }
        #endregion
    }
}


