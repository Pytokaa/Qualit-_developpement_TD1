using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Playwright;

namespace BlazorE2ETests;

public class ProductTypePageTest :  PageTest
{
    private const string BaseUrl =  "http://localhost:7074/productTypes";

    [Test]
    public async Task ProductTypePage_Loading()
    {
        await Page.GotoAsync(BaseUrl);
        
        //attendre que la page charge
        var h3Locator = Page.Locator("h3");
        await h3Locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        var h3Text = await h3Locator.InnerTextAsync();
        Assert.That(h3Text.Trim(), Is.EqualTo("Product Types"));
    }
    [Test]
    public async Task BrandInUpdateInputWhenSelected()
    {
        await Page.GotoAsync(BaseUrl);

        await Page.SelectOptionAsync("#ItemSelector", "Tablette");
        await Task.Delay(100);

        var brandInput = Page.Locator("[data-testid='NomTypeProduit']");
        var brandValue = await brandInput.InputValueAsync();

        Assert.That(brandValue, Is.EqualTo("Tablette"));
    }
}