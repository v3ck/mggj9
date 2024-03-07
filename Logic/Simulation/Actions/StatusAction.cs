using Logic.Models;

namespace Logic.Simulation.Actions
{
    internal class StatusAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Status;

        public required int UnitId { get; init; }

        public required Hex Location { get; init; }

        public required string Status { get; init; }

        public required bool Active { get; init; }
    }
}
