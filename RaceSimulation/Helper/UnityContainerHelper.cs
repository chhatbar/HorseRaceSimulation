using Microsoft.Practices.Unity;
using RaceSimulation.Managers;

namespace RaceSimulation.Helper
{
  /// <summary>
  /// Helper class for Prism UnityContainer to facilitate IoC / Dependency Injection
  /// </summary>
  static class UnityContainerHelper
  {
    private static IUnityContainer _unityContainer;

    public static IUnityContainer UnityContainer
    {
      get { return _unityContainer; }
    }

    static UnityContainerHelper()
    {
      _unityContainer = new UnityContainer();

      _unityContainer.RegisterType<RunnerManager>();
      _unityContainer.RegisterType<RaceManager>();

    }
  }
}
