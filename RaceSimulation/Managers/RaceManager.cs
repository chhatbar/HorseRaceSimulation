using System;
using System.Collections.Generic;
using System.Linq;
using RaceSimulation.Config;
using RaceSimulation.Model;

namespace RaceSimulation.Managers
{
  /// <summary>
  /// Encapsulates the strategy of creating races and deciding the winnners
  /// </summary>
  class RaceManager
  {
    #region Private Data Members
    private RaceConfig _raceConfig;
    private RunnerManager _runnerManager;
    private IDictionary<int, Race> _raceDictionary = new Dictionary<int, Race>();
    private List<Runner> _allRunners = new List<Runner>();
    private EliminationManager _eliminationManager;
    private List<Runner> _winners;
    #endregion

    #region Constructor/s
    public RaceManager(RaceConfig raceConfig_, RunnerManager runnerManager_)
    {
      _raceConfig = raceConfig_;
      _runnerManager = runnerManager_;
      _eliminationManager = new EliminationManager(_raceDictionary, _allRunners);
    }
    #endregion

    #region Public Properties

    /// <summary>
    /// Get a collection of Winners among the set of Runners
    /// </summary>
    public IEnumerable<Runner> Winners
    {
      get { return _winners; }
    }

    public EliminationManager EliminationMgr
    {
      get { return _eliminationManager; }
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Strategy to execute Race/s among Runners to decide a set of Winners
    /// </summary>
    public void ExecuteRace()
    {
      try
      {
        // Make the runners ready to run their Races
        PopulateRunners();

        // Make sure each Runner runs atleast one race
        RunPreliminaryRaces();

        // Retrieve a topper for the Races ran so far, if required run more races to decide a topper
        Runner topper = GetTopperFromRaces(_raceDictionary.Values);

        // Get the runnerUps
        IEnumerable<Runner> runnerUps = GetRunnerUps(topper);

        // Winners - RunnerUps will follow the Topper
        PopulateWinners(runnerUps, topper);

        // Print the result
        PrintWinners();
      }
      catch (Exception ex)
      {
        throw new Exception("Exception while excuting race", ex);
      }
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Given the set of RunnerUps and Topper, Populates the list of Winners
    /// </summary>
    private void PopulateWinners(IEnumerable<Runner> runnerUps, Runner topper)
    {
      _winners = new List<Runner>(runnerUps);
      _winners.Insert(0, topper);
    }

    /// <summary>
    /// Prints the formatted result of Winners
    /// </summary>
    private void PrintWinners()
    {
      Console.WriteLine("\nTotal Race Conducted: " + _raceDictionary.Values.Count);
      Console.WriteLine("\nRace Winners: ");
      for (int i = 0; i < _raceConfig.TotalRanks; i++)
      {
        Console.WriteLine("Rank " + i + ": " + _winners.ElementAt(i));
      }
    }

    /// <summary>
    /// Gets a list of RunnerUps. Creates and executes more races as and when required to decide the runnerups
    /// </summary>
    /// <param name="topper">Topper should be known in prior</param>
    private IEnumerable<Runner> GetRunnerUps(Runner topper)
    {
      IEnumerable<Runner> runnerUps = new List<Runner>();

      // Only consider non-eliminated runners
      IEnumerable<Runner> nonEliminatedRunners = _eliminationManager.GetNonEliminatedRunners(topper);

      // Create the requried number of races
      IEnumerable<Race> moreRaces = CreateRacesForRunners(nonEliminatedRunners.Where(x => x != topper));

      // Run the races
      RunRaces(moreRaces);

      // Possibly, only one race was required to know the runnerUps
      if (moreRaces.Count() == 1)
      {
        runnerUps = moreRaces.ElementAt(0).RaceResults;
      }
      else // more than one race was required to know the runnerUps
      {
        runnerUps = DeriveRunnerUps(moreRaces); ;
      }
      return runnerUps;
    }

    /// <summary>
    /// Gets a list of runnerUps from multiple race instances
    /// </summary>
    private IEnumerable<Runner> DeriveRunnerUps(IEnumerable<Race> moreRaces_)
    {
      var newRunnerUps = new List<Runner>();
      Runner nextRunnerUp = GetTopperFromRaces(moreRaces_);
      newRunnerUps.Add(GetTopperFromRaces(moreRaces_));
      newRunnerUps.AddRange(GetRunnerUps(nextRunnerUp));
      return newRunnerUps;
    }

    /// <summary>
    /// Gets the topper from the list of races run so far. Runs more races to decide the topper.
    /// </summary>
    private Runner GetTopperFromRaces(IEnumerable<Race> races_)
    {
      Runner topper = null;
      IEnumerable<Race> moreRaces = CreateRacesForRunners(GetToppersFromEachRace(races_));

      RunRaces(moreRaces);

      if (moreRaces.Count() == 1)
      {
        topper = moreRaces.ElementAt(0).RaceResults.ElementAt(0);
      }
      else
      {
        return GetTopperFromRaces(moreRaces);
      }
      return topper;
    }

    /// <summary>
    /// Run preliminary races so that all runners are made to run at least one race
    /// </summary>
    private void RunPreliminaryRaces()
    {
      IEnumerable<Race> races = CreateRacesForRunners(_allRunners);

      RunRaces(races);
    }

    /// <summary>
    /// Runs the set of argument races and caches the results
    /// </summary>
    private void RunRaces(IEnumerable<Race> races)
    {
      foreach (var race in races)
      {
        race.Start();

        Console.WriteLine(race.ToString());
        _raceDictionary.Add(race.Id, race);
      }
    }

    /// <summary>
    /// Selects the toppers from the arguments set of races
    /// </summary>
    private IEnumerable<Runner> GetToppersFromEachRace(IEnumerable<Race> races_)
    {
      return races_.Select(race => race.RaceResults.ElementAt(0));
    }

    /// <summary>
    /// Considering the Race Configurations (max runners per race, Distance), creates Race instances
    /// </summary>
    /// <param name="runners_"></param>
    /// <returns></returns>
    private IEnumerable<Race> CreateRacesForRunners(IEnumerable<Runner> runners_)
    {
      List<Runner> runners = runners_.ToList();

      var races = new Race[(runners.Count() / _raceConfig.MaxRunnersInSingleRace)];

      for (int i = 0; i < races.Length; i++)
      {
        races[i] = new Race(runners.GetRange(i * _raceConfig.MaxRunnersInSingleRace, _raceConfig.MaxRunnersInSingleRace), _raceConfig.Distance);
      }
      return races;
    }

    /// <summary>
    /// Create and Cache the runners who are going to participate in the Races
    /// </summary>
    private void PopulateRunners()
    {
      for (var i = 0; i < _raceConfig.TotalRunners; i++)
      {
        _allRunners.Add(_runnerManager.GetNextRunner());
      }

      Console.WriteLine("Participating Runners: ");
      foreach (var runner in _allRunners)
      {
        Console.WriteLine(runner.ToString());
      }
      Console.WriteLine(string.Empty);

    }
    #endregion
  }
}
