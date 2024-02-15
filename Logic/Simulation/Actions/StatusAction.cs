using Logic.Models;
using Logic.Util;

namespace Logic.Simulation.Actions
{
    internal class StatusAction() : BattleActionBase(ActionType.Status)
    {
        public required int UnitId { get; init; }

        public required Hex Location { get; init; }

        public required StatusCode Status { get; init; }
    }
}
