using Logic.Util;

namespace Logic.Events
{
    public class HealthChangedEventArgs : EventArgs
    {
        public required IntVector2 Location { get; init; }

        public required int UnitId { get; init; }

        public required int Amount { get; init; }
    }
}
