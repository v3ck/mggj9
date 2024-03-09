using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class StompAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "STOMP";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new StompAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user.Location is null)
            {
                return false;
            }

            return _state.Units.Values
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => unit.Location is not null)
                .Where(unit => 1 == (_user.Location - unit.Location).Magnitude)
                .Any();
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

            var targets =
            _state.Units.Values
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => unit.Location is not null)
                .Where(unit => 1 == (_user.Location - unit.Location).Magnitude);
            List<IBattleAction> actions = [];
            foreach (var unit in targets)
            {
                actions.AddRange(GetActionsForUnit(unit));
            }

            return actions;
        }

        private List<IBattleAction> GetActionsForUnit(BattleUnit target)
        {
            if (target?.Location is null)
            {
                return [];
            }

            var oldHealth = target.Health;
            target.Damage(1);

            List<IBattleAction> actions = [];
            actions.Add(new HealthAction()
            {
                UnitId = target.Id,
                Location = target.Location,
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
                    Location = target.Location,
                    Exists = false
                });

                _state.Units.Remove(target.Id);
            }

            return actions;
        }
    }
}
