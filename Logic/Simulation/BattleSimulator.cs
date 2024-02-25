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

        public List<IBattleAction> Start()
        {
            Debug.WriteLine($"Round [{_state.Round}]");
            var actions = SpawnUnits();
            foreach (var action in actions)
            {
                UpdateTurns(action, true);
            }
            return actions;

        }

        public List<IBattleAction> TakeTurn()
        {
            var (unitId, isEndOfRound) = _turnManager.Next();
            var unitActions = ProcessUnitAction(unitId);
            var endRoundActions = ProcessEndOfRound(isEndOfRound);
            var actions = unitActions.Concat(endRoundActions).ToList();
            foreach (var action in actions)
            {
                UpdateTurns(action, false);
                UpdateUnits(action);
            }
            return actions;
        }

        private void UpdateTurns(IBattleAction? action, bool isStarting)
        {
            if (action is not ExistenceAction existenceAction)
            {
                return;
            }

            if (existenceAction.Exists)
            {
                _turnManager.Add(existenceAction.UnitId, isStarting);
            }
            else
            {
                _turnManager.Remove(existenceAction.UnitId);
            }
        }

        private void UpdateUnits(IBattleAction? action)
        {
            if (action is null)
            {
                return;
            }

            foreach (var unit in _state.Units.Values)
            {
                unit.ChargeAbilities(action);
            }
        }

        private Hex? PlaceUnit(UnitModel unit)
        {
            var candidates = _gameModel.Grid.Hexes
                .Where(hex => !_state.Units.Values.Any(unit => hex == unit.Location))
                .Shuffle();

            if ("GOOD" == unit.Faction)
            {
                return candidates.MaxBy(hex => hex.Y);
            }

            return candidates.MinBy(hex => hex.Y);
        }

        private List<IBattleAction> ProcessUnitAction(int unitId)
        {
            if (!_state.Units.TryGetValue(unitId, out BattleUnit? unit))
            {
                _turnManager.Remove(unitId);
                return [];
            }

            return unit.Act();
        }

        private List<IBattleAction> ProcessEndOfRound(bool isEndOfRound)
        {
            if (!isEndOfRound)
            {
                return [];
            }

            _state.Round++;
            Debug.WriteLine($"Round [{_state.Round}]");
            return SpawnUnits();
        }

        private List<IBattleAction> SpawnUnits()
        {
            List<IBattleAction> actions = [];
            if (!_gameModel.Spawns.TryGetValue(_state.Round, out var spawn))
            {
                return actions;
            }

            foreach (var unitCode in spawn.UnitCodes)
            {
                var action = TrySpawnUnit(unitCode);
                if (action is not null)
                {
                    actions.Add(action);
                }
            }

            return actions;
        }

        private IBattleAction? TrySpawnUnit(string unitCode)
        {
            if (!_gameModel.Units.TryGetValue(unitCode, out var unitModel))
            {
                return null;
            }

            var unit = new BattleUnit(unitModel, _state, _gameModel)
            {
                Location = PlaceUnit(unitModel)
            };
            _state.Units[unit.Id] = unit;

            if (unit.Location is null)
            {
                return null;
            }

            Debug.WriteLine(unitCode);

            return new ExistenceAction()
            {
                UnitId = unit.Id,
                UnitCode = unitModel.Code,
                Exists = true,
                Location = unit.Location
            };
        }
    }
}
