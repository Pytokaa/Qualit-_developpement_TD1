using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace BlazorE2ETests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProductsPageTests : PageTest
{
    private const string BaseUrl = "http://localhost:7074/products";
    
    

    [Test]
    public async Task ProductPage_Loading()
    {
        await Page.GotoAsync(BaseUrl);
        
        //Attendre qu'elle se charge
        var h3Locator = Page.Locator("h3");
        await h3Locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        
        var h3Text = await h3Locator.InnerTextAsync();
        Assert.That(h3Text.Trim(),Is.EqualTo("Products"));
    }

    [Test]
    public async Task Products_Filter_By_Brand_Apple_Should_Work()
    {
        await Page.GotoAsync(BaseUrl);
        
        await Page.SelectOptionAsync("#selectbrand", "Apple");
        
        await Task.Delay(100); //unique manière de ne pas trier la liste complète des produits et d'attendre que le trie se fasse.

        var selected = await Page.Locator("#selectbrand").InputValueAsync();
        Assert.That(selected, Is.EqualTo("Apple"));

        var productDivs = await Page.Locator("#divProducts .divProduct").AllAsync();
        Assert.That(productDivs.Count, Is.GreaterThan(0));

        foreach (var div in productDivs)
        {
            var brandText = await div.Locator("h5:nth-of-type(1)").InnerTextAsync();
            Assert.That(brandText.Contains("Apple"));
        }
    }
    
    

    [Test]
    public async Task Products_Filter_By_ProductType_Tablette_Should_Work()
    {
        await Page.GotoAsync(BaseUrl);
    
        await Page.SelectOptionAsync("#selectproductType", "Tablette");
    
        await Task.Delay(100); // attendre que le filtre soit appliqué

        var selected = await Page.Locator("#selectproductType").InputValueAsync();
        Assert.That(selected, Is.EqualTo("Tablette"));

        var productDivs = await Page.Locator("#divProducts .divProduct").AllAsync();
        Assert.That(productDivs.Count, Is.GreaterThan(0));

        foreach (var div in productDivs)
        {
            var typeText = await div.Locator("h5:nth-of-type(2)").InnerTextAsync();
            Assert.That(typeText.Contains("Tablette"));
        }
    }

    [Test]
    public async Task Products_Filter_By_SearchTerm_St_Should_Work()
    {
        await Page.GotoAsync(BaseUrl);

        var searchInput = Page.Locator("#searchinput");
        await searchInput.FillAsync("st");

        await Task.Delay(600); 

        var productDivs = await Page.Locator("#divProducts .divProduct").AllAsync();
        Assert.That(productDivs.Count, Is.GreaterThan(0));

        foreach (var div in productDivs)
        {
            var nameText = await div.Locator("h4").InnerTextAsync();
            Assert.That(nameText.Contains("st", StringComparison.OrdinalIgnoreCase));
        }
    }
    [Test]
    public async Task Products_Filter_By_SearchTerm_NoMatch_Should_ReturnNothing()
    {
        await Page.GotoAsync(BaseUrl);

        var searchInput = Page.Locator("#searchinput");
        await searchInput.FillAsync("zzzzzz");

        await Task.Delay(600); 

        var productDivs = await Page.Locator("#divProducts .divProduct").AllAsync();

        Assert.That(productDivs.Count, Is.EqualTo(0));
    }
    [Test]
    public async Task AddProductButton_NavigatesToCreatePage()
    {
        await Page.GotoAsync(BaseUrl);
        
        await Page.Locator("#addProduct").ClickAsync();

        await Page.WaitForURLAsync("**/product/create");

        Assert.That(Page.Url, Does.EndWith("/product/create"));
    }
    [Test]
    public async Task ClickingProduct_NavigatesToProductDetail()
    {
        await Page.GotoAsync(BaseUrl);

        var firstProduct = Page.Locator("#divProducts .divProduct").First;
        await firstProduct.ClickAsync();

        await Page.WaitForURLAsync(new Regex(@"/product/\d+$"));

        Assert.That(Page.Url, Does.Match(@".*/product/\d+$"));
    }






}
