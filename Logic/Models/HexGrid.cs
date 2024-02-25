namespace Logic.Models
{
    internal class HexGrid
    {
        private readonly HashSet<Hex> _hexes = [];
        public IEnumerable<Hex> Hexes => _hexes.AsEnumerable();

        private readonly Hex _centre;
        public Hex Centre => _centre;

        public HexGrid(int radius)
        {
            _centre = new Hex(radius, radius);
            _hexes.Add(_centre);

            foreach (int i in Enumerable.Range(1, radius))
            {
                foreach (int j in Enumerable.Range(0, i))
                {
                    _hexes.Add(_centre + new Hex(-i, j));
                    _hexes.Add(_centre + new Hex(i, -j));
                    _hexes.Add(_centre + new Hex(j - i, i));
                    _hexes.Add(_centre + new Hex(i - j, -i));
                    _hexes.Add(_centre + new Hex(j, i - j));
                    _hexes.Add(_centre + new Hex(-j, j - i));
                }
            }
        }

        public IEnumerable<Hex> AtDistance(Hex hex, int distance)
        {
            return _hexes.Where(x => (x - hex).Magnitude == distance);
        }

        public IEnumerable<Hex> WithinDistance(Hex hex, int minDistance, int maxDistance)
        {
            return _hexes.Where(x => {
                var magnitude = (hex - x).Magnitude;
                return minDistance <= magnitude && magnitude <= maxDistance; });
        }
    }
}
