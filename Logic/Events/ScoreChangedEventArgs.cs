namespace Logic.Events
{
    public class ScoreChangedEventArgs : EventArgs
    {
        public required int Amount { get; init; }
    }
}
