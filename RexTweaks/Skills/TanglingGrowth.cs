using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Skills
{
    public class TanglingGrowth : TweakBase
    {
        public static float rootDamage;
        public static float healPercent;
        public static int pulseCount;
        public static float healthCost;
        public static float cooldown;
        public static float radius;

        public override string Name => ": Special :: Tangling Growth";

        public override string SkillToken => "special";

        public override string DescText => "<style=cIsHealth>" + d(healthCost) + " HP</style>. Fire a flower that <style=cIsDamage>roots</style> for <style=cIsDamage>" + d(rootDamage) + " damage</style>. <style=cIsHealing>Heals " + d((healPercent / (pulseCount - 1)) * 0.25f) + " HP for every target hit</style>, up to <style=cIsHealing>" + ((pulseCount - 1) * 4) + "</style> times.";

        public override void Init()
        {
            rootDamage = ConfigOption(2f, "Root Damage", "Decimal. Vanilla is 2");

            healPercent = ConfigOption(0.08f, "Heal Percent", "Decimal. Formula for Healing Per Pulse: (Heal Percent / (Pulse Count - 1)) * 0.25. Vanilla is 0.08");
            pulseCount = ConfigOption(5, "Pulse Count", "Vanilla is 5");

            healthCost = ConfigOption(0.25f, "Health Cost", "Decimal. Vanilla is 0.25");

            cooldown = ConfigOption(12f, "Cooldown", "Vanilla is 12");
            radius = ConfigOption(10f, "Radius", "Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.FireFlower2.OnEnter += FireFlower2_OnEnter;
            On.EntityStates.Treebot.FlowerProjectileHover.OnEnter += FlowerProjectileHover_OnEnter;
        }

        private void FlowerProjectileHover_OnEnter(On.EntityStates.Treebot.FlowerProjectileHover.orig_OnEnter orig, EntityStates.Treebot.FlowerProjectileHover self)
        {
            self.pulseCount = pulseCount;
            self.pulseRadius = radius;
            EntityStates.Treebot.FlowerProjectileHover.healthFractionYieldPerHit = healPercent;
            orig(self);
        }

        private void FireFlower2_OnEnter(On.EntityStates.FireFlower2.orig_OnEnter orig, EntityStates.FireFlower2 self)
        {
            EntityStates.FireFlower2.damageCoefficient = rootDamage;
            EntityStates.FireFlower2.healthCostFraction = healthCost;
            orig(self);
        }

        public static void Changes()
        {
            var tanglingGrowth = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyFireFlower2.asset").WaitForCompletion();
            tanglingGrowth.baseRechargeInterval = cooldown;
        }
    }
}