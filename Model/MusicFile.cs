using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaApp.Model
{
    class MusicFile
    {
        public StorageFile SongFile { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AlbumName { get; set; }
        public string Year { get; set; }
        public BitmapImage AlbumArt { get; set; }
        public MusicFile(StorageFile SongFile,string FileName,string Title, string Artist, string AlbumName,BitmapImage AlbumArt, string Year)
        {
            this.SongFile = SongFile;
            this.FileName = FileName;
            this.Title = Title;
            this.Artist = Artist;
            this.AlbumName = AlbumName;
            this.AlbumArt = AlbumArt;
            this.Year = Year;
                
        }
    }
}
