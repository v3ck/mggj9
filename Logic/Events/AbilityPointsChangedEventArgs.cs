namespace Logic.Events
{
    public class AbilityPointsChangedEventArgs : EventArgs
    {
        public required int UnitId { get; init; }

        public required int Amount { get; init; }
    }
}
