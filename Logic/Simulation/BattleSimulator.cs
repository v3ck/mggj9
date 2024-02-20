using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation
{
    internal class BattleSimulator
    {
        private readonly GameModel _gameModel;

        private readonly BattleState _state = new();

        private readonly TurnManager _turnManager = new();

        private readonly Random rng = new();

        public BattleSimulator(GameModel gameModel)
        {
            _gameModel = gameModel;
            SpawnUnits();
            _turnManager.Init(_state.Units.Keys.Shuffle());
        }

        public IEnumerable<IBattleAction> TakeTurn()
        {
            var (unitId, isEndOfRound) = _turnManager.Next();
            return ProcessUnitAction(unitId)
                .Concat(ProcessEndOfRound(isEndOfRound));
        }

        private Hex? PlaceUnit()
        {
            return _gameModel.Grid.Hexes
                .Where(hex => !_state.Units.Values.Any(unit => hex == unit.Location))
                .Random();
        }

        private IEnumerable<IBattleAction> ProcessUnitAction(int unitId)
        {
            if (!_state.Units.TryGetValue(unitId, out BattleUnit? unit))
            {
                _turnManager.Remove(unitId);
                return Enumerable.Empty<IBattleAction>();
            }

            return unit.Act() ?? Enumerable.Empty<IBattleAction>();
        }

        private IEnumerable<IBattleAction> ProcessEndOfRound(bool isEndOfRound)
        {
            if (!isEndOfRound)
            {
                return Enumerable.Empty<IBattleAction>() ;
            }

            _state.Round++;
            return SpawnUnits();
        }

        private IEnumerable<IBattleAction> SpawnUnits()
        {
            return _gameModel.Spawns
                .Select(TrySpawnUnit)
                .Aggregate(Enumerable.Empty<IBattleAction>(), (a, b) => a.Concat(b));
        }

        private IEnumerable<IBattleAction> TrySpawnUnit(SpawnModel spawn)
        {
            if (_state.Round < spawn.BeginRound)
            {
                yield break;
            }

            if (spawn.EndRound <= _state.Round)
            {
                yield break;
            }

            if (spawn.Probability < rng.NextDouble())
            {
                yield break;
            }

            if (!_gameModel.Units.TryGetValue(spawn.UnitCode, out var unitModel))
            {
                yield break;
            }

            var unit = new BattleUnit(unitModel, _state, _gameModel)
            {
                Location = PlaceUnit()
            };
            _state.Units[unit.Id] = unit;

            if (unit.Location is null)
            {
                yield break;
            }

            yield return new ExistenceAction()
            {
                UnitId = unit.Id,
                UnitCode = unitModel.Code,
                Exists = true,
                Location = unit.Location
            };
        }
    }
}
