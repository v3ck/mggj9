namespace Logic.Simulation
{
    internal class TurnManager
    {
        private readonly List<int> _unitOrder = new();

        private int _index = 0;

        private int _round = 0;
        public int Round => _round;

        public void Init(IEnumerable<int> ids)
        {
            _unitOrder.InsertRange(_index, ids);
        }

        public int Next()
        {
            var next = _unitOrder[_index];
            _index = (_unitOrder.Count <= (_index + 1)) ? 0 : (_index + 1);
            if (0 == _index)
            {
                _round++;
            }
            return next;
        }

        public void Add(int id)
        {
            _unitOrder.Insert(_index, id);
            _index++;
        }

        public void Remove(int id)
        {
            var removeIndex = _unitOrder.IndexOf(id);
            _unitOrder.RemoveAt(removeIndex);
            if (removeIndex < _index)
            {
                _index--;
            }
        }
    }
}
