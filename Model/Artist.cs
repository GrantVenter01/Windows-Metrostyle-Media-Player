using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaApp.Model
{
    class Artist
    {
        string ImagePath = "ms-appx:///Assets/ArtistImages/";
        public BitmapImage ArtistPicture { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Synopsis { get; set; }

        public Artist(string ArtistPictureName, string Name,string Genre, string Synopsis)
        {
            this.ArtistPicture = new BitmapImage(new Uri(ImagePath + ArtistPictureName));
            this.Name = Name;
            this.Genre = Genre;
            this.Synopsis = Synopsis;
        }
    }
}
