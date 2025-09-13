using TD1.Controllers;
using TD1.Models;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TD1.Repository;


namespace TD1.Tests.Controllers;



[TestClass]
[TestSubject(typeof(ProduitController))]
[TestCategory("mock")]
public class ProductControllerMockTest
{
    private readonly ProduitController _produitController;

    public ProductControllerMockTest()
    {
        var manager = new Mock<IDataRepository<Produit>>();
        _produitController = new ProduitController(manager.Object);
    }
}