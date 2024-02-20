using Logic.Util;

namespace Logic.Events
{
    public class ExistenceChangedEventArgs : EventArgs
    {
        public required int UnitId { get; init; }

        public required IntVector2 Location { get; init; }

        public required bool Exists { get; init; }

        public required string UnitCode { get; init; }
    }
}
