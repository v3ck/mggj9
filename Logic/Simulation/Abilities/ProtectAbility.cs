using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class ProtectAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "PROTECT";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ProtectAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _state.Units.Values.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            // void
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var target = _state.Units.Values
                .Where(IsTargetValid)
                .Random();

            if (target?.Location is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = _user.Location,
                EndLocation = target.Location,
                Ability = Code
            });

            target.ShieldCount += 1;
            actions.Add(new StatusAction()
            {
                UnitId = target.Id,
                Location = target.Location,
                Status = "SHIELD",
                Active = true

            });

            return actions;
        }

        private bool IsTargetValid(BattleUnit unit)
        {
            if (_user.Location is null)
            {
                return false;
            }

            if (unit.Location is null)
            {
                return false;
            }

            if (3 < (_user.Location - unit.Location).Magnitude)
            {
                return false;
            }

            if (unit.Model.Faction != _user.Model.Faction)
            {
                return false;
            }

            if (0 < unit.ShieldCount)
            {
                return false;
            }

            return (unit.Health < unit.Model.MaxHealth);
        }
    }
}
