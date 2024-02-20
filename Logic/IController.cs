using Logic.Events;
using Logic.Util;

namespace Logic
{
    /// <summary>
    /// The API to be exposed to Godot
    /// </summary>
    public interface IController
    {
        public event EventHandler<UnitMovedEventArgs>? UnitMoved;

        public event EventHandler<StatusChangedEventArgs>? StatusChanged;

        public event EventHandler<HealthChangedEventArgs>? HealthChanged;

        public event EventHandler<AbilityFiredEventArgs>? AbilityFired;

        public event EventHandler<ExistenceChangedEventArgs>? ExistenceChanged;

        public void StartBattle();

        public void TakeTurn();

        public void AddUnit(string code, int health, string faction, string[] abilities);

        public string[] GetUnitAbilities(string unitCode);

        public void UpdateUnitAbilities(string unitCode, string[] abilities);

        public void AddSpawn(string unitCode, int beginRound, int endRound, double probability);
    }
}
