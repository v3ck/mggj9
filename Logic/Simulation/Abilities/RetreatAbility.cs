using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal abstract class RetreatAbility(
        AbilityModel model,
        BattleUnit user,
        BattleState state,
        GameModel gameModel,
        int maxDistance)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        private readonly int _maxDistance = maxDistance;

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            if (!_state.Units.Values
                .Any(unit => unit.Location is not null &&
                IsValidFaction(unit)))
            {
                return false;
            }

            if (!_state.Units.Values
                .Any(unit => unit.Location is not null &&
                (unit.Location - _user.Location).Magnitude < _maxDistance &&
                IsValidFaction(unit)))
            {
                return false;
            }

            return _gameModel.Grid.AtDistance(_user.Location, 1)
                .Any(CanMoveToHex);
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

            var candidates = _gameModel.Grid.AtDistance(_user.Location, 1)
                .Where((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex));
            if (!candidates.Any())
            {
                return [];
            }

            var target = candidates.MaxBy(CalculateDistanceToClosestUnit);
            if (target is null)
            {
                return [];
            }

            var oldLocation = _user.Location;
            _user.Location = target;
            return [new MoveAction()
            {
                UnitId = _user.Id,
                FromLocation = oldLocation,
                ToLocation = target,
                IsTeleport = false
            }];
        }

        private int CalculateDistanceToClosestUnit(Hex location)
        {
            return _state.Units.Values
                .Where(unit => IsValidFaction(unit) &&
                    unit.Location is not null)
                .Min(unit => (unit.Location - location).Magnitude);
        }

        private bool IsValidFaction(BattleUnit unit)
        {
            return unit.Model.Faction != _user.Model.Faction;
        }

        private bool CanMoveToHex(Hex hex)
        {
            if (_state.Units.Values.Any(unit => unit.Location == hex))
            {
                return false;
            }

            var distanceAfter = _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(IsValidFaction)
                .Min(unit => (unit.Location - hex).Magnitude);

            var distanceBefore = _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(IsValidFaction)
                .Min(unit => (unit.Location - _user.Location).Magnitude);

            return distanceBefore < distanceAfter;
        }
    }
}
