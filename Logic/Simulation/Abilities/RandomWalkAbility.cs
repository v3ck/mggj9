using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;
using Logic.Util;

namespace Logic.Simulation.Abilities
{
    internal class RandomWalkAbility(BattleState state, HexGrid grid) : IBattleAbility
    {
        public static string Code => "RANDOM_WALK";

        public int CurrentCharge { get; set; } = 0;
        public int MaxCharge { get; } = 0;

        private readonly BattleState _state = state;

        private readonly HexGrid _grid = grid;

        public bool CanUse(BattleUnit user)
        {
            if (null == user?.Location)
            {
                return false;
            }

            return _grid.AtDistance(user.Location, 1).
                Any((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex));
        }

        public void TryCharge(IBattleAction action)
        {
            // void
        }

        public IEnumerable<IBattleAction> Use(BattleUnit user)
        {
            if (null == user?.Location)
            {
                yield break;
            }

            var target = _grid.AtDistance(user.Location, 1).
                Where((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex)).
                Random();
            if (null == target)
            {
                yield break;
            }

            var oldLocation = user.Location;
            user.Location = target;
            yield return new MoveAction()
            {
                UnitId = user.Model.Id,
                FromLocation = oldLocation,
                ToLocation = target,
                IsTeleport = false
            };
        }
    }
}
