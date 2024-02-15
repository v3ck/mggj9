using Logic.Util;

namespace Logic.Simulation.Actions
{
    internal class HealthAction() : BattleActionBase(ActionType.Health)
    {
        public required AbilityCode Action { get; init; }

        public required int UnitId { get; init; }

        public required int Amount { get; init; }
    }
}
