using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class RideTheLightningAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "RIDE_THE_LIGHTNING";

        private int _kills = 0;

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new RideTheLightningAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => (unit.Location - _user.Location).Magnitude <= 2)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Any();
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is not ExistenceAction existenceAction)
            {
                return;
            }

            if (existenceAction.Exists)
            {
                return;
            }

            if (!_gameModel.Units.TryGetValue(existenceAction.UnitCode, out var unitModel))
            {
                return;
            }

            if (unitModel.Faction == _user.Model.Faction)
            {
                return;
            }

            if (0 < _kills)
            {
                _kills -= 1;
            }
            else
            {
                _currentCharge += 1;
            }
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            return UseChain(_user.Location, [_user.Location]);
        }

        private List<IBattleAction> UseChain(Hex location, List<Hex> hitLocations)
        {
            if (8 < hitLocations.Count)
            {
                return [];
            }

            var target = _gameModel.Grid.AtDistance(location, 1)
                .Where(hex => !hitLocations.Contains(hex))
                .Shuffle()
                .MaxBy(hex => _state.Units.Values
                    .Where(unit => IsTargetValid(unit, hex))
                    .Count());

            if (target is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            foreach (var unit in _state.Units.Values)
            {
                actions.AddRange(HitUnit(unit, target));
            }

            hitLocations.Add(target);
            actions.AddRange(UseChain(target, hitLocations));

            if (!actions.Any(action => action is MoveAction) &&
                !_state.Units.Values.Any(unit => unit.Location == target))
            {
                _user.Location = target;
                actions.Add(new MoveAction()
                {
                    UnitId = _user.Id,
                    FromLocation = _user.Location,
                    ToLocation = target,
                    IsTeleport = false
                });
            }

            return actions;
        }

        private List<IBattleAction> HitUnit(BattleUnit unit, Hex location)
        {
            if (!IsTargetValid(unit, location))
            {
                return [];
            }

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = location,
                EndLocation = unit.Location,
                Ability = Code
            });

            var oldHealth = unit.Health;
            actions.AddRange(unit.Damage(1));

            actions.Add(new HealthAction()
            {
                UnitId = unit.Id,
                Location = unit.Location,
                Amount = unit.Health,
                PreviousAmount = oldHealth,
                SourceUnitId = _user.Id
            });

            if (unit.Health <= 0)
            {
                actions.Add(new ExistenceAction()
                {
                    UnitId = unit.Id,
                    UnitCode = unit.Model.Code,
                    Location = location,
                    Exists = false
                });

                _state.Units.Remove(unit.Id);
                _kills += 1;
            }

            return actions;
        }

        private bool IsTargetValid(BattleUnit unit, Hex location)
        {
            if (unit?.Location is null)
            {
                return false;
            }

            if (1 < (unit.Location - location).Magnitude)
            {
                return false;
            }

            if (unit.Model.Faction == _user.Model.Faction)
            {
                return false;
            }

            return true;
        }
    }
}
