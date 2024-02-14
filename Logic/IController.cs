namespace Logic
{
    /// <summary>
    /// The API to be exposed to Godot
    /// </summary>
    public interface IController
    {
        public event EventHandler<int> ThingHappened;

        public void TakeTurn();
    }
}
