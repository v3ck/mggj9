using Logic.Util;

namespace Logic.Events
{
    public class StatusChangedEventArgs : EventArgs
    {
        public required int UnitId { get; init; }

        public required IntVector2 Location { get; init; }

        public required string Status { get; init; }

        public required bool Active { get; init; }
    }
}
