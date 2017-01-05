using MediaApp.Model;
using MediaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;



namespace MediaApp.View
{
    public sealed partial class ViewLyricPage : Page
    {
        string FileName;
        string NavigationParameter;
        MusicProperties SongProperties;
        PrintDocument LyricDoc = new PrintDocument();
        string ImagePath = "ms-appdata:///local/";
        PdfDocument LyricPdf;
        
        public ViewLyricPage()
        {
            this.InitializeComponent();
            PlayListView.ItemsSource = MainPageViewModel.PlayList;
            MainPage.RootMediaElement.MediaOpened += RootMediaElement_MediaOpened;
            ViewPDFListView.ItemsSource = ViewLyricPageViewModel.PDFImages;
        }

        void RootMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (ViewLyricPageViewModel.counter == 1)
            {
                ViewLyricPageViewModel.counter++;
                LoadPdf();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LyricDoc.GetPreviewPage += LyricDoc_GetPreviewPage;
            LyricDoc.AddPages += LyricDoc_AddPages;
            PrintManager printManger = PrintManager.GetForCurrentView();
            printManger.PrintTaskRequested += printManger_PrintTaskRequested;
            ViewLyricPageViewModel.PDFImages.Clear();
            LoadPdf();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnSearchBox.Focus(FocusState.Pointer);
        }

        private void btnSearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            Frame.Navigate(typeof(SearchResultsPage),args.QueryText);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            LyricDoc.GetPreviewPage -= LyricDoc_GetPreviewPage;
            LyricDoc.AddPages -= LyricDoc_AddPages;
            PrintManager printManger = PrintManager.GetForCurrentView();
            printManger.PrintTaskRequested -= printManger_PrintTaskRequested;
        }

        #region Media Buttons

        private async void ChangeMediaPlayerSongIndex()
        {
            var MyStream = await MainPageViewModel.CurrentFile.SongFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MainPage.RootMediaElement.SetSource(MyStream, MainPageViewModel.CurrentFile.SongFile.ContentType);
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

        private async void ChangeMediaPlayerSong()
        {
            var file = (MusicFile)PlayListView.Items.ElementAt(MainPageViewModel.CurrentPosition);
            MainPageViewModel.CurrentFile = file;
            var MyStream = await file.SongFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MainPage.RootMediaElement.SetSource(MyStream, file.SongFile.ContentType);
            PauseIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
            PlayIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

        private void btnRewind_Click(object sender, RoutedEventArgs e)
        {
            MainPage.RootMediaElement.Position -= TimeSpan.FromSeconds(3);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.RootMediaElement.CurrentState == MediaElementState.Playing)
            {
                MainPage.RootMediaElement.Pause();
                MainPageViewModel.IsPaused = true;
                PlayIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PauseIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
            else
            {
                MainPage.RootMediaElement.Play();
                MainPageViewModel.IsPaused = false;
                PauseIcon.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PlayIcon.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void btnFastForward_Click(object sender, RoutedEventArgs e)
        {
            MainPage.RootMediaElement.Position += TimeSpan.FromSeconds(3);
        }

        #endregion

        #region Adding, Loading, Deleting

        private async void AddSongs()
        {
            FileOpenPicker OpenMusicFiles = new FileOpenPicker();
            OpenMusicFiles.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            OpenMusicFiles.FileTypeFilter.Add(".mp3");
            OpenMusicFiles.FileTypeFilter.Add(".wav");
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
                PlayListView.ItemsSource = MainPageViewModel.PlayList;
                PlayListView.Focus(FocusState.Pointer);
                if (MainPage.RootMediaElement.CurrentState == MediaElementState.Closed)
                    ChangeMediaPlayerSong();
                if (MainPageViewModel.StartUp && MainPageViewModel.PlayList.Count != 0)
                {
                    MainPageViewModel.StartUp = false;
                }
            }
        }

        public async void AddLyrics()
        {
            FileOpenPicker OpenPdfFile = new FileOpenPicker();
            OpenPdfFile.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            OpenPdfFile.FileTypeFilter.Add(".pdf");
            var File = await OpenPdfFile.PickSingleFileAsync();
            ViewLyricPageViewModel.PDFImages.Clear();
            if (File != null)
            {
                await File.CopyAsync(ApplicationData.Current.LocalFolder, FileName);
                var LyricPdf = await PdfDocument.LoadFromFileAsync(File);
                for (uint i = 0; i < LyricPdf.PageCount; i++)
                {
                    var pdfPage = LyricPdf.GetPage(i);
                    using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {
                        await pdfPage.RenderToStreamAsync(stream);
                        var image = new BitmapImage();
                        await image.SetSourceAsync(stream);
                        ViewLyricPageViewModel.PDFImages.Add(new PDFImage(image));
                    }
                }

            }
        }

        async Task<Boolean> DoesFileExistAsync(Uri DataUri)
        {
            try
            {
                await StorageFile.GetFileFromApplicationUriAsync(DataUri);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void LyricDoc_AddPages(object sender, AddPagesEventArgs e)
        {
            for (int i = 0; i < ViewLyricPageViewModel.PDFImages.Count; i++)
            {
                this.LyricDoc.AddPage(new Image
                {
                    Source = ViewLyricPageViewModel.PDFImages.ElementAt(i).PdfImage
                });
            }

            this.LyricDoc.AddPagesComplete();
        }

        public async void LoadPdf()
        {
            ViewLyricPageViewModel.PDFImages.Clear();
            FileName = MainPageViewModel.CurrentFile.SongFile.Name + ".pdf";
            if (!await DoesFileExistAsync(new Uri(ImagePath + FileName)))
            {
                VisualStateManager.GoToState(this, "NoLyrics", true);
                ViewLyricPageViewModel.counter = 1;
            }
            else
            {
                ViewLyricPageViewModel.counter = 1;
                //CreateImagesOfPDF();
            }
        }

        private void btnAddItems_Click(object sender, RoutedEventArgs e)
        {
            AddSongs();
        }

        private void btnAddLyrics_Click(object sender, RoutedEventArgs e)
        {
            AddLyrics();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var MusicFiles = PlayListView.SelectedItems;
            foreach (var File in MusicFiles.ToList())
            {
                MainPageViewModel.PlayList.Remove((MusicFile)File);
            }
        }

        #endregion

        #region Printing

        void LyricDoc_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            this.LyricDoc.SetPreviewPageCount(ViewLyricPageViewModel.PDFImages.Count, PreviewPageCountType.Final);
            for (int i = 0; i < ViewLyricPageViewModel.PDFImages.Count; i++)
            {
                if (e.PageNumber == i + 1)
                {
                    this.LyricDoc.SetPreviewPage(i + 1, new Image
                    {
                        Source = ViewLyricPageViewModel.PDFImages.ElementAt(i).PdfImage
                    });
                }
            }
        }

        private void printManger_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask task = args.Request.CreatePrintTask(MainPageViewModel.CurrentFile.Title + "Lyrics",
            async (taskArgs) =>
            {
                var deferral = taskArgs.GetDeferral();
                await
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    taskArgs.SetSource(LyricDoc.DocumentSource);
                    deferral.Complete();
                });
            });
        }

        private async void btnPrintPdf_Click(object sender, RoutedEventArgs e)
        {
            await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
        }

        #endregion

        #region Navigation

        private void btnArtistBio_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ArtistBio));
        }

        private void btnViewLyrics_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewLyricPage));
        }

        private void btnPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void PlayListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            MainPageViewModel.CurrentFile = (MusicFile)PlayListView.SelectedItem;
            MainPageViewModel.CurrentPosition = PlayListView.SelectedIndex;
            ChangeMediaPlayerSongIndex();
        }

        #endregion

    }
}
