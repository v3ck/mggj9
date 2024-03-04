namespace Logic.Simulation.Actions
{
    internal class ScoreAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Score;

        public required int Amount { get; init; }
    }
}
