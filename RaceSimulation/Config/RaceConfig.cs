namespace RaceSimulation.Config
{
  /// <summary>
  /// Encapsulates the configuration for a race. Potentially, this class could be initialized from XML file or DB
  /// </summary>
  class RaceConfig
  {
    private readonly int _totalRunners = 25;
    private readonly int _maxRunnersInSingleRace = 5;
    private readonly int _distance = 100;
    private readonly int _totalRanks = 3;

    public int TotalRunners
    {
      get { return _totalRunners; }
    }

    public int MaxRunnersInSingleRace
    {
      get { return _maxRunnersInSingleRace; }
    }

    public int Distance
    {
      get { return _distance; }
    }

    public int TotalRanks
    {
      get { return _totalRanks; }
    }
  }
}
