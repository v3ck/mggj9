namespace Logic.Simulation.Actions
{
    internal class RoundAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Round;

        public required int Round { get; init; }
    }
}
