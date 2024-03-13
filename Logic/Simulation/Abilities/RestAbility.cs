using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class RestAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "REST";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new RestAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _user.Health < _user.Model.MaxHealth;
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

            var oldHealth = _user.Health;
            _user.Heal(1);
            if (oldHealth == _user.Health)
            {
                return [];
            }

            return [new HealthAction()
            {
                UnitId = _user.Id,
                Location = _user.Location,
                Amount = _user.Health,
                PreviousAmount = oldHealth,
                SourceUnitId = _user.Id
            }];
        }
    }
}
