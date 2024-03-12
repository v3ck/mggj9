using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class VoidOrbAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "VOID_ORB";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new VoidOrbAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _state.Units.Values.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is RoundAction)
            {
                _currentCharge += 1;
                return;
            }

            if (action is not MoveAction moveAction)
            {
                return;
            }

            if (moveAction.UnitId != _user.Id)
            {
                return;
            }

            _currentCharge = 0;
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
                .MaxBy(unit => unit.Health);

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
            actions.AddRange(target.Damage(8));

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

            if (5 < (unit.Location - _user.Location).Magnitude)
            {
                return false;
            }

            return (unit.Model.Faction != _user.Model.Faction);
        }
    }
}
