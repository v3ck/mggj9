using Logic.Models;
using Logic.Simulation.Abilities;
using Logic.Simulation.Actions;

namespace Logic.Simulation
{
    internal class BattleUnit(UnitModel model)
    {
        public Hex? Location { get; set; } = null;

        private readonly IList<IBattleAbility> _abilities = new List<IBattleAbility>();

        public UnitModel Model { get; } = model;

        public IEnumerable<BattleActionBase> Act()
        {
            var ability = SelectAbility();
            return ability?.Use(this) ?? Enumerable.Empty<BattleActionBase>();
        }

        private IBattleAbility? SelectAbility()
        {
            return _abilities.FirstOrDefault((IBattleAbility? ability) => ability?.CanUse(this) ?? false, null);
        }
    }
}
