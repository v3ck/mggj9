using Logic.Models;
using Logic.Simulation.Abilities;
using Logic.Simulation.Actions;
using Logic.Util;
using System;
using System.Diagnostics;

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

            bool isStunned = (0 < _stunTurns);
            var ability = GetAbility(isStunned);
            if (ability is not null)
            {
                actions.AddRange(ability.Use());
            }

            var stunAction = CheckStun();
            if (stunAction is not null)
            {
                actions.Add(stunAction);
            }

            _abilityPoints++;
            actions.Add(new TurnAction()
            {
                UnitId = Id,
                UnitCode = _model.Code,
                AbilityPoints = _abilityPoints,
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

            Debug.WriteLine($"[{_model.Code}] used [{ability.Code}]");
            return ability;
        }

        public void ChargeAbilities(IBattleAction action)
        {
            foreach (var ability in _abilities.Values)
            {
                ability.TryCharge(action);
            }
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
    }
}
