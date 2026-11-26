using System;
using System.Text;

namespace Avalonia.Labs.Controls.Cache
{
    /// <summary>Helpers for the files stored by the <see cref="AsyncImage"/> cache.</summary>
    public static class CacheHelper
    {
        /// <summary>Gets the cache file name, without folder, for the item downloaded from <paramref name="uri"/>.</summary>
        /// <remarks>Files are stored in the <c>ImageCache</c> folder under <see cref="CacheOptions.BaseCachePath"/>, or the temp folder.</remarks>
        public static string GetCacheFileName(Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);
            return CreateHash64(uri.ToString()).ToString();
        }

        private static ulong CreateHash64(string str)
        {
            var utf8 = Encoding.UTF8.GetBytes(str);

            var value = (ulong)utf8.Length;
            for (var n = 0; n < utf8.Length; n++)
            {
                value += (ulong)utf8[n] << ((n * 5) % 56);
            }

            return value;
        }
    }
}
