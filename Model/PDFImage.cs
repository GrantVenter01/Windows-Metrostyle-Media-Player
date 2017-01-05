using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaApp.ViewModel
{
    class PDFImage
    {
        public BitmapImage PdfImage { get; set; }
        public PDFImage(BitmapImage image)
        {
            PdfImage = image;
        }
    }
}
