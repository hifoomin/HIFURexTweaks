using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Skills
{
    public class DIRECTIVEInject : TweakBase
    {
        public static float damage;
        public static float procCoefficient;
        public static int syringes;
        public static float durationPer;
        public static float healPercent;

        public override string Name => ": Primary : DIRECTIVE: Inject";

        public override string SkillToken => "primary";

        public override string DescText => "Fire " + syringes + " syringes for <style=cIsDamage>" + syringes + "x" + d(damage) + " damage</style>. The last syringe <style=cIsDamage>Weakens</style> and <style=cIsHealing>heals for " + d(healPercent) + " of damage dealt</style>.";

        public override void Init()
        {
            damage = ConfigOption(0.8f, "Damage", "Decimal. Vanilla is 0.8");
            syringes = ConfigOption(3, "Syringe Count", "Vanilla is 3");
            durationPer = ConfigOption(0.2f, "Duration Per Syringe", "Vanilla is 0.2");
            healPercent = ConfigOption(0.45f, "Healing Percentage", "Decimal. Vanilla is 0.6");
            procCoefficient = ConfigOption(0.5f, "Proc Coefficient", "Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Treebot.Weapon.FireSyringe.OnEnter += FireSyringe_OnEnter;
        }

        private void FireSyringe_OnEnter(On.EntityStates.Treebot.Weapon.FireSyringe.orig_OnEnter orig, EntityStates.Treebot.Weapon.FireSyringe self)
        {
            EntityStates.Treebot.Weapon.FireSyringe.damageCoefficient = damage;
            EntityStates.Treebot.Weapon.FireSyringe.projectileCount = syringes;
            EntityStates.Treebot.Weapon.FireSyringe.baseFireDuration = durationPer;
            EntityStates.Treebot.Weapon.FireSyringe.baseDuration = syringes * durationPer;

            orig(self);
        }

        public static void Changes()
        {
            var syringe = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/SyringeProjectile.prefab").WaitForCompletion();

            var projectileController = syringe.GetComponent<ProjectileController>();
            projectileController.procCoefficient = procCoefficient;

            var projectileTargetComponent = syringe.AddComponent<ProjectileTargetComponent>();
            projectileTargetComponent.enabled = true;

            var projectileSteerTowardTarget = syringe.AddComponent<ProjectileSteerTowardTarget>();
            projectileSteerTowardTarget.enabled = true;
            projectileSteerTowardTarget.rotationSpeed = 1080f;

            var projectileDirectionalTargetFinder = syringe.AddComponent<ProjectileDirectionalTargetFinder>();
            projectileDirectionalTargetFinder.enabled = true;
            projectileDirectionalTargetFinder.lookRange = 20f;
            projectileDirectionalTargetFinder.lookCone = 10f;
            projectileDirectionalTargetFinder.targetSearchInterval = 0.1f;
            projectileDirectionalTargetFinder.onlySearchIfNoTarget = true;
            projectileDirectionalTargetFinder.allowTargetLoss = false;
            projectileDirectionalTargetFinder.testLoS = false;
            projectileDirectionalTargetFinder.ignoreAir = false;
            projectileDirectionalTargetFinder.flierAltitudeTolerance = Mathf.Infinity;

            var syringeHealing = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/SyringeProjectileHealing.prefab").WaitForCompletion();
            syringeHealing.GetComponent<ProjectileHealOwnerOnDamageInflicted>().fractionOfDamage = healPercent;

            var projectileController2 = syringeHealing.GetComponent<ProjectileController>();
            projectileController2.procCoefficient = procCoefficient;

            var projectileTargetComponent2 = syringeHealing.AddComponent<ProjectileTargetComponent>();
            projectileTargetComponent2.enabled = true;

            var projectileSteerTowardTarget2 = syringeHealing.AddComponent<ProjectileSteerTowardTarget>();
            projectileSteerTowardTarget2.enabled = true;
            projectileSteerTowardTarget2.rotationSpeed = 1080f;

            var projectileDirectionalTargetFinder2 = syringeHealing.AddComponent<ProjectileDirectionalTargetFinder>();
            projectileDirectionalTargetFinder2.enabled = true;
            projectileDirectionalTargetFinder2.lookRange = 20f;
            projectileDirectionalTargetFinder2.lookCone = 10f;
            projectileDirectionalTargetFinder2.targetSearchInterval = 0.1f;
            projectileDirectionalTargetFinder2.onlySearchIfNoTarget = true;
            projectileDirectionalTargetFinder2.allowTargetLoss = false;
            projectileDirectionalTargetFinder2.testLoS = false;
            projectileDirectionalTargetFinder2.ignoreAir = false;
            projectileDirectionalTargetFinder2.flierAltitudeTolerance = Mathf.Infinity;
        }
    }
}