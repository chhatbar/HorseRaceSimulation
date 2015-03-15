using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace RaceSimulation.Model
{
  /// <summary>
  /// Encapsulates a Race. Simulates a Race of Runners using Multithreading / ManualResetEvent
  /// </summary>
  class Race
  {
    private static int _nextRaceId = 0;

    #region Private Data Members
    private int _id;
    private IEnumerable<Runner> _participants;
    private int _distance;
    private readonly List<Runner> _raceResults = new List<Runner>();
    private RaceState _raceState = RaceState.NotStarted;

    #endregion Private Data Members

    #region Constructor/s
    public Race(IEnumerable<Runner> participants_, int distance_)
    {
      _id = _nextRaceId++;
      _participants = participants_;
      _distance = distance_;
    }
    #endregion Constructor/s

    #region Public Properties
    public int Id
    {
      get { return _id; }
    }

    public IEnumerable<Runner> Participants
    {
      get { return _participants; }
    }

    public IEnumerable<Runner> RaceResults
    {
      get { return _raceResults; }
    }

    public RaceState RaceState
    {
      get { return _raceState; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Prints the Race instance in a formatted fashion
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return
        "\nRace ID: " + Id
        + "\nDistance:" + _distance
        + Participants.Aggregate("\nParticipants: ", (current, participant) => current + participant.ToString())
        + RaceResults.Aggregate("\nResult: ", (current, participant) => current + participant.ToString());

    }

    /// <summary>
    /// Starts a Race. Creates a dedicated Thread for each Runner. The funcion does not return until all the Runners have passed the Finish Line. 
    /// </summary>
    public void Start()
    {
      // update the state of the race to Started
      _raceState = RaceState.Started;

      // Create a Thread for each participant
      var raceParticipants = new List<RaceParticipant>();
      foreach (var participant in _participants)
      {
        var raceParticipant = new RaceParticipant() { MSE = new ManualResetEvent(false), Runner = participant };
        raceParticipants.Add((raceParticipant));

        var thread = new Thread(RunParticipant);
        thread.Start(raceParticipant);
      }

      // Block until all Runners have crossed the Finish Line 
      WaitHandle.WaitAll(raceParticipants.Select(x => x.MSE).ToArray());

      // Update the state of the race to Finished
      _raceState = RaceState.Finished;
    }
    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// This method will get called by each Thread corresponding to each Runner of the Race
    /// </summary>
    /// <param name="raceParticipant_"></param>
    private void RunParticipant(object raceParticipant_)
    {
      var raceParticipant = (RaceParticipant)raceParticipant_;

      try
      {
        // The participant will start from the Start Line - Distance = 0 
        decimal distanceCovered = 0;

        // Continue running until you cross the finish line
        while (distanceCovered < _distance)
        {
          // Let the participant run at his own speed
          distanceCovered += ((decimal)raceParticipant.Runner.Speed) / (decimal)1000.0;
          Thread.Sleep(1);
        }

        // Particpant has now crossed the finish line. Add him onto to the results collection
        _raceResults.Add(raceParticipant.Runner);

      }
      catch (Exception ex)
      {
        throw new Exception("Exception in Thread for Particpant " + raceParticipant.Runner.ToString(), ex);
      }
      // Signal and exit the Thread
      raceParticipant.MSE.Set();
    }
    #endregion Private Methods

    #region Inner Class

    /// <summary>
    /// Encapsulates the Runner and ManualResetEvent to faciliate simulation of Race
    /// </summary>
    class RaceParticipant
    {
      public Runner Runner { get; set; }
      public ManualResetEvent MSE { get; set; }
    }
    #endregion Inner Class


  }
}
