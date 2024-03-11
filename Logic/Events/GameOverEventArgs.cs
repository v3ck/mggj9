namespace Logic.Events
{
    public class GameOverEventArgs : EventArgs
    {
        public required bool IsVictory { get; init; }

        public required int Round { get; init; }

        public required int Score { get; init; }
    }
}
