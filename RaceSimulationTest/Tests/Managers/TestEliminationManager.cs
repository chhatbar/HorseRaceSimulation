using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using RaceSimulation.Config;
using RaceSimulation.Helper;
using RaceSimulation.Managers;
using RaceSimulation.Model;

namespace RaceSimulationTest.Tests.Managers
{
  [TestFixture]
  class TestEliminationManager
  {
    [Test]
    public void EliminationMgrTest()
    {
      var raceConfig = UnityContainerHelper.UnityContainer.Resolve<RaceConfig>();
      var raceManager = UnityContainerHelper.UnityContainer.Resolve<RaceManager>();
      raceManager.ExecuteRace();

      // Post 7 races, excpet the top ranks, all the other Runners should be eliminated
      IEnumerable<Runner> relativeNonEliminatedRunners = raceManager.EliminationMgr.GetNonEliminatedRunners(raceManager.Winners.ElementAt(0));
      Assert.IsTrue(relativeNonEliminatedRunners.Count() == raceConfig.TotalRanks);
    }
  }
}
