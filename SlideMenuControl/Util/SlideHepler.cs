using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SlideMenuControl.Util
{  
    /// <summary>
    /// This class provide hepler function
    /// </summary>
    public class SlideHepler
    {   
        /// <summary>
        /// Constructer 
        /// </summary>
        public SlideHepler()
        { 
        }

        /// <summary>
        /// Return Image source
        /// </summary>
        /// <param name="source">Uri</param>
        /// <returns>Image Source</returns>
        public ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
