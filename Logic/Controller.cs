using Logic.Events;
using Logic.Models;
using Logic.Simulation;
using Logic.Simulation.Actions;
using System.Diagnostics;

namespace Logic
{
    internal class Controller : IController
    {
        private const int MAX_TURN_ATTEMPTS = 255;

        public event EventHandler<UnitMovedEventArgs>? UnitMoved;

        public event EventHandler<StatusChangedEventArgs>? StatusChanged;

        public event EventHandler<HealthChangedEventArgs>? HealthChanged;

        public event EventHandler<AbilityFiredEventArgs>? AbilityFired;

        public event EventHandler<ExistenceChangedEventArgs>? ExistenceChanged;

        public event EventHandler<RewardObtainedEventArgs>? RewardObtained;

        public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;

        public event EventHandler<RoundChangedEventArgs>? RoundChanged;

        public event EventHandler<AbilityPointsChangedEventArgs>? AbilityPointsChanged;

        public event EventHandler<GameOverEventArgs>? GameOver;

        private readonly GameModel _model = new(new GameParameters()
        {
            GridRadius = 5,
            RewardInterval = 5
        });

        private BattleSimulator? _simulator = null;

        private bool _isGameOver = false;

        public void StartBattle()
        {
            //Debug.WriteLine(_model.Abilities.Values.Where(x => 0 == x.Rarity).Count().ToString());
            //Debug.WriteLine(_model.Abilities.Values.Where(x => 1 == x.Rarity).Count().ToString());
            //Debug.WriteLine(_model.Abilities.Values.Where(x => 2 == x.Rarity).Count().ToString());

            _simulator = new(_model);
            var actions = _simulator.Start();
            foreach (var action in actions)
            {
                PropagateAction(action);
            }
        }

        public void TakeTurn()
        {
            foreach (var _ in Enumerable.Range(0, MAX_TURN_ATTEMPTS))
            {
                if (TryTakeTurn())
                {
                    return;
                }
            }

            if (!_isGameOver)
            {
                throw new Exception("The simulation is stuck.");
            }
        }

        private bool TryTakeTurn()
        {
            var actions = _simulator?.TakeTurn() ?? Enumerable.Empty<IBattleAction>();
            foreach (var action in actions)
            {
                PropagateAction(action);
            }

            if (!actions.Any(action =>
                (action is TurnAction turnAction) &&
                (turnAction.AbilityCode is not null)))
            {
                return false;
            }

            return true;
        }

        public void AddUnit(string code, int health, string faction, string[] abilities)
        {
            var unit = new UnitModel()
            {
                Code = code,
                Faction = faction,
                MaxHealth = health
            };
            _model.Units.Add(unit.Code, unit);
            unit.Abilities.AddRange(abilities);
        }

        public string[] GetUnitAbilities(string unitCode)
        {
            if (!_model.Units.TryGetValue(unitCode, out var unit))
            {
                return [];
            }

            return [.. unit.Abilities];
        }

        public string[] GetAllCodexAbilities()
        {
            return [.. _model.CodexAbilities];
        }

        public string[] GetCodexAbilities(string unitCode)
        {
            if (!_model.Units.TryGetValue(unitCode, out var unit))
            {
                return [];
            }

            return [.. _model.CodexAbilities
                .Where(abilityCode => !unit.Abilities.Contains(abilityCode))
                .Distinct()
                .Order()];
        }

        public void AddCodexAbility(string abilityCode)
        {
            _model.CodexAbilities.Add(abilityCode);
        }

        public void MoveUnitAbilityUp(string unitCode, string abilityCode)
        {
            if (!_model.Units.TryGetValue(unitCode, out var unit))
            {
                return;
            }

            if (!unit.Abilities.Contains(abilityCode))
            {
                return;
            }

            var index = unit.Abilities.IndexOf(abilityCode);
            if (0 == index)
            {
                return;
            }

            unit.Abilities.RemoveAt(index);
            unit.Abilities.Insert(index - 1, abilityCode);
            _simulator?.UpdateUnitAbilities(unitCode);
        }

        public void MoveUnitAbilityDown(string unitCode, string abilityCode)
        {
            if (!_model.Units.TryGetValue(unitCode, out var unit))
            {
                return;
            }

            if (!unit.Abilities.Contains(abilityCode))
            {
                return;
            }

            var index = unit.Abilities.IndexOf(abilityCode);
            if (unit.Abilities.Count - 1 <= index)
            {
                return;
            }

            unit.Abilities.RemoveAt(index);
            unit.Abilities.Insert(index + 1, abilityCode);
            _simulator?.UpdateUnitAbilities(unitCode);
        }

        public void EquipAbility(string unitCode, string abilityCode)
        {
            if (!_model.Units.TryGetValue(unitCode, out var unit))
            {
                return;
            }

            if (!_model.CodexAbilities.Contains(abilityCode))
            {
                return;
            }

            unit.Abilities.Add(abilityCode);
            _model.CodexAbilities.Remove(abilityCode);
            _simulator?.UpdateUnitAbilities(unitCode);
        }

        public void UnequipAbility(string unitCode, string abilityCode)
        {
            if (!_model.Units.TryGetValue(unitCode, out var unit))
            {
                return;
            }

            if (!unit.Abilities.Contains(abilityCode))
            {
                return;
            }

            unit.Abilities.Remove(abilityCode);
            _model.CodexAbilities.Add(abilityCode);
            _simulator?.UpdateUnitAbilities(unitCode);
        }

        public void AddSpawn(int beginRound, int endRound, string unitCode, double rate, double volatility)
        {
            _model.Spawns.Add(new SpawnModel()
            {
                BeginRound = beginRound,
                EndRound = endRound,
                UnitCode = unitCode,
                Rate = rate,
                Volatility = volatility
            });
        }

        public void AddAbility(string abilityCode, int maxCharge, int cost, int rarity)
        {
            _model.Abilities.Add(
                abilityCode,
                new AbilityModel()
                {
                    Code = abilityCode,
                    MaxCharge = maxCharge,
                    Cost = cost,
                    Rarity = rarity
                });
        }

        private void PropagateAction(IBattleAction action)
        {
            switch (action.Type)
            {
                case ActionType.Move:
                    UnitMoved?.Invoke(this, MoveActionToEventArgs(action));
                    break;
                case ActionType.Status:
                    StatusChanged?.Invoke(this, StatusActionToEventArgs(action));
                    break;
                case ActionType.Health:
                    HealthChanged?.Invoke(this, HealthActionToEventArgs(action));
                    break;
                case ActionType.Ability:
                    AbilityFired?.Invoke(this, AbilityActionToEventArgs(action));
                    break;
                case ActionType.Existence:
                    ExistenceChanged?.Invoke(this, ExistenceActionToEventArgs(action));
                    break;
                case ActionType.Reward:
                    RewardObtained?.Invoke(this, RewardActionToEventArgs(action));
                    break;
                case ActionType.Score:
                    ScoreChanged?.Invoke(this, ScoreActionToEventArgs(action));
                    break;
                case ActionType.Round:
                    RoundChanged?.Invoke(this, RoundActionToEventArgs(action));
                    break;
                case ActionType.AbilityPoint:
                    AbilityPointsChanged?.Invoke(this, AbilityPointActionToEventArgs(action));
                    break;
                case ActionType.GameOver:
                    GameOver?.Invoke(this, GameOverActionToEventArgs(action));
                    _isGameOver = true;
                    break;
            }
        }

        private static UnitMovedEventArgs MoveActionToEventArgs(IBattleAction action)
        {
            if (action is not MoveAction moveAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new UnitMovedEventArgs()
            {
                FromLocation = moveAction.FromLocation.AsIntVector2,
                ToLocation = moveAction.ToLocation.AsIntVector2,
                UnitId = moveAction.UnitId,
                IsTeleport = moveAction.IsTeleport
            };
        }

        private static StatusChangedEventArgs StatusActionToEventArgs(IBattleAction action)
        {
            if (action is not StatusAction statusAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new StatusChangedEventArgs()
            {
                UnitId = statusAction.UnitId,
                Location = statusAction.Location.AsIntVector2,
                Status = statusAction.Status,
                Active = statusAction.Active
            };
        }

        private static HealthChangedEventArgs HealthActionToEventArgs(IBattleAction action)
        {
            if (action is not HealthAction healthAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new HealthChangedEventArgs()
            {
                UnitId = healthAction.UnitId,
                Location = healthAction.Location.AsIntVector2,
                Amount = healthAction.Amount
            };
        }

        private static AbilityFiredEventArgs AbilityActionToEventArgs(IBattleAction action)
        {
            if (action is not AbilityAction abilityAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new AbilityFiredEventArgs()
            {
                FromLocation = abilityAction.BeginLocation.AsIntVector2,
                ToLocation = abilityAction.EndLocation.AsIntVector2,
                Ability = abilityAction.Ability
            };
        }

        private static ExistenceChangedEventArgs ExistenceActionToEventArgs(IBattleAction action)
        {
            if (action is not ExistenceAction existenceAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new ExistenceChangedEventArgs()
            {
                UnitId = existenceAction.UnitId,
                Location = existenceAction.Location.AsIntVector2,
                Exists = existenceAction.Exists,
                UnitCode = existenceAction.UnitCode
            };
        }

        private static RewardObtainedEventArgs RewardActionToEventArgs(IBattleAction action)
        {
            if (action is not RewardAction rewardAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new RewardObtainedEventArgs()
            {
                Abilities = rewardAction.AbilityCodes
            };
        }

        private static ScoreChangedEventArgs ScoreActionToEventArgs(IBattleAction action)
        {
            if (action is not ScoreAction scoreAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new ScoreChangedEventArgs()
            {
                Amount = scoreAction.Amount
            };
        }

        private static RoundChangedEventArgs RoundActionToEventArgs(IBattleAction action)
        {
            if (action is not RoundAction roundAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new RoundChangedEventArgs()
            {
                Round = roundAction.Round
            };
        }

        private static AbilityPointsChangedEventArgs AbilityPointActionToEventArgs(IBattleAction action)
        {
            if (action is not AbilityPointAction abilityPointAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new AbilityPointsChangedEventArgs()
            {
                UnitId = abilityPointAction.UnitId,
                Amount = abilityPointAction.Amount
            };
        }

        private static GameOverEventArgs GameOverActionToEventArgs(IBattleAction action)
        {
            if (action is not GameOverAction gameOverAction)
            {
                throw new ArgumentException("Incorrect ActionType", nameof(action));
            }

            return new GameOverEventArgs()
            {
                IsVictory = gameOverAction.IsVictory,
                Round = gameOverAction.Round,
                Score = gameOverAction.Score
            };
        }
    }
}
