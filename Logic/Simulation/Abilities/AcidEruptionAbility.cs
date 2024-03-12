using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class AcidEruptionAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "ACID_ERUPTION";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new AcidEruptionAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            return _user?.Location is not null;
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

            var targets = _gameModel.Grid.Hexes
                .Where(hex => hex != _user.Location)
                .Shuffle()
                .Take(45);

            List<IBattleAction> actions = [];
            foreach (var hex in targets)
            {
                actions.AddRange(GetActionsAtLocation(hex));
            }

            return actions;
        }


        private List<IBattleAction> GetActionsAtLocation(Hex location)
        {
            List<IBattleAction> actions = [];
            actions.Add(new AbilityAction()
            {
                BeginLocation = _user.Location,
                EndLocation = location,
                Ability = Code
            });

            var target = _state.Units.Values.Where(unit => location == unit.Location)
                .FirstOrDefault();
            if (target is null)
            {
                return actions;
            }

            var oldHealth = target.Health;
            actions.AddRange(target.Damage(4));

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

            return actions;
        }
    }
}
