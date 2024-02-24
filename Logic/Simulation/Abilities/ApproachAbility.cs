using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class ApproachAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "APPROACH";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ApproachAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            var anEnemyExists = _state.Units.Values
                .Any(unit => unit.Location is not null &&
                unit.Model.Faction != _user.Model.Faction);

            var isEnemyTooClose = _state.Units.Values
                .Any(unit => unit.Location is not null &&
                (unit.Location - _user.Location).Magnitude <= 1 &&
                unit.Model.Faction != _user.Model.Faction);

            var isRoomToMove = _gameModel.Grid.AtDistance(_user.Location, 1)
                .Any((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex));

            return anEnemyExists && !isEnemyTooClose && isRoomToMove;
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

            var target = candidates.MinBy(CalculateDistanceToClosestEnemy);
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

        private int CalculateDistanceToClosestEnemy(Hex location)
        {
            return _state.Units.Values
                .Where(unit => unit.Model.Faction != _user.Model.Faction &&
                    unit.Location is not null)
                .Min(unit => (unit.Location - location).Magnitude);
        }
    }
}
