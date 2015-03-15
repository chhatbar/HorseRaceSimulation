using System.Linq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using RaceSimulation.Helper;
using RaceSimulation.Managers;

namespace RaceSimulationTest.Tests.Managers
{
  [TestFixture]
  class TestRaceManager
  {
    [Test]
    public void ExecuteRaceTest()
    {
      var raceManager = UnityContainerHelper.UnityContainer.Resolve<RaceManager>();
      raceManager.ExecuteRace();

      for (var i = 0; i < raceManager.Winners.Count() - 1; i++)
      {
        Assert.IsTrue(raceManager.Winners.ElementAt(i).Speed >= raceManager.Winners.ElementAt(i + 1).Speed);
      }

    }
  }
}
