using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;
using System.Diagnostics;

namespace Logic.Simulation.Abilities
{
    internal class BlessingAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "BLESSING";

        private bool _usedThisRound = false;

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new BlessingAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _state.Units.Values.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is RoundAction)
            {
                _usedThisRound = false;
                return;
            }

            if (_usedThisRound)
            {
                return;
            }

            if (action is not HealthAction healthAction)
            {
                return;
            }

            if (healthAction.SourceUnitId != _user.Id)
            {
                return;
            }

            if (healthAction.UnitId == _user.Id)
            {
                return;
            }

            _currentCharge += Math.Max(0, healthAction.Amount - healthAction.PreviousAmount);
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var target = _state.Units.Values
                .Where(IsTargetValid)
                .Shuffle()
                .MaxBy(GetMissingHealth);

            if (target?.Location is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = _user.Location,
                EndLocation = target.Location,
                Ability = Code
            });

            var oldHealth = target.Health;
            target.Heal(6);

            if (oldHealth != target.Health)
            {
                actions.Add(new HealthAction()
                {
                    UnitId = target.Id,
                    Location = target.Location,
                    Amount = target.Health,
                    PreviousAmount = oldHealth,
                    SourceUnitId = _user.Id
                });
            }

            _usedThisRound = true;

            return actions;
        }

        private bool IsTargetValid(BattleUnit unit)
        {
            if (_user.Location is null)
            {
                return false;
            }

            if (unit.Location is null)
            {
                return false;
            }

            if (unit.Model.Faction != _user.Model.Faction)
            {
                return false;
            }

            return (unit.Health < (1 + unit.Model.MaxHealth) / 2);
        }

        private int GetMissingHealth(BattleUnit unit)
        {
            return unit.Model.MaxHealth - unit.Health;
        }
    }
}
