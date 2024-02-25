using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;
using System.ComponentModel.Design;

namespace Logic.Simulation.Abilities
{
    internal class AssassinateAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "ASSASSINATE";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new AssassinateAbility(model, user, state, gameModel);
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

            var targetUnit = _state.Units.Values
                .Where(IsTargetValid)
                .MinBy(unit => unit.Health);

            if (targetUnit?.Location is null)
            {
                return [];
            }

            var targetLocation = _gameModel.Grid.AtDistance(targetUnit.Location, 1)
                .Where(hex => (hex - _user.Location).Magnitude < 3)
                .Where(hex => !_state.Units.Values.Any(otherUnit => otherUnit.Location == hex))
                .Shuffle()
                .MinBy(hex => _state.Units.Values
                    .Where(unit => unit.Location is not null)
                    .Where(unit => unit.Model.Faction != _user.Model.Faction)
                    .Where(unit => 1 == (hex - unit.Location).Magnitude)
                    .Count());

            if (targetLocation == null)
            {
                return [];
            }

            List<IBattleAction> actions = [];

            var oldLocation = _user.Location;
            _user.Location = targetLocation;
            actions.Add(new MoveAction()
            {
                UnitId = _user.Id,
                FromLocation = oldLocation,
                ToLocation = targetLocation,
                IsTeleport = true
            });

            actions.Add(new AbilityAction()
            {
                BeginLocation = targetLocation,
                EndLocation = targetUnit.Location,
                Ability = Code
            });

            targetUnit.Health -= 4;

            actions.Add(new HealthAction()
            {
                UnitId = targetUnit.Id,
                Location = targetUnit.Location,
                Amount = targetUnit.Health
            });

            if (targetUnit.Health <= 0)
            {
                actions.Add(new ExistenceAction()
                {
                    UnitId = targetUnit.Id,
                    UnitCode = targetUnit.Model.Code,
                    Location = targetUnit.Location,
                    Exists = false
                });

                _state.Units.Remove(targetUnit.Id);
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

            if (unit.Model.Faction == _user.Model.Faction)
            {
                return false;
            }

            return _gameModel.Grid.AtDistance(unit.Location, 1)
                .Where(hex => (hex - _user.Location).Magnitude < 3)
                .Where(hex => !_state.Units.Values.Any(otherUnit => otherUnit.Location == hex))
                .Any();
        }
    }
}
