using System;
using Microsoft.Practices.Unity;
using RaceSimulation.Helper;
using RaceSimulation.Managers;

namespace RaceSimulationTest
{
  /// <summary>
  /// Executes a sample simulation of Horse Race Problem
  /// </summary>
  class RaceSimulationMain
  {
    private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static void Main(string[] args)
    {
      _log.Info("Entering RaceSimulationMain.Main");
      var raceManager = UnityContainerHelper.UnityContainer.Resolve<RaceManager>();
      raceManager.ExecuteRace();

      _log.Info("Exiting RaceSimulationMain.Main");

      Console.ReadLine();

    }
  }
}
