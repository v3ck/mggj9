using Logic.Events;
using Logic.Util;

namespace Logic
{
    /// <summary>
    /// The API to be exposed to Godot
    /// </summary>
    public interface IController
    {
        public event EventHandler<UnitMovedEventArgs>? UnitMoved;

        public event EventHandler<StatusChangedEventArgs>? StatusChanged;

        public event EventHandler<HealthChangedEventArgs>? HealthChanged;

        public event EventHandler<AbilityFiredEventArgs>? AbilityFired;

        public event EventHandler<ExistenceChangedEventArgs>? ExistenceChanged;

        public event EventHandler<RewardObtainedEventArgs>? RewardObtained;

        public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;

        public void StartBattle();

        public void TakeTurn();

        public void AddUnit(string code, int health, string faction, string[] abilities);

        public string[] GetUnitAbilities(string unitCode);

        public string[] GetAllCodexAbilities();

        public string[] GetCodexAbilities(string unitCode);

        public void AddCodexAbility(string abilityCode);

        public void MoveUnitAbilityUp(string unitCode, string abilityCode);

        public void MoveUnitAbilityDown(string unitCode, string abilityCode);

        public void EquipAbility(string unitCode, string abilityCode);

        public void UnequipAbility(string unitCode, string abilityCode);

        public void AddSpawn(int[] rounds, string[] unitCodes);

        public void AddAbility(string abilityCode, int maxCharge, int cost, int rarity);
    }
}
