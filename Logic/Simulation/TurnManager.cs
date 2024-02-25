using System.Diagnostics;

namespace Logic.Simulation
{
    internal class TurnManager
    {
        public event EventHandler<EventArgs>? RoundCompleted;

        private readonly List<int> _unitOrder = [];

        private int _index = 0;

        public (int, bool) Next()
        {
            Debug.WriteLine($"Turn [{_index}] / [{_unitOrder.Count}]");
            var next = _unitOrder[_index];
            _index = (_unitOrder.Count <= (_index + 1)) ? 0 : (_index + 1);
            return (next, 0 == _index);
        }

        public void Add(int id, bool init)
        {
            if (init)
            {
                _unitOrder.Insert(_unitOrder.Count, id);
                Debug.WriteLine($"Add [{_unitOrder.Count}]");
            }
            else
            {
                _unitOrder.Insert(_index, id);
                Debug.WriteLine($"Add [{_index}]");
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
            else if (removeIndex == _unitOrder.Count)
            {
                _index = 0;
            }
            Debug.WriteLine($"Remove [{removeIndex}]");
        }
    }
}
