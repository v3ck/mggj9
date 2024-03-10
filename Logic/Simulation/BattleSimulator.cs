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
            //Debug.WriteLine($"Round [{_state.Round}]");

            foreach (var spawnModel in _gameModel.Spawns)
            {
                _state.Spawns.Add(new BattleSpawn(spawnModel));
                //Debug.WriteLine($"{spawnModel.UnitCode}, {spawnModel.BeginRound}, {spawnModel.EndRound}, {spawnModel.Rate}, {spawnModel.Volatility}");
            }

            var actions = SpawnUnits();
            foreach (var action in actions)
            {
                UpdateTurns(action, true);
            }
            return actions;
        }

        public List<IBattleAction> TakeTurn()
        {
            var (unitId, isEndOfRound) = GetNextTurn();
            var unitActions = ProcessUnitAction(unitId);
            var endRoundActions = ProcessEndOfRound(isEndOfRound);
            var actions = unitActions.Concat(endRoundActions).ToList();
            foreach (var action in actions)
            {
                UpdateTurns(action, false);
                UpdateUnits(action);
            }

            actions.AddRange(UpdateScores(actions));

            return actions;
        }

        public void UpdateUnitAbilities(string unitCode)
        {
            var units = _state.Units.Values.Where(unit => unit.Model.Code == unitCode);
            foreach (var unit in units)
            {
                unit.RefreshAbilities();
            }
        }

        private (int, bool) GetNextTurn()
        {
            var units = _state.Units.Values.Where(unit => 0 < unit.TimeTurns);
            if (!units.Any())
            {
                return _turnManager.Next();
            }

            var id = units
                .Shuffle()
                .MaxBy(unit => unit.TimeTurns)
                .Id;
            return (id, false);
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

        private List<IBattleAction> UpdateScores(IEnumerable<IBattleAction> actions)
        {
            var scoreActions = new List<IBattleAction?>();
            foreach (var action in actions)
            {
                scoreActions.AddRange(UpdateScore(action));
            }
            return scoreActions
                .Where(scoreAction => scoreAction is not null)
                .Select(scoreAction => scoreAction!)
                .ToList();
        }

        private List<IBattleAction> UpdateScore(IBattleAction? action)
        {
            if (action is not ExistenceAction existenceAction)
            {
                return [];
            }

            if (existenceAction.Exists)
            {
                return [];
            }

            if (!_gameModel.Units.TryGetValue(existenceAction.UnitCode, out var unit))
            {
                return [];
            }

            if ("GOOD" == unit.Faction)
            {
                return [];
            }

            _state.Score += 1;
            List<IBattleAction> scoreActions = [];
            scoreActions.Add(new ScoreAction()
            {
                Amount = _state.Score
            });

            if (0 == _state.Score % _gameModel.RewardInterval)
            {
                scoreActions.Add(GenerateRewards());
            }

            return scoreActions;
        }

        private IBattleAction GenerateRewards()
        {
            List<string> rewards = [];
            foreach (var _ in Enumerable.Range(0, 3))
            {
                rewards.Add(GenerateReward(rewards));
            }
            return new RewardAction
            {
                AbilityCodes = [.. rewards]
            };
        }

        private string GenerateReward(IEnumerable<string> rewardsFound)
        {
            var rand = rng.Next(13);
            var rarity = (rand < 1) ? 2 : ((rand < 4) ? 1 : 0);
            return _gameModel.Abilities.Values
                .Where(ability => !rewardsFound.Contains(ability.Code))
                .Where(ability => rarity == ability.Rarity)
                .Random()?
                .Code ?? string.Empty;
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

            if (!_state.Units.Values.Any(u => "GOOD" == u.Model.Faction))
            {
                return candidates.MinBy(hex => hex.Y);
            }

            return candidates
                .MaxBy(hex => _state.Units.Values
                    .Where(u => "GOOD" == u.Model.Faction)
                    .Where(u => u.Location is not null)
                    .Min(u => (u.Location - hex).Magnitude));
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

            List<IBattleAction> actions = [];
            _state.Round++;
            actions.Add(new RoundAction()
            {
                Round = _state.Round
            });
            //Debug.WriteLine($"Round [{_state.Round}]");
            actions.AddRange(SpawnUnits());
            return actions;
        }

        private List<IBattleAction> SpawnUnits()
        {
            List<IBattleAction> actions = [];
            foreach (var spawn in _state.Spawns)
            {
                actions.AddRange(ProcSpawn(spawn));
            }

            return actions;
        }

        private List<IBattleAction> ProcSpawn(BattleSpawn spawn)
        {
            List<IBattleAction> actions = [];
            var count = spawn.Spawn(_state.Round);
            foreach (var _ in Enumerable.Range(0, count))
            {
                var action = TrySpawnUnit(spawn.UnitCode);
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

            //Debug.WriteLine(unitCode);

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
