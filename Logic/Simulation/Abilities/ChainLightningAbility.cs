using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class ChainLightningAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "CHAIN_LIGHTNING";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ChainLightningAbility(model, user, state, gameModel);
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
            if (action is not MoveAction moveAction)
            {
                return;
            }

            if (moveAction.UnitId != _user.Id)
            {
                return;
            }

            _currentCharge += (moveAction.ToLocation - moveAction.FromLocation).Magnitude;
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
                BeginLocation = location,
                EndLocation = target.Location,
                Ability = Code
            });

            hitIds.Add(target.Id);
            var oldHealth = target.Health;
            target.Health -= hitIds.Count;

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
            if (2 < distance)
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
