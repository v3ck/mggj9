namespace Logic.Models
{
    internal class HexGrid
    {
        private readonly HashSet<Hex> _hexes = new ();
        public IEnumerable<Hex> Hexes => _hexes.AsEnumerable();

        public HexGrid(int radius)
        {
            var centreHex = new Hex(radius, radius);
            _hexes.Add(centreHex);

            foreach (int i in Enumerable.Range(1, radius + 1))
            {
                foreach (int j in Enumerable.Range(0, i))
                {
                    _hexes.Add(centreHex + new Hex(-i, j));
                    _hexes.Add(centreHex + new Hex(i, -j));
                    _hexes.Add(centreHex + new Hex(j - i, i));
                    _hexes.Add(centreHex + new Hex(i - j, -i));
                    _hexes.Add(centreHex + new Hex(j, i - j));
                    _hexes.Add(centreHex + new Hex(-j, j - i));
                }
            }
        }

        public IEnumerable<Hex> AtDistance(Hex hex, int distance)
        {
            return _hexes.Where(x => (hex - x).Magnitude == distance);
        }

        public IEnumerable<Hex> WithinDistance(Hex hex, int minDistance, int maxDistance)
        {
            return _hexes.Where(x => {
                var magnitude = (hex - x).Magnitude;
                return minDistance <= magnitude && magnitude <= maxDistance; });
        }
    }
}
