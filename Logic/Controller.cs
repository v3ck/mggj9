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

        public int AddUnit(UnitCode code)
        {
            var unit = new UnitModel()
            {
                Code = code,
                IsEnemy = false // TODO
            };
            _model.Units.Add(unit.Id, unit);
            return unit.Id;
        }

        public AbilityCode[] GetUnitAbilities(int unitId)
        {
            if (!_model.Units.TryGetValue(unitId, out var unit))
            {
                return [];
            }

            return [.. unit.Abilities];
        }

        public void UpdateUnitAbilities(int unitId, AbilityCode[] abilities)
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
                FromLocation = new IntVector2(moveAction.FromLocation.X, moveAction.FromLocation.Y),
                ToLocation = new IntVector2(moveAction.ToLocation.X, moveAction.ToLocation.Y),
                UnitId = moveAction.UnitId,
                IsTeleport = moveAction.IsTeleport
            };
        }
    }
}
