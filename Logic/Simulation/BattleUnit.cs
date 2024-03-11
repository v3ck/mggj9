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

        private int _stunTurns = 0;
        public int StunTurns
        {
            get => _stunTurns;
            set => _stunTurns = value;
        }

        private int _timeTurns = 0;
        public int TimeTurns
        {
            get => _timeTurns;
            set => _timeTurns = value;
        }

        //private readonly List<IBattleAbility> _abilities = [];
        private readonly Dictionary<string, IBattleAbility> _abilities = new();

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
            RefreshAbilities();
        }

        public void RefreshAbilities()
        {
            foreach (var abilityCode in _abilities.Keys)
            {
                TryRemoveAbility(abilityCode);
            }

            foreach (var abilityCode in _model.Abilities)
            {
                TryAddAbility(abilityCode);
            }
        }

        public List<IBattleAction> Act()
        {
            List<IBattleAction> actions = [];

            var timeAction = CheckTime();
            if (timeAction is not null)
            {
                actions.Add(timeAction);
            }

            bool isStunned = (0 < _stunTurns);
            var stunAction = CheckStun();
            if (stunAction is not null)
            {
                actions.Add(stunAction);
            }

            var ability = GetAbility(isStunned);
            if (ability is not null)
            {
                actions.AddRange(ability.Use());
            }

            GainAbilityPoints(isStunned);

            actions.Add(new AbilityPointAction()
            {
                UnitId = _id,
                Amount = _abilityPoints
            });

            actions.Add(new TurnAction()
            {
                UnitId = Id,
                UnitCode = _model.Code,
                AbilityCode = ability?.Code
            });

            return actions;
        }

        private IBattleAbility? GetAbility(bool isStunned)
        {
            if (isStunned)
            {
                return null;
            }

            var ability = SelectAbility();
            if (ability is null)
            {
                return null;
            }
            
            if (!ability.CanPay())
            {
                return null;
            }

            //Debug.WriteLine($"[{_model.Code}] used [{ability.Code}]");
            return ability;
        }

        private void GainAbilityPoints(bool isStunned)
        {
            if (isStunned)
            {
                return;
            }

            _abilityPoints += 1;
        }

        public void ChargeAbilities(IBattleAction action)
        {
            foreach (var ability in _abilities.Values)
            {
                ability.TryCharge(action);
            }
        }

        public void Damage(int amount)
        {
            _health = Math.Max(0, _health - amount);
        }

        public void Heal(int amount)
        {
            _health = Math.Min(_model.MaxHealth, _health + amount);
        }

        private IBattleAbility? SelectAbility()
        {
            var code = _model.Abilities
                .Where(_abilities.ContainsKey)
                .Where(abilityCode => _abilities[abilityCode].CanUse())
                .FirstOrDefault();
            if (code is null)
            {
                return null;
            }

            return _abilities[code];
        }

        private void TryAddAbility(string abilityCode)
        {
            if (_abilities.ContainsKey(abilityCode))
            {
                return;
            }

            if (!_gameModel.Abilities.TryGetValue(abilityCode, out var abilityModel))
            {
                return;
            }

            var ability = AbilityFactory.Create(abilityModel, this, _state, _gameModel);
            if (ability is null)
            {
                return;
            }

            //Debug.WriteLine($"Added ability: [{ability?.Code ?? "None"}]");

            _abilities.Add(abilityCode, ability);
        }

        private void TryRemoveAbility(string abilityCode)
        {
            if (_model.Abilities.Contains(abilityCode))
            {
                return;
            }

            _abilities.Remove(abilityCode);
        }

        private IBattleAction? CheckStun()
        {
            if (0 == _stunTurns)
            {
                return null;
            }

            _stunTurns -= 1;
            if (0 < _stunTurns)
            {
                return null;
            }

            if (Location is null)
            {
                return null;
            }

            return new StatusAction()
            {
                UnitId = Id,
                Location = Location,
                Status = "STUN",
                Active = false
            };
        }

        private IBattleAction? CheckTime()
        {
            if (0 == _timeTurns)
            {
                return null;
            }

            _timeTurns -= 1;
            if (0 < _timeTurns)
            {
                return null;
            }

            if (_location is null)
            {
                return null;
            }

            return new StatusAction()
            {
                Active = false,
                UnitId = _id,
                Location = _location,
                Status = "TIME"
            };
        }
    }
}
