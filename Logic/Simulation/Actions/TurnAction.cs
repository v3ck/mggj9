namespace Logic.Simulation.Actions
{
    internal class TurnAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Turn;

        public required int UnitId { get; init; }
        
        public required string UnitCode { get; init; }

        public required string? AbilityCode { get; init; }
    }
}
