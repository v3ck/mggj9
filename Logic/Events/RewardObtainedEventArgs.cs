namespace Logic.Events
{
    public class RewardObtainedEventArgs : EventArgs
    {
        public required string[] Abilities { get; init; }
    }
}
