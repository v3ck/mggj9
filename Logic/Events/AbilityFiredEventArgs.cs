using Logic.Util;

namespace Logic.Events
{
    public class AbilityFiredEventArgs : EventArgs
    {
        public required IntVector2 FromLocation { get; init; }

        public required IntVector2 ToLocation { get; init; }

        public required string Ability { get; init; }
    }
}
