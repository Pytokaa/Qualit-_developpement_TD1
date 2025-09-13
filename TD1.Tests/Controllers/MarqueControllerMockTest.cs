using TD1.Controllers;
using TD1.Models;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TD1.Repository;

namespace TD1.Tests.Controllers;


[TestClass]
[TestSubject(typeof(MarqueController))]
[TestCategory("mock")]
public class MarqueControllerMockTest
{
    private readonly MarqueController _marqueController;

    public MarqueControllerMockTest()
    {
        var manager = new Mock<IDataRepository<Marque>>();
        _marqueController = new MarqueController(manager.Object);
    }
}