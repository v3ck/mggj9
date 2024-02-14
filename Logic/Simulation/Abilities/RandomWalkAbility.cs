using Logic.Extensions;
using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class RandomWalkAbility(BattleState state, HexGrid grid) : IAbility
    {
        public int Id { get; init; }
        public int CurrentCharge { get; set; }
        public int MaxCharge { get; set; }

        public event EventHandler<BattleAction>? ActionPerformed;

        private readonly BattleState _state = state;

        private readonly HexGrid _grid = grid;

        public bool CanUse(Unit user)
        {
            if (null == user?.Location)
            {
                return false;
            }

            return _grid.AtDistance(user.Location, 1).
                Any((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex));
        }

        public void TryCharge(BattleAction action)
        {
            // void
        }

        public IEnumerable<BattleAction> Use(Unit user)
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

            user.Location = target;
            yield return new BattleAction()
            {
                AbilityId = Id,
                Source = user.Location,
                Target = target,
                Type = BattleAction.ActionType.Move
            };
        }
    }
}
