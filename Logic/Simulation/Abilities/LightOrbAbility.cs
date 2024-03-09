using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class LightOrbAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "LIGHT_ORB";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new LightOrbAbility(model, user, state, gameModel);
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
                .Shuffle()
                .MinBy(unit => (unit.Model.Faction == _user.Model.Faction) ? 0 : 1);

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

            var oldHealth = target.Health;
            if (target.Model.Faction == _user.Model.Faction)
            {
                target.Heal(1);
            }
            else
            {
                target.Damage(1);
            }

            if (oldHealth != target.Health)
            {
                actions.Add(new HealthAction()
                {
                    UnitId = target.Id,
                    Location = target.Location,
                    Amount = target.Health,
                    PreviousAmount = oldHealth,
                    SourceUnitId = _user.Id
                });
            }

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
            if (_user.Location is null)
            {
                return false;
            }

            if (unit.Location is null)
            {
                return false;
            }

            var distance = (unit.Location - _user.Location).Magnitude;
            if ((distance < 1) || (3 < distance))
            {
                return false;
            }
            
            return (unit.Model.Faction != _user.Model.Faction ||
                    unit.Health < unit.Model.MaxHealth);
        }
    }
}
