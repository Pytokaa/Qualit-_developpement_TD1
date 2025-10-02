using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace BlazorE2ETests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class NavigatorTest : PageTest
{
    private const string BaseUrl = "http://localhost:7074/";
    [Test]
    public async Task Clicking_Brands_NavigatesToBrands()
    {
        await Page.GotoAsync(BaseUrl);
        var brandsButton = Page.Locator("#brandsNav").First;
        await brandsButton.ClickAsync();
        
        await Page.WaitForURLAsync(new Regex(@"/brands"));
        Assert.That(Page.Url, Does.Match(@".*/brands"));
    }

    [Test]
    public async Task Clicking_ProductTypes_NavigatesToProductTypes()
    {
        await Page.GotoAsync(BaseUrl);
        var brandsButton = Page.Locator("#productTypesNav").First;
        await brandsButton.ClickAsync();
        
        await Page.WaitForURLAsync(new Regex(@"/productTypes"));
        Assert.That(Page.Url, Does.Match(@".*/productTypes"));
    }
    [Test]
    public async Task Clicking_Home_NavigatesToHome()
    {
        await Page.GotoAsync(BaseUrl);
        var brandsButton = Page.Locator("#homeNav").First;
        await brandsButton.ClickAsync();
        
        await Page.WaitForURLAsync(new Regex(@"/"));
        Assert.That(Page.Url, Does.Match(@".*/"));
    }
    [Test]
    public async Task Clicking_Products_NavigatesToProducts()
    {
        await Page.GotoAsync(BaseUrl);
        var brandsButton = Page.Locator("#productsNav").First;
        await brandsButton.ClickAsync();
        
        await Page.WaitForURLAsync(new Regex(@"/products"));
        Assert.That(Page.Url, Does.Match(@".*/products"));
    }
}