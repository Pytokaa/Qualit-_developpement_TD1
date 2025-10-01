using System;
using System.Net.Http;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace TD1.Tests.BlazorTests;

[TestFixture]
public class ProductsPageTest
{
    private IWebDriver _driver;
    // Mets http:// ou https:// selon ton app (vérifie launchSettings.json ou la console dotnet run)
    private string BaseUrl = "https://localhost:7074/";

    [SetUp]
    public void Setup()
    {
        try
        {
            var browser = Environment.GetEnvironmentVariable("BROWSER") ?? "Chrome";

            if (browser.Equals("firefox", StringComparison.OrdinalIgnoreCase))
            {
                var ffOpts = new OpenQA.Selenium.Firefox.FirefoxOptions
                {
                    AcceptInsecureCertificates = true
                };
                _driver = new FirefoxDriver(ffOpts);
            }
            else if (browser.Equals("edge", StringComparison.OrdinalIgnoreCase))
            {
                var edgeOptions = new EdgeOptions
                {
                    AcceptInsecureCertificates = true
                };
                _driver = new EdgeDriver(edgeOptions);
            }
            else // Chrome par défaut
            {
                var options = new ChromeOptions();
                // <--- important fix pour certaines versions Chrome + Selenium
                options.AddArgument("--remote-allow-origins=*");
                // accepter les certifs auto-signés si besoin
                options.AcceptInsecureCertificates = true;
                options.AddArgument("--ignore-certificate-errors");
                _driver = new ChromeDriver(options);
            }

            // on garde un timeout réduit car on utilise explicit wait ensuite
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Setup: {ex}");
            throw;
        }
    }

    [TearDown]
    public void TearDown()
    {
        try { _driver?.Quit(); } catch { }
    }
    [Test]
    public void Products_Page_Should_Load_And_Display_Title()
    {
        _driver.Navigate().GoToUrl("https://localhost:7074/products");

        Console.WriteLine("[DEBUG] Current URL: " + _driver.Url);
        Console.WriteLine("[DEBUG] Page title: " + _driver.Title);

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        var h3 = wait.Until(d => d.FindElement(By.TagName("h3")));

        Assert.That(h3.Text, Is.EqualTo("Products"));
    }
    [Test]
    public void Products_Filter_By_Brand_Should_Work()
    {
        // 1️⃣ Naviguer vers la page Products
        _driver.Navigate().GoToUrl($"{BaseUrl}products");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        // 2️⃣ Attendre que le select et les produits soient chargés
        var brandSelect = wait.Until(d => d.FindElement(By.Id("selectbrand")));
        var productDivs = wait.Until(d => d.FindElements(By.CssSelector("#divProducts .divProduct")));

        // 3️⃣ Sélectionner une marque
        var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(brandSelect);
        string brandToSelect = "Starlabs"; // Remplace par une marque existante dans ta DB
        selectElement.SelectByText(brandToSelect);

        // 4️⃣ Attendre que la liste de produits se mette à jour (petit délai pour le debounce)
        Thread.Sleep(600); // debounce de 500ms dans ton OnSearchInput/OnBrandChange
        productDivs = _driver.FindElements(By.CssSelector("#divProducts .divProduct"));

        // 5️⃣ Vérifier que tous les produits affichés ont bien la marque sélectionnée
        foreach (var productDiv in productDivs)
        {
            var brandText = productDiv.FindElement(By.CssSelector("h5:nth-of-type(1)")).Text; 
            // h5:nth-of-type(1) = "Brand : BrandName"
            Assert.IsTrue(brandText.Contains(brandToSelect), $"Produit trouvé avec marque incorrecte : {brandText}");
        }

        // 6️⃣ Optionnel : vérifier qu'il y a bien au moins un produit affiché
        Assert.IsTrue(productDivs.Count > 0, "Aucun produit trouvé pour la marque sélectionnée.");
    }

    

}
