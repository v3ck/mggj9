using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class RandomWalkAbility(BattleUnit user, BattleState state, HexGrid grid) : IBattleAbility
    {
        public string Code => "RANDOM_WALK";

        public int CurrentCharge { get; set; } = 0;
        public int MaxCharge { get; } = 0;

        private readonly BattleState _state = state;

        private readonly HexGrid _grid = grid;

        private readonly BattleUnit _user = user;

        public static IBattleAbility Create(BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new RandomWalkAbility(user, state, gameModel.Grid);
        }

        public bool CanUse()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _grid.AtDistance(_user.Location, 1)
                .Any((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex));
        }

        public void TryCharge(IBattleAction action)
        {
            // void
        }

        public IEnumerable<IBattleAction> Use()
        {
            if (_user?.Location is null)
            {
                yield break;
            }

            var target = _grid.AtDistance(_user.Location, 1)
                .Where((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex))
                .Random();
            if (target is null)
            {
                yield break;
            }

            var oldLocation = _user.Location;
            _user.Location = target;
            yield return new MoveAction()
            {
                UnitId = _user.Id,
                FromLocation = oldLocation,
                ToLocation = target,
                IsTeleport = false
            };
        }
    }
}
