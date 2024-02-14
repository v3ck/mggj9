using Logic.Extensions;
using Logic.Models;

namespace Logic.Simulation
{
    internal class TargetRules(HexGrid hexGrid, BattleState battleState)
    {
        private readonly HexGrid _hexGrid = hexGrid;

        private readonly BattleState _battleState = battleState;

        public Hex? SingleTarget(Hex sourceHex, int minDistance, int maxDistance)
        {
            return _hexGrid.WithinDistance(sourceHex, minDistance, maxDistance).
                Where(hex => _battleState.Units.Values.Any(unit => unit.Location == hex)).
                Random();
        }
    }
}
