using Logic.Models;
using Logic.Util;

namespace Logic.Simulation.Actions
{
    internal class AbilityAction() : BattleActionBase(ActionType.Ability)
    {
        public required Hex BeginLocation { get; init; }

        public required Hex EndLocation { get; init; }

        public required AbilityCode Ability { get; init; }
    }
}
