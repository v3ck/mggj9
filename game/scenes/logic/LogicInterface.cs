using Godot;
using Logic.Util;
using System.Linq;

namespace Game
{
    /// <summary>
    /// Wraps Logic module to be used by Godot
    /// </summary>
    public partial class LogicInterface : Node
    {
        [Signal]
        public delegate void UnitMovedEventHandler(int id, Vector2I fromLocation, Vector2I toLocation, bool isTeleport);

        [Signal]
        public delegate void StatusChangedEventHandler(int id, Vector2I location, string status, bool isActive);

        [Signal]
        public delegate void HealthChangedEventHandler(int id, Vector2I location, int health);

        [Signal]
        public delegate void AbilityFiredEventHandler(Vector2I fromLocation, Vector2I toLocation, string ability);

        [Signal]
        public delegate void ExistenceChangedEventHandler(int id, Vector2I location, string code, bool exists);

        [Signal]
        public delegate void RewardObtainedEventHandler(string[] abilityCodes);

        [Signal]
        public delegate void ScoreChangedEventHandler(int amount);

        [Signal]
        public delegate void RoundChangedEventHandler(int round);

        [Signal]
        public delegate void AbilityPointsChangedEventHandler(int id, int amount);

        private readonly Logic.IController _controller = Logic.Api.CreateController();

        public override void _Ready()
        {
            _controller.UnitMoved += Controller_UnitMoved;
            _controller.StatusChanged += Controller_StatusChanged;
            _controller.HealthChanged += Controller_HealthChanged;
            _controller.AbilityFired += Controller_AbilityFired;
            _controller.ExistenceChanged += Controller_ExistenceChanged;
            _controller.RewardObtained += Controller_RewardObtained;
            _controller.ScoreChanged += Controller_ScoreChanged;
            _controller.RoundChanged += Controller_RoundChanged;
            _controller.AbilityPointsChanged += Controller_AbilityPointsChanged;
        }

        public void AddUnit(Resource unitResource)
        {
            var abilities = unitResource
                .Get("default_abilities")
                .AsGodotArray<Resource>()
                .AsEnumerable()
                .Select(abilityResource => abilityResource.Get("code").AsString())
                .ToArray();

            _controller.AddUnit(
                unitResource.Get("code").AsString(),
                unitResource.Get("default_health").AsInt32(),
                unitResource.Get("faction").AsString(),
                abilities);
        }

        public void AddSpawn(Resource spawnResource)
        {
            var unitCode = spawnResource
                .Get("unit_resource")
                .AsGodotObject()
                .Get("code")
                .AsString();

            _controller.AddSpawn(
                spawnResource.Get("begin_round").AsInt32(),
                spawnResource.Get("end_round").AsInt32(),
                unitCode,
                spawnResource.Get("rate").AsDouble(),
                spawnResource.Get("volatility").AsDouble());
        }

        public void AddAbility(Resource abilityResource)
        {
            _controller.AddAbility(
                abilityResource.Get("code").AsString(),
                abilityResource.Get("max_charge").AsInt32(),
                abilityResource.Get("default_cost").AsInt32(),
                abilityResource.Get("rarity").AsInt32());
        }

        public void TakeTurn()
        {
            _controller.TakeTurn();
        }

        public void StartBattle()
        {
            _controller.StartBattle();
        }

        public string[] GetUnitAbilities(string unitCode)
        {
            return _controller.GetUnitAbilities(unitCode);
        }

        public string[] GetAllCodexAbilities()
        {
            return _controller.GetAllCodexAbilities();
        }

        public string[] GetCodexAbilities(string unitCode)
        {
            return _controller.GetCodexAbilities(unitCode);
        }

        public void AddCodexAbility(string abilityCode)
        {
            _controller.AddCodexAbility(abilityCode);
        }

        public void MoveUnitAbilityUp(string unitCode, string abilityCode)
        {
            _controller.MoveUnitAbilityUp(unitCode, abilityCode);
        }

        public void MoveUnitAbilityDown(string unitCode, string abilityCode)
        {
            _controller.MoveUnitAbilityDown(unitCode, abilityCode);
        }

        public void EquipAbility(string unitCode, string abilityCode)
        {
            _controller.EquipAbility(unitCode, abilityCode);
        }

        public void UnequipAbility(string unitCode, string abilityCode)
        {
            _controller.UnequipAbility(unitCode, abilityCode);
        }

        private void Controller_UnitMoved(object sender, Logic.Events.UnitMovedEventArgs e)
        {
            EmitSignal(
                SignalName.UnitMoved,
                e.UnitId,
                IntVector2ToVector2I(e.FromLocation),
                IntVector2ToVector2I(e.ToLocation),
                e.IsTeleport);
        }

        private void Controller_StatusChanged(object sender, Logic.Events.StatusChangedEventArgs e)
        {
            EmitSignal(
                SignalName.StatusChanged,
                e.UnitId,
                IntVector2ToVector2I(e.Location),
                e.Status,
                e.Active);
        }

        private void Controller_HealthChanged(object sender, Logic.Events.HealthChangedEventArgs e)
        {
            EmitSignal(
                SignalName.HealthChanged,
                e.UnitId,
                IntVector2ToVector2I(e.Location),
                e.Amount);
        }

        private void Controller_AbilityFired(object sender, Logic.Events.AbilityFiredEventArgs e)
        {
            EmitSignal(
                SignalName.AbilityFired,
                IntVector2ToVector2I(e.FromLocation),
                IntVector2ToVector2I(e.ToLocation),
                e.Ability);
        }

        private void Controller_ExistenceChanged(object sender, Logic.Events.ExistenceChangedEventArgs e)
        {
            EmitSignal(
                SignalName.ExistenceChanged,
                e.UnitId,
                IntVector2ToVector2I(e.Location),
                e.UnitCode,
                e.Exists);
        }

        private void Controller_RewardObtained(object sender, Logic.Events.RewardObtainedEventArgs e)
        {
            EmitSignal(
                SignalName.RewardObtained,
                e.Abilities);
        }

        private void Controller_ScoreChanged(object sender, Logic.Events.ScoreChangedEventArgs e)
        {
            EmitSignal(
                SignalName.ScoreChanged,
                e.Amount);
        }

        private void Controller_RoundChanged(object sender, Logic.Events.RoundChangedEventArgs e)
        {
            EmitSignal(
                SignalName.RoundChanged,
                e.Round);
        }

        private void Controller_AbilityPointsChanged(object sender, Logic.Events.AbilityPointsChangedEventArgs e)
        {
            EmitSignal(
                SignalName.AbilityPointsChanged,
                e.UnitId,
                e.Amount);
        }

        private static Vector2I IntVector2ToVector2I(IntVector2 iv)
        {
            return new Vector2I(iv.X, iv.Y);
        }
    }
}
