using Logic.Models;
using Logic.Simulation.Abilities;
using Logic.Simulation.Actions;

namespace Logic.Simulation
{
    internal class BattleUnit
    {
        private static int _nextId = 0;
        private readonly int _id = _nextId++;
        public int Id => _id;

        private Hex? _location = null;
        public Hex? Location
        {
            get => _location;
            set => _location = value;
        }

        private readonly List<IBattleAbility> _abilities = [];

        private readonly UnitModel _model;
        public UnitModel Model => _model;

        private readonly BattleState _state;
        private readonly GameModel _gameModel;

        public BattleUnit(
            UnitModel model,
            BattleState state, 
            GameModel gameModel)
        {
            _model = model;
            _state = state;
            _gameModel = gameModel;
            _abilities.AddRange(
                model.Abilities.Select(abilityCode => AbilityFactory.Create(abilityCode, this, _state, _gameModel))
                .Where(ability => ability is not null)
                .Select(ability => ability!));
        }

        public IEnumerable<IBattleAction> Act()
        {
            var ability = SelectAbility();
            return ability?.Use() ?? Enumerable.Empty<IBattleAction>();
        }

        private IBattleAbility? SelectAbility()
        {
            return _abilities.FirstOrDefault((IBattleAbility? ability) => ability?.CanUse() ?? false, null);
        }
    }
}
