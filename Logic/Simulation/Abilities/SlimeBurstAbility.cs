using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class SlimeBurstAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "SLIME_BURST";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SlimeBurstAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user.Location is null)
            {
                return false;
            }

            return _gameModel.Grid.WithinDistance(_user.Location, 0, 2)
                .Any(hex => _state.Units.Values
                    .Where(unit => unit.Location is not null)
                    .Where(unit => unit.Model.Faction != _user.Model.Faction)
                    .Where(unit => hex == unit.Location)
                    .Any());
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

            var targetLocation = _gameModel.Grid.WithinDistance(_user.Location, 0, 1)
                .Where(hex => !_state.Units.Values.Any(unit => hex == unit.Location))
                .Shuffle()
                .MaxBy(hex => _state.Units.Values
                    .Where(unit => unit.Location is not null)
                    .Where(unit => unit.Model.Faction != _user.Model.Faction)
                    .Where(unit => 1 == (hex - unit.Location).Magnitude)
                    .Count());

            if (targetLocation is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            var oldLocation = _user.Location;
            if (oldLocation != targetLocation)
            {
                _user.Location = targetLocation;
                actions.Add(new MoveAction()
                {
                    UnitId = _user.Id,
                    FromLocation = oldLocation,
                    ToLocation = targetLocation,
                    IsTeleport = false
                });
            }

            var targetUnits = _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => 1 == (targetLocation - unit.Location).Magnitude);

            foreach (var unit in targetUnits)
            {
                actions.AddRange(HitTarget(unit, oldLocation));
            }

            return actions;
        }

        private List<IBattleAction> HitTarget(BattleUnit targetUnit, Hex sourceLocation)
        {
            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = sourceLocation,
                EndLocation = targetUnit.Location,
                Ability = Code
            });

            var oldHealth = targetUnit.Health;
            targetUnit.Damage(3);

            actions.Add(new HealthAction()
            {
                UnitId = targetUnit.Id,
                Location = targetUnit.Location,
                Amount = targetUnit.Health,
                PreviousAmount = oldHealth,
                SourceUnitId = _user.Id
            });

            if (targetUnit.Health <= 0)
            {
                actions.Add(new ExistenceAction()
                {
                    UnitId = targetUnit.Id,
                    UnitCode = targetUnit.Model.Code,
                    Location = targetUnit.Location,
                    Exists = false
                });

                _state.Units.Remove(targetUnit.Id);
            }

            return actions;
        }
    }
}
