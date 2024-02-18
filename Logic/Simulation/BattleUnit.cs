using Logic.Models;
using Logic.Simulation.Abilities;
using Logic.Simulation.Actions;

namespace Logic.Simulation
{
    internal class BattleUnit(UnitModel model)
    {
        private static int _nextId = 0;

        public int Id { get; } = _nextId++;

        public Hex? Location { get; set; } = null;

        private readonly IList<IBattleAbility> _abilities = new List<IBattleAbility>();

        public UnitModel Model { get; } = model;

        public IEnumerable<IBattleAction> Act()
        {
            var ability = SelectAbility();
            return ability?.Use(this) ?? Enumerable.Empty<IBattleAction>();
        }

        private IBattleAbility? SelectAbility()
        {
            return _abilities.FirstOrDefault((IBattleAbility? ability) => ability?.CanUse(this) ?? false, null);
        }
    }
}
