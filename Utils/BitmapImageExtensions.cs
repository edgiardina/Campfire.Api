using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Campfire.Api.Utils
{
    /// <summary>
    /// Helper class to take the results of the Campfire API GetFile method and return a usable bitmap image.
    /// </summary>
    public class BitmapImageUtils
    {
        public static BitmapImage ToImage(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();

            var stream = new InMemoryRandomAccessStream();
            stream.WriteAsync(byteArray.AsBuffer());
            stream.FlushAsync();
            stream.Seek(0);

            bitmapImage.SetSource(stream);
            return bitmapImage;
        }
    }
}
