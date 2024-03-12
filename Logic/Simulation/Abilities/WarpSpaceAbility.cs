using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class WarpSpaceAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "WARP_SPACE";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new WarpSpaceAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user.Location is null)
            {
                return false;
            }

            return _gameModel.Grid.WithinDistance(_user.Location, 0, 2)
                .Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is not RoundAction)
            {
                return;
            }

            _currentCharge++;
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var targetLocation = _gameModel.Grid.WithinDistance(_user.Location, 0, 2)
                .Where(IsTargetValid)
                .MaxBy(hex => _state.Units.Values
                    .Where(unit => unit.Location is not null)
                    .Where(unit => unit.Model.Faction != _user.Model.Faction)
                    .Where(unit => 1 == (hex - unit.Location).Magnitude)
                    .Count());

            if (targetLocation is null)
            {
                return [];
            }

            var targetUnits = _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => 1 == (targetLocation - unit.Location).Magnitude);

            List<IBattleAction> actions = [];

            var oldLocation = _user.Location;
            _user.Location = targetLocation;
            actions.Add(new MoveAction()
            {
                UnitId = _user.Id,
                FromLocation = oldLocation,
                ToLocation = targetLocation,
                IsTeleport = true
            });

            _user.ShieldCount += 1;
            actions.Add(new StatusAction()
            {
                UnitId = _user.Id,
                Location = targetLocation,
                Status = "SHIELD",
                Active = true

            });

            foreach (var unit in targetUnits)
            {
                actions.AddRange(StunUnit(unit, targetLocation));
            }

            return actions;
        }

        private List<IBattleAction> StunUnit(BattleUnit unit, Hex targetLocation)
        {
            if (unit?.Location is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = targetLocation,
                EndLocation = unit.Location,
                Ability = Code
            });

            unit.StunTurns += 1;

            actions.Add(new StatusAction()
            {
                UnitId = unit.Id,
                Location = unit.Location,
                Status = "STUN",
                Active = true
            });

            return actions;
        }

        private bool IsTargetValid(Hex hex)
        {
            if (_user.Location is null)
            {
                return false;
            }

            if (_state.Units.Values.Any(unit => unit.Location == hex))
            {
                return false;
            }

            return _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => 1 == (hex - unit.Location).Magnitude)
                .Any();
        }
    }
}
