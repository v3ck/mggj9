using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class SparkAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "SPARK";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SparkAbility(model, user, state, gameModel);
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

            actions.Add(new HealthAction()
            {
                UnitId = target.Id,
                Location = target.Location,
                Amount = 2
            });

            target.Health -= 2;
            if (target.Health <= 0)
            {
                actions.Add(new ExistenceAction()
                {
                    UnitId = target.Id,
                    UnitCode = target.Model.Code,
                    Location = target.Location,
                    Exists = false
                });

                _state.Units.Remove(target.Id);
            }

            return actions;
        }

        private bool IsTargetValid(BattleUnit unit)
        {
            return _user.Location is not null &&
                unit.Location is not null &&
                (unit.Location - _user.Location).Magnitude <= 2 &&
                unit.Model.Faction != _user.Model.Faction;
        }
    }
}
