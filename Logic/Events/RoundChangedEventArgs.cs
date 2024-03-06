namespace Logic.Events
{
    public class RoundChangedEventArgs : EventArgs
    {
        public required int Round { get; init; }
    }
}
