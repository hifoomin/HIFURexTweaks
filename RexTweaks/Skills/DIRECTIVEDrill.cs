using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Skills
{
    public class DIRECTIVEDrill : TweakBase
    {
        public static float damage;
        public static float procCoefficient;
        public static float cooldown;
        public static float duration;

        public override string Name => ": Secondary : DIRECTIVE: Drill";

        public override string SkillToken => "secondary_alt1";

        public override string DescText => "Launch a series of seed bullets into the sky, raining down for <style=cIsDamage>" + d((16f * damage) / cooldown) + " damage per second</style>.";

        public override void Init()
        {
            damage = ConfigOption(1.6f, "Damage", "Decimal. Vanilla is 0.9");
            procCoefficient = ConfigOption(1.0f, "Proc Coefficient", "Vanilla is 0.7");
            cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 6");
            duration = ConfigOption(3f, "Duration", "Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var drill = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/TreebotMortarRain").GetComponent<ProjectileDotZone>();
            drill.damageCoefficient = damage / 4.5f;
            drill.lifetime = duration;
            drill.overlapProcCoefficient = procCoefficient;

            var drillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyAimMortarRain.asset").WaitForCompletion();
            drillDef.baseRechargeInterval = cooldown;
        }
    }
}