using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Skills
{
    public class SeedBarrage : TweakBase
    {
        public static float damage;
        public static float cooldown;
        public static float healthCost;

        public override string Name => ": Secondary :: Seed Barrage";

        public override string SkillToken => "secondary";

        public override string DescText => "<style=cIsHealth>" + d(healthCost) + " HP</style>. Launch a mortar into the sky for <style=cIsDamage>" + d(damage) + " damage</style>.";

        public override void Init()
        {
            damage = ConfigOption(3.7f, "Damage", "Decimal. Vanilla is 4.5");
            cooldown = ConfigOption(0.5f, "Cooldown", "Vanilla is 0.5");
            healthCost = ConfigOption(0.15f, "Health Cost", "Decimal. Vanilla is 0.15");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Treebot.Weapon.FireMortar2.OnEnter += FireMortar2_OnEnter;
        }

        private void FireMortar2_OnEnter(On.EntityStates.Treebot.Weapon.FireMortar2.orig_OnEnter orig, EntityStates.Treebot.Weapon.FireMortar2 self)
        {
            EntityStates.Treebot.Weapon.FireMortar2.damageCoefficient = damage;
            EntityStates.Treebot.Weapon.FireMortar2.healthCostFraction = healthCost;
            orig(self);
        }

        public static void Changes()
        {
            var seed = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyAimMortar2.asset").WaitForCompletion();
            seed.baseRechargeInterval = cooldown;
        }
    }
}