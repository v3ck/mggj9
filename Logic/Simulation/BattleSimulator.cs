using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation
{
    internal class BattleSimulator
    {
        private readonly GameModel _gameModel;
        private readonly BattleState _state = new();

        public BattleSimulator(GameModel gameModel)
        {
            _gameModel = gameModel;
            foreach (var unitModel in gameModel.Units.Values)
            {
                _state.Units[unitModel.Id] = new BattleUnit(unitModel)
                {
                    Location = gameModel.Grid.Hexes.
                        Where(hex => !_state.Units.Values.Any(unit => hex == unit.Location)).
                        Random()
                };

            }

            foreach (var id in gameModel.Units.Keys.Shuffle())
            {
                _state.Initiative.Enqueue(id);
            }
        }

        public IEnumerable<IBattleAction> TakeTurn()
        {
            var unitId = _state.Initiative.Dequeue();
            if (!_state.Units.TryGetValue(unitId, out BattleUnit? unit))
            {
                return Enumerable.Empty<IBattleAction>();
            }

            _state.Initiative.Enqueue(unitId);
            return unit.Act() ?? Enumerable.Empty<IBattleAction>();
        }
    }
}
