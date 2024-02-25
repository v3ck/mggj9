using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class CentreWalkAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "CENTRE_WALK";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new CentreWalkAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _gameModel.Grid.AtDistance(_user.Location, 1).Any(CanMoveToHex);
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

            var target = _gameModel.Grid.AtDistance(_user.Location, 1)
                .Where(CanMoveToHex)
                .Random();
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

        private bool CanMoveToHex(Hex hex)
        {
            if (_state.Units.Values.Any(unit => unit.Location == hex))
            {
                return false;
            }

            return (_gameModel.Grid.Centre - hex).Magnitude < (_gameModel.Grid.Centre - _user.Location).Magnitude;
        }
    }
}
