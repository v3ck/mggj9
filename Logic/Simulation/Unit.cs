using Logic.Models;

namespace Logic.Simulation
{
    internal class Unit(UnitModel model)
    {
        public Hex? Location { get; set; } = null;

        private readonly IList<IAbility> _abilities = new List<IAbility>();

        public UnitModel Model { get; } = model;

        public IEnumerable<BattleAction> Act()
        {
            var ability = SelectAbility();
            if (null == ability)
            {
                return Enumerable.Empty<BattleAction>();
            }

            // TODO
            return Enumerable.Empty<BattleAction>();
        }

        private IAbility? SelectAbility()
        {
            // TODO
            return _abilities.FirstOrDefault((IAbility? ability) => true, null);
        }
    }
}
