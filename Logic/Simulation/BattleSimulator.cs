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
                var unit = new BattleUnit(unitModel)
                {
                    Location = gameModel.Grid.Hexes.
                        Where(hex => !_state.Units.Values.Any(unit => hex == unit.Location)).
                        Random()
                };
                _state.Units[unit.Id] = unit;

            }

            _state.Turns.Init(_state.Units.Keys.Shuffle());
        }

        public IEnumerable<IBattleAction> TakeTurn()
        {
            var unitId = _state.Turns.Next();
            if (!_state.Units.TryGetValue(unitId, out BattleUnit? unit))
            {
                _state.Turns.Remove(unitId);
                return TakeTurn();
            }

            return unit.Act() ?? Enumerable.Empty<IBattleAction>();
        }
    }
}
