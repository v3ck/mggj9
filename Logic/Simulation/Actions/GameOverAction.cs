namespace Logic.Simulation.Actions
{
    internal class GameOverAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.GameOver;

        public required bool IsVictory { get; init; }

        public required int Round { get; init; }

        public required int Score { get; init; }
    }
}
