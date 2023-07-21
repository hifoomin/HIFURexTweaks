using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFURexTweaks.Misc
{
    internal class HurtBox : MiscBase
    {
        public static float SizeMultiplier;
        public override string Name => ": Misc : Hurt Box";

        public override void Init()
        {
            SizeMultiplier = ConfigOption(0.66f, "Size Multiplier", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var rex = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotBody.prefab").WaitForCompletion();
            var mainHurtBox = rex.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("TempHurtbox").GetComponent<CapsuleCollider>();
            mainHurtBox.radius = 1.42f * SizeMultiplier;
            mainHurtBox.height = 4.26f * SizeMultiplier;
        }
    }
}