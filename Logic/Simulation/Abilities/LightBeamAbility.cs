using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class LightBeamAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "LIGHT_BEAM";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new LightBeamAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _state.Units.Values.Any(unit => IsTargetValid(_user.Location, unit, []));
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

            return UseChain(_user.Location, []);
        }

        private List<IBattleAction> UseChain(Hex location, List<int> hitIds)
        {
            if (2 <= hitIds.Count)
            {
                return [];
            }

            var target = _state.Units.Values
                .Where(unit => IsTargetValid(location, unit, hitIds))
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

            hitIds.Add(target.Id);
            var oldHealth = target.Health;
            actions.AddRange(target.Damage(2));

            actions.Add(new HealthAction()
            {
                UnitId = target.Id,
                Location = location,
                Amount = target.Health,
                PreviousAmount = oldHealth,
                SourceUnitId = _user.Id
            });

            if (target.Health <= 0)
            {
                actions.Add(new ExistenceAction()
                {
                    UnitId = target.Id,
                    UnitCode = target.Model.Code,
                    Location = location,
                    Exists = false
                });

                _state.Units.Remove(target.Id);
            }

            actions.AddRange(UseChain(target.Location, hitIds));

            return actions;
        }

        private bool IsTargetValid(Hex sourceHex, BattleUnit target, List<int> hitIds)
        {
            if (target.Location is null)
            {
                return false;
            }

            var distance = (target.Location - sourceHex).Magnitude;
            var range = (0 == hitIds.Count) ? 3 : 1;
            if (range < distance)
            {
                return false;
            }

            if (target.Model.Faction == _user.Model.Faction)
            {
                return false;
            }

            if (hitIds.Contains(target.Id))
            {
                return false;
            }

            return true;
        }
    }
}
