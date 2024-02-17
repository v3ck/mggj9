using Logic.Events;
using Logic.Models;
using Logic.Simulation;
using Logic.Simulation.Actions;
using Logic.Util;

namespace Logic
{
    internal class Controller : IController
    {
        public event EventHandler<UnitMovedEventArgs>? UnitMoved;

        public event EventHandler<StatusChangedEventArgs>? StatusChanged;

        public event EventHandler<HealthChangedEventArgs>? HealthChanged;

        public event EventHandler<AbilityFiredEventArgs>? AbilityFired;

        private readonly GameModel _model = new(new GameParameters()
        {
            GridRadius = 5
        });

        private BattleSimulator? _simulator = null;

        public void StartBattle()
        {
            _simulator = new(_model);
        }

        public void TakeTurn()
        {
            var actions = _simulator?.TakeTurn() ?? Enumerable.Empty<IBattleAction>();
            foreach (var action in actions)
            {
                PropagateAction(action);
            }
        }

        public int AddUnit(string code, int health, string faction, string[] abilities)
        {
            var unit = new UnitModel()
            {
                Code = code,
                Faction = faction,
                MaxHealth = health
            };
            _model.Units.Add(unit.Id, unit);
            UpdateUnitAbilities(unit.Id, abilities);
            return unit.Id;
        }

        public string[] GetUnitAbilities(int unitId)
        {
            if (!_model.Units.TryGetValue(unitId, out var unit))
            {
                return [];
            }

            return [.. unit.Abilities];
        }

        public void UpdateUnitAbilities(int unitId, string[] abilities)
        {
            if (!_model.Units.TryGetValue(unitId, out var unit))
            {
                return;
            }

            unit.Abilities.Clear();
            foreach (var code in abilities)
            {
                unit.Abilities.Add(code);
            }
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
                Status = statusAction.Status
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
    }
}
