using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HIFURexTweaks.Skills
{
    public class DIRECTIVEHarvest : TweakBase
    {
        public static float damage;
        public static float cooldown;
        public static float endlag;
        public static float percentHeal;
        public static bool giveBuffs;
        public static float buffDuration;
        public static BuffDef checkIfEligible;
        public static BuffDef armorBuffDef;
        public static BuffDef damageBuffDef;
        public static BuffDef attackSpeedBuffDef;
        public static BuffDef movementSpeedBuffDef;
        public static float armorBuff;
        public static float damageBuff;
        public static float attackSpeedBuff;
        public static float movementSpeedBuff;
        public static BuffDef uselessBuff;

        public override string Name => ": Special : DIRECTIVE: Harvest";

        public override string SkillToken => "special_alt1";

        public override string DescText => "Fire a bolt that deals <style=cIsDamage>" + d(damage) + " damage</style> and <style=cIsDamage>injects</style> an enemy. On death, drop multiple <style=cIsHealing>healing fruits</style> that heal for <style=cIsHealing>" + d(percentHeal) + " HP</style>" +
                                           (giveBuffs ? " and <style=cIsUtility>give a short buff</style> each." : ".");

        public override void Init()
        {
            damage = ConfigOption(6f, "Damage", "Decimal. Vanilla is 3.3");
            endlag = ConfigOption(0.3f, "Endlag", "Vanilla is 1");
            cooldown = ConfigOption(6f, "Cooldown", "Vanilla is 6");
            percentHeal = ConfigOption(0.12f, "Percent Heal", "Decimal. Vanilla is 0.25");
            giveBuffs = ConfigOption(true, "Should fruit give Buffs?", "Vanilla is false");
            buffDuration = ConfigOption(6f, "Buff Duration", "Vanilla is 0");
            armorBuff = ConfigOption(8f, "Armor Buff", "Vanilla is 0");
            damageBuff = ConfigOption(0.08f, "Damage Buff", "Decimal. Vanilla is 0");
            attackSpeedBuff = ConfigOption(0.13f, "Attack Speed Buff", "Decimal. Vanilla, is 0");
            movementSpeedBuff = ConfigOption(0.13f, "Movement Speed Buff", "Decimal. Vanilla is 0");

            uselessBuff = ScriptableObject.CreateInstance<BuffDef>();
            uselessBuff.isHidden = true;
            uselessBuff.isDebuff = false;
            uselessBuff.canStack = false;
            uselessBuff.isCooldown = false;
            uselessBuff.name = "Useless Buff";

            ContentAddition.AddBuffDef(uselessBuff);

            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Treebot.TreebotFireFruitSeed.OnEnter += TreebotFireFruitSeed_OnEnter;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.BuffPickup.OnTriggerStay += BuffPickup_OnTriggerStay;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            // GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.EntityStates.Treebot.TreebotPrepFruitSeed.OnEnter += TreebotPrepFruitSeed_OnEnter;
        }

        private void TreebotPrepFruitSeed_OnEnter(On.EntityStates.Treebot.TreebotPrepFruitSeed.orig_OnEnter orig, EntityStates.Treebot.TreebotPrepFruitSeed self)
        {
            self.baseDuration = 1f / 60f;
            self.duration = 1f / 60f;
            orig(self);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            if (!report.attacker)
            {
                return;
            }

            var attackerTeamIndex = report.attackerTeamIndex;
            var victimBody = report.victimBody;
            if (!victimBody)
            {
                return;
            }

            if (victimBody.HasBuff(RoR2Content.Buffs.Fruiting) || (report.damageInfo != null && (report.damageInfo.damageType & DamageType.FruitOnHit) > DamageType.Generic))
            {
                var whatTheFuck = Mathf.Min(Math.Max(1, (int)(victimBody.bestFitRadius * 2f)), 8);
                var harvestPickup = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/TreebotFruitPack");
                for (int i = 0; i < whatTheFuck; i++)
                {
                    var instantiated = UnityEngine.Object.Instantiate(harvestPickup, victimBody.transform.position + Random.insideUnitSphere * victimBody.radius * 0.5f, Random.rotation);
                    var teamFilter = instantiated.GetComponent<TeamFilter>();
                    if (teamFilter)
                    {
                        teamFilter.teamIndex = attackerTeamIndex;
                    }
                    instantiated.GetComponentInChildren<HealthPickup>();
                    instantiated.transform.localScale = new Vector3(1f, 1f, 1f);
                    NetworkServer.Spawn(instantiated);
                }
            }
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdstr("Prefabs/Effects/TreebotFruitDeathEffect.prefab")))
            {
                for (int i = 0; i < 11; i++)
                {
                    c.Remove();
                }
            }
            else
            {
                Main.HRTLogger.LogError("Failed to apply VFX Hook");
            }

            c.Index = 0;
        }

        private void TreebotFireFruitSeed_OnEnter(On.EntityStates.Treebot.TreebotFireFruitSeed.orig_OnEnter orig, EntityStates.Treebot.TreebotFireFruitSeed self)
        {
            self.damageCoefficient = damage;
            self.baseDuration = endlag;
            orig(self);
        }

        public static void BuffPickup_OnTriggerStay(On.RoR2.BuffPickup.orig_OnTriggerStay orig, BuffPickup self, Collider other)
        {
            if (self && self.buffDef == checkIfEligible)
            {
                switch (Random.RandomRangeInt(1, 5))
                {
                    case 1:
                        self.buffDef = armorBuffDef;
                        break;

                    case 2:
                        self.buffDef = damageBuffDef;
                        break;

                    case 3:
                        self.buffDef = attackSpeedBuffDef;
                        break;

                    default:
                        self.buffDef = movementSpeedBuffDef;
                        break;
                }
            }
            orig(self, other);
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                args.armorAdd += armorBuff * sender.GetBuffCount(armorBuffDef);
                args.damageMultAdd += damageBuff * sender.GetBuffCount(damageBuffDef);
                args.attackSpeedMultAdd += attackSpeedBuff * sender.GetBuffCount(attackSpeedBuffDef);
                args.moveSpeedMultAdd += movementSpeedBuff * sender.GetBuffCount(movementSpeedBuffDef);
            }
        }

        public static void Changes()
        {
            LanguageAPI.Add("TREEBOT_SPECIAL_ALT1_NAME", "DIRECTIVE: Harvest");

            var harvestDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyFireFruitSeed.asset").WaitForCompletion();
            harvestDef.baseRechargeInterval = cooldown;
            var harvest = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFruitPack.prefab").WaitForCompletion();
            GameObject.Destroy(harvest.GetComponent<EffectComponent>());
            var harvestTrigger = harvest.transform.GetChild(2);
            harvestTrigger.GetComponent<HealthPickup>().fractionalHealing = percentHeal;

            checkIfEligible = ScriptableObject.CreateInstance<BuffDef>();
            checkIfEligible.isDebuff = false;
            checkIfEligible.canStack = false;
            checkIfEligible.iconSprite = null;
            checkIfEligible.buffColor = new Color32(255, 255, 255, 255);
            checkIfEligible.isHidden = true;
            ContentAddition.AddBuffDef(checkIfEligible);

            var harvestProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFruitSeedProjectile.prefab").WaitForCompletion();
            harvestProjectile.transform.localScale = new Vector3(2f, 2f, 2f);

            if (giveBuffs)
            {
                var ArmorIcon = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texBuffGenericShield.tif").WaitForCompletion();
                var DamageIcon = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/LunarSkillReplacements/texBuffLunarDetonatorIcon.tif").WaitForCompletion();
                var AttackSpeedIcon = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/AttackSpeedOnCrit/texBuffAttackSpeedOnCritIcon.tif").WaitForCompletion();
                var SpeedIcon = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texMovespeedBuffIcon.tif").WaitForCompletion();

                armorBuffDef = ScriptableObject.CreateInstance<BuffDef>();
                armorBuffDef.isDebuff = false;
                armorBuffDef.canStack = true;
                armorBuffDef.iconSprite = Sprite.Create(ArmorIcon, new Rect(0f, 0f, (float)ArmorIcon.width, (float)ArmorIcon.height), new Vector2(0f, 0f));
                armorBuffDef.buffColor = new Color32(243, 243, 184, 225);
                armorBuffDef.name = "DIRECTIVE: Harvest Armor";
                armorBuffDef.canStack = true;
                ContentAddition.AddBuffDef(armorBuffDef);

                damageBuffDef = ScriptableObject.CreateInstance<BuffDef>();
                damageBuffDef.isDebuff = false;
                damageBuffDef.canStack = true;
                damageBuffDef.iconSprite = Sprite.Create(DamageIcon, new Rect(0f, 0f, (float)DamageIcon.width, (float)DamageIcon.height), new Vector2(0f, 0f));
                damageBuffDef.buffColor = new Color32(243, 243, 184, 225);
                damageBuffDef.name = "DIRECTIVE: Harvest Damage";
                damageBuffDef.canStack = true;
                ContentAddition.AddBuffDef(damageBuffDef);

                attackSpeedBuffDef = ScriptableObject.CreateInstance<BuffDef>();
                attackSpeedBuffDef.isDebuff = false;
                attackSpeedBuffDef.canStack = true;
                attackSpeedBuffDef.iconSprite = Sprite.Create(AttackSpeedIcon, new Rect(0f, 0f, (float)AttackSpeedIcon.width, (float)AttackSpeedIcon.height), new Vector2(0f, 0f));
                attackSpeedBuffDef.buffColor = new Color32(243, 243, 184, 225);
                attackSpeedBuffDef.name = "DIRECTIVE: Harvest Attack Speed";
                attackSpeedBuffDef.canStack = true;
                ContentAddition.AddBuffDef(attackSpeedBuffDef);

                movementSpeedBuffDef = ScriptableObject.CreateInstance<BuffDef>();
                movementSpeedBuffDef.isDebuff = false;
                movementSpeedBuffDef.canStack = true;
                movementSpeedBuffDef.iconSprite = Sprite.Create(SpeedIcon, new Rect(0f, 0f, (float)SpeedIcon.width, (float)SpeedIcon.height), new Vector2(0f, 0f));
                movementSpeedBuffDef.buffColor = new Color32(243, 243, 184, 225);
                movementSpeedBuffDef.name = "DIRECTIVE: Harvest Move Speed";
                movementSpeedBuffDef.canStack = true;
                ContentAddition.AddBuffDef(movementSpeedBuffDef);

                var pickupGlow = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/HealthOrbEffect.prefab").WaitForCompletion(), "DIRECTIVE: Harvest Pickup", false);

                pickupGlow.SetActive(false);

                ContentAddition.AddEffect(pickupGlow);

                var buffPickup = harvestTrigger.gameObject.AddComponent<BuffPickup>();
                buffPickup.teamFilter = harvest.GetComponent<TeamFilter>();
                buffPickup.buffDef = checkIfEligible;
                buffPickup.buffDuration = buffDuration;
                buffPickup.pickupEffect = pickupGlow;
                buffPickup.baseObject = harvestTrigger.gameObject;
            }
        }
    }
}