using Logic.Simulation;

namespace Logic
{
    /// <summary>
    /// The API to be exposed to Godot
    /// </summary>
    public interface IController
    {
        public void StartBattle();

        public void TakeTurn();
    }
}
