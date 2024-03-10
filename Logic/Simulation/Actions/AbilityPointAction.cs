namespace Logic.Simulation.Actions
{
    internal class AbilityPointAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.AbilityPoint;

        public required int UnitId { get; init; }

        public required int Amount { get; init; }
    }
}
