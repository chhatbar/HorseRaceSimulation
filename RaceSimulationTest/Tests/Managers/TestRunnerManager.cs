using System.Globalization;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using RaceSimulation.Helper;
using RaceSimulation.Managers;
using RaceSimulation.Model;

namespace RaceSimulationTest.Tests.Managers
{
  [TestFixture]
  class TestRunner
  {
    [Test]
    public void CreateRandomRunnersTest()
    {
      var runnerManager = UnityContainerHelper.UnityContainer.Resolve<RunnerManager>();

      for (int i = 0; i < 25; i++)
      {
        Runner runner = runnerManager.GetNextRunner();
        Assert.IsTrue(runner.Id.EndsWith(i.ToString(CultureInfo.InvariantCulture)));
        Assert.IsTrue(runner.Speed >= 10 && runner.Speed <= 200);
      }
    }
  }
}
