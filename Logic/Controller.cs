
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
            if (null == _simulator)
            {
                return;
            }

            var actions = _simulator.TakeTurn();
            foreach (var action in actions)
            {
                PropagateAction(action);
            }
        }

        private void PropagateAction(BattleActionBase action)
        {
            switch (action.Type)
            {
                case BattleActionBase.ActionType.Move:
                    UnitMoved?.Invoke(this, MoveActionToEventArgs(action));
                    break;
            }
        }

        private static UnitMovedEventArgs MoveActionToEventArgs(BattleActionBase action)
        {
            if (action is not MoveAction moveAction)
            {
                throw new ArgumentNullException(nameof(action));
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
