using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal abstract class ApproachAbility(
        AbilityModel model,
        BattleUnit user,
        BattleState state,
        GameModel gameModel,
        int minDistance,
        bool approachAllies,
        bool approachEnemies)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        private readonly int _minDistance = minDistance;
        private readonly bool _approachAllies = approachAllies;
        private readonly bool _approachEnemies = approachEnemies;

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

            if (_state.Units.Values
                .Any(unit => unit.Location is not null &&
                (unit.Location - _user.Location).Magnitude <= _minDistance &&
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

            var target = candidates.MinBy(CalculateDistanceToClosestUnit);
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
            if (unit.Id == _user.Id)
            {
                return false;
            }

            if (!_approachAllies && (unit.Model.Faction == _user.Model.Faction))
            {
                return false;
            }

            if (!_approachEnemies && (unit.Model.Faction != _user.Model.Faction))
            {
                return false;
            }

            return true;
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

            return distanceAfter < distanceBefore;
        }
    }
}
