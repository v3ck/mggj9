using Logic.Models;

namespace Logic.Simulation
{
    internal class BattleSimulator(GameModel gameModel)
    {
        private readonly GameModel _gameModel = gameModel;
        private readonly BattleState _state = new();

        public IEnumerable<BattleAction> TakeTurn()
        {
            var unitId = _state.Initiative.Dequeue();
            if (!_state.Units.TryGetValue(unitId, out Unit? unit))
            {
                return Enumerable.Empty<BattleAction>();
            }

            _state.Initiative.Enqueue(unitId);
            return unit.Act() ?? Enumerable.Empty<BattleAction>();
        }
    }
}
