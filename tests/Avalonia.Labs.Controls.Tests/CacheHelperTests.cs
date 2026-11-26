using Avalonia.Labs.Controls.Cache;
using Xunit;

namespace Avalonia.Labs.Controls.Tests;

public class CacheHelperTests
{
    [Fact]
    public void GetCacheFileName_Is_Stable()
    {
        // The cache file name must not change between versions, otherwise existing caches are invalidated.
        Assert.Equal("5851319189687117373", CacheHelper.GetCacheFileName(new Uri("https://example.com/image.png")));
    }

    [Fact]
    public void GetCacheFileName_Throws_On_Null()
    {
        Assert.Throws<ArgumentNullException>(() => CacheHelper.GetCacheFileName(null!));
    }
}
