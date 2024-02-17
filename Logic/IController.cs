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

        public void StartBattle();

        public void TakeTurn();

        public int AddUnit(UnitCode configUnit);

        public AbilityCode[] GetUnitAbilities(int unitId);

        public void UpdateUnitAbilities(int unitId, AbilityCode[] abilities);
    }
}
