using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class FireballAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "FIREBALL";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new FireballAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _gameModel.Grid.Hexes.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            // void
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var target = _gameModel.Grid.Hexes
                .Where(IsTargetValid)
                .Random();

            if (target is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = _user.Location,
                EndLocation = target,
                Ability = Code
            });

            foreach (var hex in _gameModel.Grid.WithinDistance(target, 0, 1))
            {
                actions.AddRange(GetActionsAtLocation(hex));
            }

            return actions;
        }

        private bool IsTargetValid(Hex hex)
        {
            if (_user.Location is null)
            {
                return false;
            }

            var distance = (hex - _user.Location).Magnitude;
            if ((distance < 2) || (3 < distance))
            {
                return false;
            }

            var candidates = _gameModel.Grid.WithinDistance(hex, 0, 1);
            if (_state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction == _user.Model.Faction)
                .Where(unit => (hex - unit.Location).Magnitude <= 1)
                .Any())
            {
                return false;
            }

            return _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => (hex - unit.Location).Magnitude <= 1)
                .Any();
        }

        private List<IBattleAction> GetActionsAtLocation(Hex location)
        {
            var target = _state.Units.Values.Where(unit => location == unit.Location)
                .FirstOrDefault();
            if (target is null)
            {
                return [];
            }

            var oldHealth = target.Health;
            target.Health -= 3;

            List<IBattleAction> actions = [];
            actions.Add(new HealthAction()
            {
                UnitId = target.Id,
                Location = location,
                Amount = target.Health,
                PreviousAmount = oldHealth
            });

            if (target.Health <= 0)
            {
                actions.Add(new ExistenceAction()
                {
                    UnitId = target.Id,
                    UnitCode = target.Model.Code,
                    Location = location,
                    Exists = false
                });

                _state.Units.Remove(target.Id);
            }

            return actions;
        }
    }
}