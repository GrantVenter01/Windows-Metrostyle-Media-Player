using MediaApp.Model;
using MediaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MediaApp.View
{
    public sealed partial class ArtistBio : Page
    {
        public ArtistBio()
        {
            this.InitializeComponent();
            itemGridView.ItemsSource = ArtistBioPageViewModel.ArtistInfoList;
            if (ArtistBioPageViewModel.StartUp)
            {
                ArtistBioPageViewModel.GetData();
                ArtistBioPageViewModel.StartUp = false;
            }
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ArtistBioItem), e.ClickedItem as Artist);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
