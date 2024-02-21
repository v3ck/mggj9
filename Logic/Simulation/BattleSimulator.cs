using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;
using System.Diagnostics;

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
        }

        public IEnumerable<IBattleAction> Start()
        {
            Debug.WriteLine($"Round [{_state.Round}]");
            return SpawnUnits();
        }

        public IEnumerable<IBattleAction> TakeTurn()
        {
            var (unitId, isEndOfRound) = _turnManager.Next();
            var actions = ProcessUnitAction(unitId)
                .Concat(ProcessEndOfRound(isEndOfRound));
            foreach (var action in actions)
            {
                UpdateTurns(action);
            }
            return actions;
        }

        private void UpdateTurns(IBattleAction? action)
        {
            if (action is not ExistenceAction existenceAction)
            {
                return;
            }

            if (existenceAction.Exists)
            {
                _turnManager.Add(existenceAction.UnitId);
            }
            else
            {
                _turnManager.Remove(existenceAction.UnitId);
            }
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
            Debug.WriteLine($"Round [{_state.Round}]");
            return SpawnUnits();
        }

        private IEnumerable<IBattleAction> SpawnUnits()
        {
            // I am certain there is a better way to write this but I had issues
            List<IBattleAction> actions = [];
            foreach (var spawn in _gameModel.Spawns)
            {
                var action = TrySpawnUnit(spawn);
                if (action is not null)
                {
                    actions.Add(action);
                }
            }

            return actions;
        }

        private IBattleAction? TrySpawnUnit(SpawnModel spawn)
        {
            if (_state.Round < spawn.BeginRound)
            {
                return null;
            }

            if (spawn.EndRound <= _state.Round)
            {
                return null;
            }

            if (spawn.Probability < rng.NextDouble())
            {
                return null;
            }

            if (!_gameModel.Units.TryGetValue(spawn.UnitCode, out var unitModel))
            {
                return null;
            }

            var unit = new BattleUnit(unitModel, _state, _gameModel)
            {
                Location = PlaceUnit()
            };
            _state.Units[unit.Id] = unit;
            _turnManager.Add(unit.Id);

            if (unit.Location is null)
            {
                return null;
            }

            return new ExistenceAction()
            {
                UnitId = unit.Id,
                UnitCode = unitModel.Code,
                Exists = true,
                Location = unit.Location
            };
        }

        //private IEnumerable<IBattleAction> SpawnUnits()
        //{
        //    return _gameModel.Spawns
        //        .Select(TrySpawnUnit)
        //        .Aggregate(Enumerable.Empty<IBattleAction>(), (a, b) => a.Concat(b));
        //}

        //private IEnumerable<IBattleAction> TrySpawnUnit(SpawnModel spawn)
        //{
        //    if (_state.Round < spawn.BeginRound)
        //    {
        //        yield break;
        //    }

        //    if (spawn.EndRound <= _state.Round)
        //    {
        //        yield break;
        //    }

        //    if (spawn.Probability < rng.NextDouble())
        //    {
        //        yield break;
        //    }

        //    if (!_gameModel.Units.TryGetValue(spawn.UnitCode, out var unitModel))
        //    {
        //        yield break;
        //    }

        //    var unit = new BattleUnit(unitModel, _state, _gameModel)
        //    {
        //        Location = PlaceUnit()
        //    };
        //    _state.Units[unit.Id] = unit;

        //    if (unit.Location is null)
        //    {
        //        yield break;
        //    }

        //    yield return new ExistenceAction()
        //    {
        //        UnitId = unit.Id,
        //        UnitCode = unitModel.Code,
        //        Exists = true,
        //        Location = unit.Location
        //    };
        //}
    }
}
