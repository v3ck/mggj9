using Logic.Models;
using Logic.Util;

namespace Logic.Simulation.Actions
{
    internal class StatusAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Status;

        public required int UnitId { get; init; }

        public required Hex Location { get; init; }

        public required string Status { get; init; }
    }
}
