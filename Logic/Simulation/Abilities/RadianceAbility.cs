using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class RadianceAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "RADIANCE";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new RadianceAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _state.Units.Values.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is not HealthAction healthAction)
            {
                return;
            }

            if (healthAction.SourceUnitId != _user.Id)
            {
                return;
            }

            _currentCharge += Math.Max(0, healthAction.PreviousAmount - healthAction.Amount);
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var targets = _state.Units.Values.Where(IsTargetValid);

            List<IBattleAction> actions = [];
            foreach (var target in targets)
            {
                actions.AddRange(HealUnit(target));
            }
            return actions;
        }

        private List<IBattleAction> HealUnit(BattleUnit unit)
        {

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = _user.Location,
                EndLocation = unit.Location,
                Ability = Code
            });

            var oldHealth = unit.Health;
            unit.Heal(4);
            if (oldHealth != unit.Health)
            {
                actions.Add(new HealthAction()
                {
                    UnitId = unit.Id,
                    Location = unit.Location,
                    Amount = unit.Health,
                    PreviousAmount = oldHealth,
                    SourceUnitId = _user.Id
                });
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

            if (unit.Model.Faction != _user.Model.Faction)
            {
                return false;
            }

            if (4 < (unit.Location - _user.Location).Magnitude)
            {
                return false;
            }

            return (unit.Health < unit.Model.MaxHealth);
        }
    }
}
