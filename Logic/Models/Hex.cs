namespace Logic.Models
{
    internal class Hex(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public int Z => -(X + Y);

        public int Magnitude => (Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z)) / 2;

        public static Hex operator -(Hex a, Hex b)
        {
            return new Hex(a.X - b.X, a.Y - b.Y);
        }

        public static Hex operator +(Hex a, Hex b)
        {
            return new Hex(a.X + b.X, a.Y + b.Y);
        }
    }
}
