using Logic.Models;

namespace Logic.Simulation.Actions
{
    internal class HealthAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Health;

        public required Hex Location { get; init; }

        public required int UnitId { get; init; }

        public required int Amount { get; init; }

        public required int PreviousAmount { get; init; }
    }
}
