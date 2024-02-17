using Logic.Models;
using Logic.Util;

namespace Logic.Simulation.Actions
{
    internal class AbilityAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Ability;

        public required Hex BeginLocation { get; init; }

        public required Hex EndLocation { get; init; }

        public required string Ability { get; init; }
    }
}
