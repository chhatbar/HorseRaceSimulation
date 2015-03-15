using System.Collections.Generic;
using System.Linq;
using RaceSimulation.Config;
using Microsoft.Practices.Unity;
using RaceSimulation.Helper;
using RaceSimulation.Model;

namespace RaceSimulation.Managers
{
  /// <summary>
  /// Encapsulates the logic to eliminate or retain a Runner
  /// </summary>
  class EliminationManager
  {
    #region Private Data Members
    private IDictionary<int, Race> _raceDictionary = new Dictionary<int, Race>();
    private IEnumerable<Runner> _allRunners;
    private RaceConfig _raceConfig = UnityContainerHelper.UnityContainer.Resolve<RaceConfig>();
    #endregion

    #region Public Constructor
    public EliminationManager(IDictionary<int, Race> raceDictionary_, IEnumerable<Runner> allRunners_)
    {
      _raceDictionary = raceDictionary_;
      _allRunners = allRunners_;
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Gets a collection of non-eliminated runners. 
    /// If you pass a topper, potentially more runners could be eliminated considering their relative ranking from the Topper 
    /// If you do not pass a topper, non-eleminated runners will be returned without considering their relative ranking from the Topper
    /// </summary>
    /// <param name="topper_"></param>
    /// <returns></returns>
    public IEnumerable<Runner> GetNonEliminatedRunners(Runner topper_)
    {
      IEnumerable<Runner> nonEliminatedRunners = null;

      if (topper_ == null)
      {
        IEnumerable<Runner> eliminatedRunners = GetEliminatedRunners();

        // From the collection of AllRunners, discard the the ones who are definitely not among the top rankers
        nonEliminatedRunners = _allRunners.Where(runner => eliminatedRunners.Contains(runner) == false);
      }
      else
      {
        // From the collection of non-eliminated runners, taking the topper into consideration further eliminate the runners considering their relative ranking
        nonEliminatedRunners = GetNonEliminatedRunners(null).Where(runner => (GetRelativeRankFromTopper(runner, topper_, new List<Race>()) < _raceConfig.TotalRanks));
      }
      return nonEliminatedRunners;
    }


    #endregion

    #region Private Methods

    /// <summary>
    /// Get the relative rank of a runner from the Topper considering all races so far
    /// </summary>
    /// <param name="nonTopper_">The runner whos Relative rank is desired</param>
    /// <param name="topper_">Topper should be already known in prior</param>
    /// <param name="excludeRaces_">A collection of Races that needs to be excluded (To make sure you do not get into infinite recursion)</param>
    /// <returns>Relative Rank</returns>
    private int GetRelativeRankFromTopper(Runner nonTopper_, Runner topper_, List<Race> excludeRaces_)
    {
      int relativeRank = -1;

      // if the argument Runner is a Topper, his relative ranking from the Topper is 0
      if (nonTopper_ == topper_)
      {
        relativeRank = 0;
      }
      else
      {
        // Consider all the Races which have run so far
        foreach (var race in _raceDictionary.Values)
        {
          // Do not consider the race that needs an explicit exclusion (to make sure you do not go into infinite recursion)
          if (excludeRaces_.Contains(race) == false)
          {
            // Check if this race has got Non-Topper's participation
            if (race.RaceResults.Contains(nonTopper_))
            {
              // Check if this race has also got the Topper's participation
              if (race.RaceResults.Contains(topper_))
              {
                // The relative rank between the topper and non-topper is easy to be calc'ed if both have participated in the same race
                relativeRank = GetRank(race, nonTopper_) - GetRank(race, topper_);
              }
              // Now the more complex case - if this race has NOT got the Topper's participation, Relative ranking will have to be derived from other races
              else
              {
                // For all the runners in this Race
                foreach (var runner in race.RaceResults)
                {
                  int rankDifference = GetRank(race, nonTopper_) - GetRank(race, runner);

                  // Consider only those runners who have performed better than the argument Non-Topper for this race
                  if (rankDifference > 0)
                  {
                    // Exclude ths race to get away from infinite recursion
                    excludeRaces_.Add(race);

                    // Make the recursive call
                    int betterRunnerRelativeRank = GetRelativeRankFromTopper(runner, topper_, excludeRaces_);

                    // Check if the above call resulted into a valid Relative Rank
                    if (betterRunnerRelativeRank != -1)
                    {
                      // If your Depth First Search reveals multiple relative Ranks to the Topper, make sure you pick up the one which is fartherest from the Topper
                      int newRelativeRank = betterRunnerRelativeRank + rankDifference;
                      if (newRelativeRank > relativeRank)
                      {
                        relativeRank = newRelativeRank;
                      }
                    }
                  }

                }
              }
            }
          }
        }
      }
      return relativeRank;
    }

    /// <summary>
    /// Retrieves a collections of Eliminated Runners considering all the races run so far
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Runner> GetEliminatedRunners()
    {
      var eliminatedRunners = new List<Runner>();

      foreach (Race race in _raceDictionary.Values)
      {
        eliminatedRunners.AddRange(race.RaceResults.Where(runner => CanEliminateRunnerFromRace(race, runner)));
      }

      bool runAgain = true;

      while (runAgain)
      {
        runAgain = false;
        foreach (var runner in _allRunners)
        {
          // If a runner is following another runner who is already eliminated, the former runner is also considered eliminated
          if (DoesRunnerFollowAnEliminatedRunner(runner, eliminatedRunners) && eliminatedRunners.Contains(runner) == false)
          {
            runAgain = true;
            eliminatedRunners.Add(runner);
          }
        }
      }

      return eliminatedRunners;
    }

    /// <summary>
    /// Check if the argument Runner is slower than an already eliminated runner considering all the races executed so far
    /// </summary>
    private bool DoesRunnerFollowAnEliminatedRunner(Runner argumentRunner_, IEnumerable<Runner> eliminatedRunners_)
    {
      IEnumerable<Race> races = GetAllRacesFor(argumentRunner_);

      return races.Any(race => race.RaceResults.Any(runner => GetRank(race, runner) < GetRank(race, argumentRunner_) && eliminatedRunners_.Contains(runner)));
    }

    /// <summary>
    /// Retrieves a collection of races where the argument runner has participated
    /// </summary>
    private IEnumerable<Race> GetAllRacesFor(Runner runner_)
    {
      return _raceDictionary.Values.Where(race => race.RaceResults.Contains(runner_)).ToList();
    }

    /// <summary>
    /// Depending on the Desired Ranks (typically first 3 Ranks) to be retrieved, check if the given runner can be eliminated considering the Race Results
    /// </summary>
    private bool CanEliminateRunnerFromRace(Race race_, Runner runner_)
    {
      bool canEliminate = false;

      int rank = GetRank(race_, runner_);
      if (rank >= _raceConfig.TotalRanks)
      {
        canEliminate = true;
      }

      return canEliminate;
    }

    /// <summary>
    /// Get the Rank of the runner for the argument race
    /// </summary>
    private int GetRank(Race race_, Runner runner_)
    {
      int rank = -1;
      for (int i = 0; i < race_.RaceResults.Count(); i++)
      {
        if (race_.RaceResults.ElementAt(i).Id == runner_.Id)
        {
          rank = i;
          break;
        }
      }
      return rank;
    }

    #endregion
  }
}
