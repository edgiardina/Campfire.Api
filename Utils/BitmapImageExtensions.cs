using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Campfire.Api.Utils
{
    /// <summary>
    /// Helper class to take the results of the Campfire API GetFile method and return a usable bitmap image.
    /// </summary>
    public class BitmapImageUtils
    {
        public static async Task<BitmapImage> ToImage(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();

            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(byteArray.AsBuffer());
            await stream.FlushAsync();
            stream.Seek(0);

            await bitmapImage.SetSourceAsync(stream);
            return bitmapImage;
        }
    }
}
