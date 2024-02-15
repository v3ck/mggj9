namespace Logic.Models
{
    internal class UnitModel
    {
        private static int _nextId = 0;

        public int Id { get; } = _nextId++;

        public required bool IsEnemy { get; set; }

        public IList<string> Abilities { get; } = new List<string>();
    }
}
