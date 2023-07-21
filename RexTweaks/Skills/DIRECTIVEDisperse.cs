using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Skills
{
    public class DIRECTIVEDisperse : TweakBase
    {
        public static float cooldown;
        public static int maxStock;

        public override string Name => ": Utility : Directive Disperse";

        public override string SkillToken => "utility";

        public override string DescText => "Fire a <style=cIsUtility>Sonic Boom</style> that <style=cIsDamage>Weakens</style> all enemies hit." +
                                           (maxStock > 1 ? " Can hold up to " + maxStock + "." : "");

        public override void Init()
        {
            cooldown = ConfigOption(4f, "Cooldown", "Vanilla is 5");
            maxStock = ConfigOption(2, "Max Stock", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var disperse = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodySonicBoom.asset").WaitForCompletion();
            disperse.baseRechargeInterval = cooldown;
            disperse.baseMaxStock = maxStock;
        }
    }
}