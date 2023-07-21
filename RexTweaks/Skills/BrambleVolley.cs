using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Skills
{
    public class BrambleVolley : TweakBase
    {
        public static float damage;
        public static float healPercent;
        public static float healthCost;
        public static float cooldown;
        public static float procCoefficient;

        public override string Name => ": Utility :: Bramble Volley";

        public override string SkillToken => "utility_alt1";

        public override string DescText => "<style=cIsHealth>" + d(healthCost) + " HP</style>. Fire a <style=cIsUtility>Sonic Boom</style> that <style=cIsDamage>damages</style> enemies for <style=cIsDamage>" + d(damage) + " damage</style>. <style=cIsHealing>Heals " + d(healPercent) + " for every target hit</style>.";

        public override void Init()
        {
            damage = ConfigOption(5f, "Damage", "Decimal. Vanilla is 5.5");
            healPercent = ConfigOption(0.08f, "Heal Percent", "Decimal. Vanilla is 0.1");
            healthCost = ConfigOption(0.2f, "Health Cost", "Decimal. Vanilla is 0.2");
            cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 5");
            procCoefficient = ConfigOption(0.5f, "Proc Coefficient", "Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Treebot.Weapon.FirePlantSonicBoom.OnEnter += FirePlantSonicBoom_OnEnter;
        }

        private void FirePlantSonicBoom_OnEnter(On.EntityStates.Treebot.Weapon.FirePlantSonicBoom.orig_OnEnter orig, EntityStates.Treebot.Weapon.FirePlantSonicBoom self)
        {
            EntityStates.Treebot.Weapon.FirePlantSonicBoom.damageCoefficient = damage;
            EntityStates.Treebot.Weapon.FirePlantSonicBoom.healthFractionPerHit = healPercent;
            EntityStates.Treebot.Weapon.FirePlantSonicBoom.healthCostFraction = healthCost;
            EntityStates.Treebot.Weapon.FirePlantSonicBoom.procCoefficient = procCoefficient;
            orig(self);
        }

        public static void Changes()
        {
            var brambleVolley = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyPlantSonicBoom.asset").WaitForCompletion();
            brambleVolley.baseRechargeInterval = cooldown;
        }
    }
}