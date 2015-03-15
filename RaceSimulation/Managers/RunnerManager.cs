using System;
using System.Globalization;
using RaceSimulation.Model;

namespace RaceSimulation.Managers
{
  class RunnerManager
  {
    private int _nextRunner = 0;
    private Random _randomSpeed = new Random();

    // - chhatbar - below code is commented only for quick testing 
    //private int _speed = 50;

    public Runner GetNextRunner()
    {
      // - chhatbar - below code is commented only for quick testing 
      // var runner = new Runner("H_" + _nextRunner.ToString(CultureInfo.InvariantCulture), _speed--);

      var runner = new Runner("H_" + _nextRunner.ToString(CultureInfo.InvariantCulture), _randomSpeed.Next(10, 200));
      _nextRunner++;
      return runner;
    }
  }
}
