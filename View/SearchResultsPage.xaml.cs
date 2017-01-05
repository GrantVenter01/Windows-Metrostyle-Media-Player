using MediaApp.Common;
using MediaApp.Model;
using MediaApp.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
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
    public sealed partial class SearchResultsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private ObservableCollection<MusicFile> SearchPlaylist(string searchString)
        {
            var MatchingItems = from x
                              in MainPageViewModel.PlayList
                                where x.FileName.ToLower().Contains(searchString.ToLower())
                                select x;
            var MatchingItemsList = new ObservableCollection<MusicFile>(MatchingItems);
            return MatchingItemsList;
        }

        public SearchResultsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var queryText = e.NavigationParameter as String;
            var Matches = SearchPlaylist(queryText);
            this.DefaultViewModel["Results"] = Matches;
            if (Matches.Count() > 0)
            {
                VisualStateManager.GoToState(this, "Results Found", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoResultsFound", true);
            }
            // Communicate results through the view model
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';

        }
        
        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainPageViewModel.CurrentFile = (MusicFile)e.ClickedItem;
            Frame.Navigate(typeof(MainPage),"Search");
        }

        private void btnViewLyrics_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewLyricPage));
        }

        private void btnPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
