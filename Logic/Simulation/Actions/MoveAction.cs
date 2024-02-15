using Logic.Models;

namespace Logic.Simulation.Actions
{
    internal class MoveAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Move;

        public required int UnitId { get; init; }

        public required Hex FromLocation { get; init; }

        public required Hex ToLocation { get; init; }

        public required bool IsTeleport { get; init; }
    }
}
