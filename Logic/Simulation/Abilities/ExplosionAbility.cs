using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class ExplosionAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "EXPLOSION";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ExplosionAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _gameModel.Grid.Hexes.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            if (TryResetCharge(action))
            {
                return;
            }

            TryIncrementCharge(action);
        }

        private bool TryResetCharge(IBattleAction action)
        {
            if (action is not HealthAction healthAction)
            {
                return false;
            }

            if (healthAction.SourceUnitId != _user.Id)
            {
                return false;
            }

            if (0 <= (healthAction.Amount - healthAction.PreviousAmount))
            {
                return false;
            }

            _currentCharge = 0;
            return true;
        }

        private void TryIncrementCharge(IBattleAction action)
        {
            if (action is not RoundAction)
            {
                return;
            }

            _currentCharge += 1;
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var target = _gameModel.Grid.Hexes
                .Where(IsTargetValid)
                .Shuffle()
                .MaxBy(CountEnemiesHit);

            if (target is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            foreach (var hex in _gameModel.Grid.WithinDistance(target, 0, 2))
            {
                actions.AddRange(GetActionsAtLocation(hex, target));
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
            if ((distance < 3) || (5 < distance))
            {
                return false;
            }

            if (_state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction == _user.Model.Faction)
                .Where(unit => (hex - unit.Location).Magnitude <= 2)
                .Any())
            {
                return false;
            }

            return 5 <= _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Where(unit => (hex - unit.Location).Magnitude <= 2)
                .Count();
        }

        private int CountEnemiesHit(Hex hex)
        {
            return _state.Units.Values
                .Where(unit => unit.Location is not null)
                .Where(unit => (unit.Location - hex).Magnitude <= 2)
                .Where(unit => unit.Model.Faction != _user.Model.Faction)
                .Count();
        }

        private List<IBattleAction> GetActionsAtLocation(Hex location, Hex targetLocation)
        {
            var target = _state.Units.Values.Where(unit => location == unit.Location)
                .FirstOrDefault();
            if (target is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = targetLocation,
                EndLocation = location,
                Ability = Code
            });

            var oldHealth = target.Health;
            actions.AddRange(target.Damage(10));

            actions.Add(new HealthAction()
            {
                UnitId = target.Id,
                Location = location,
                Amount = target.Health,
                PreviousAmount = oldHealth,
                SourceUnitId = _user.Id
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

            _user.StunTurns = Math.Max(_user.StunTurns, 3);
            actions.Add(new StatusAction()
            {
                UnitId = _user.Id,
                Location = _user.Location,
                Status = "STUN",
                Active = true
            });

            return actions;
        }
    }
}
