using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class TimeStopAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "TIME_STOP";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new TimeStopAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _user?.Location is not null;
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is not RoundAction)
            {
                return;
            }

            _currentCharge = (1 < _user.AbilityPoints) ? 0 : _currentCharge + 1;
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            foreach (var unit in _state.Units.Values)
            {
                var action = StunUnit(unit);
                if (action is not null)
                {
                    actions.Add(action);
                }
            }

            return actions;
        }

        private IBattleAction? StunUnit(BattleUnit unit)
        {
            if (unit?.Location is null)
            {
                return null;
            }

            if (unit.Id == _user.Id)
            {
                return null;
            }

            unit.StunTurns += 2;
            return new StatusAction()
            {
                UnitId = unit.Id,
                Location = unit.Location,
                Status = "STUN",
                Active = true
            };
        }
    }
}
