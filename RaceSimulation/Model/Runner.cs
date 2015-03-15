namespace RaceSimulation.Model
{
  /// <summary>
  /// Represents a Race Participant
  /// </summary>
  class Runner
  {
    private string _id;
    private int _speed;

    internal Runner(string id_, int speed_)
    {
      _id = id_;
      _speed = speed_;
    }

    public string Id
    {
      get { return _id; }
    }

    public int Speed
    {
      get { return _speed; }
    }

    public override string ToString()
    {
      return " [Runner: " + Id + "(" + Speed + " m/s)] ";
    }
  }
}
