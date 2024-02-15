namespace Logic.Util
{
    public class IntVector2(int x, int y) : Tuple<int, int>(x, y)
    {
        public int X => Item1;
        public int Y => Item2;
    }
}
