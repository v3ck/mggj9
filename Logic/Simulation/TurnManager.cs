using System.Diagnostics;

namespace Logic.Simulation
{
    internal class TurnManager
    {
        public event EventHandler<EventArgs>? RoundCompleted;

        private readonly List<int> _unitOrder = [];

        private int _index = 0;

        private bool _started = false;

        public (int, bool) Next()
        {
            _started = true;
            var next = _unitOrder[_index];
            Debug.WriteLine($"Turn [{_index}]");
            _index = (_unitOrder.Count <= (_index + 1)) ? 0 : (_index + 1);
            return (next, 0 == _index);
        }

        public void Add(int id)
        {
            _unitOrder.Insert(_index, id);
            if (_started)
            {
                _index++;
            }
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
