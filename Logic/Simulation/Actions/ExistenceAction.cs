using Logic.Models;

namespace Logic.Simulation.Actions
{
    internal class ExistenceAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Existence;

        public required int UnitId { get; init; }

        public required string UnitCode { get; init; }

        public required bool Exists { get; init; }

        public required Hex Location { get; init; }
    }
}
