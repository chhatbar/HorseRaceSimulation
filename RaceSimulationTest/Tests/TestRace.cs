using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using RaceSimulation.Helper;
using RaceSimulation.Managers;
using RaceSimulation.Model;

namespace RaceSimulationTest.Tests
{
  [TestFixture]
  class TestRace
  {
    [Test]
    public void CreateRaceTest()
    {
      // Get an instance of RunnerManager
      var runnerManager = UnityContainerHelper.UnityContainer.Resolve<RunnerManager>();
      var runners = new List<Runner>();

      // Create runners
      int maxRunners = 5;
      for (int i = 0; i < maxRunners; i++)
      {
        var runner = runnerManager.GetNextRunner();
        runners.Add(runner);
      }

      // initialize Race and start race
      var race = new Race(runners, 100);
      race.Start();

      List<Runner> raceResults = race.RaceResults.ToList();

      // Make sure that the race results are ordered by the speed of each runner
      for (int i = 0; i < (maxRunners - 1); i++)
      {
        Assert.IsTrue(raceResults[i].Speed >= raceResults[i + 1].Speed);
      }
    }
  }
}
