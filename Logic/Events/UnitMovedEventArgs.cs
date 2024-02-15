using Logic.Util;

namespace Logic.Events
{
    public class UnitMovedEventArgs : EventArgs
    {
        public required int UnitId { get; init; }

        public required IntVector2 FromLocation { get; init; }

        public required IntVector2 ToLocation { get; init; }

        public bool IsTeleport { get; init; }

    }
}
