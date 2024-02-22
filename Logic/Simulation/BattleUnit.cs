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

        private int _health;
        public int Health
        {
            get => _health;
            set => _health = value;
        }

        private int _abilityPoints = 1;
        public int AbilityPoints
        {
            get => _abilityPoints;
            set => _abilityPoints = value;
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
            _health = model.MaxHealth;

            foreach (var abilityCode in model.Abilities)
            {
                TryAddAbility(abilityCode);
            }
        }

        public List<IBattleAction> Act()
        {
            var ability = SelectAbility();
            List<IBattleAction> actions = [];
            if (ability is not null && ability.CanPay())
            {
                actions = ability.Use();
            }
            _abilityPoints++;
            return actions;
        }

        private IBattleAbility? SelectAbility()
        {
            return _abilities.FirstOrDefault(ability => ability?.CanUse() ?? false, null);
        }

        private void TryAddAbility(string abilityCode)
        {
            if (!_gameModel.Abilities.TryGetValue(abilityCode, out var abilityModel))
            {
                return;
            }

            var ability = AbilityFactory.Create(abilityModel, this, _state, _gameModel);
            if (ability is null)
            {
                return;
            }

            _abilities.Add(ability);
        }
    }
}
