using PdkBot.BotLib.Collection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PdkBot.BotLib.Common
{
    public class AssetImageHelper
    {
        private static Cache<string, BitmapImage> _wpfCache;
        static AssetImageHelper()
        {
            _wpfCache = new Cache<string, BitmapImage>(0, 0, null);
        }

        public static BitmapImage GetImageFromWpfCache(string assetImage)
        {
            return _wpfCache.GetValue(assetImage, () => Application.Current.FindResource(assetImage.ToString()) as BitmapImage, true, null);
        }

        private static Bitmap GetImageFromAppResource(ImageSource imageSrc)
        {
            Bitmap image;
            if (imageSrc == null)
            {
                image = null;
            }
            else
            {
                Uri uriResource = new Uri(imageSrc.ToString());
                image = new Bitmap(Application.GetResourceStream(uriResource).Stream);
            }
            return image;
        }

    }
}