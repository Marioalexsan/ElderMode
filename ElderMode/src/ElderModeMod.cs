using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding;
using SoG.Modding.Content;

namespace ElderMode
{
    [ModDependency("GrindScript", "0.14")]
    public class ElderModeMod : Mod
    {
        public override Version ModVersion => new Version("0.14");

        public override string NameID => "ElderModeMod";

        public override bool AllowDiscoveryByMods => false;

        private Dictionary<EnemyCodex.EnemyTypes, EnemyCodex.EnemyTypes> _enemiesWithSwaps;

        private double _upgradeChance = 0.45f;

        private double _hyperEliteChance = 0.2f;

        private Dictionary<EnemyCodex.EnemyTypes, double> _chanceMultipliers;

        private Harmony _harmony;

        private ElderModeMod()
        {
            _harmony = new Harmony(this.GetType().FullName);

            _enemiesWithSwaps = new Dictionary<EnemyCodex.EnemyTypes, EnemyCodex.EnemyTypes>()
            {
                [EnemyCodex.EnemyTypes.GreenSlime] = EnemyCodex.EnemyTypes.RedSlime,
                [EnemyCodex.EnemyTypes.RedSlime] = EnemyCodex.EnemyTypes.BlueSlime,
                [EnemyCodex.EnemyTypes.BlueSlime] = EnemyCodex.EnemyTypes.Desert_OrangeSlime,
                [EnemyCodex.EnemyTypes.Desert_OrangeSlime] = EnemyCodex.EnemyTypes.PapaSlime,
                [EnemyCodex.EnemyTypes.PapaSlime] = EnemyCodex.EnemyTypes.RedPapaSlime,
                [EnemyCodex.EnemyTypes.Rabbi] = EnemyCodex.EnemyTypes.RabbyWhite,
                [EnemyCodex.EnemyTypes.Bee] = EnemyCodex.EnemyTypes.BeeDiGuard,
                [EnemyCodex.EnemyTypes.BeeDiGuard] = EnemyCodex.EnemyTypes.BeeHive,
                [EnemyCodex.EnemyTypes.BeeHive] = EnemyCodex.EnemyTypes.QueenBee,
                [EnemyCodex.EnemyTypes.Blomma] = EnemyCodex.EnemyTypes.Halloweed,
                [EnemyCodex.EnemyTypes.Halloweed] = EnemyCodex.EnemyTypes.TerrorWeed,
                [EnemyCodex.EnemyTypes.Boar] = EnemyCodex.EnemyTypes.Special_ElderBoar,
                [EnemyCodex.EnemyTypes.Pumpkin] = EnemyCodex.EnemyTypes.GhostShip_LivingSkull,
                [EnemyCodex.EnemyTypes.Scarecrow] = EnemyCodex.EnemyTypes.Pumpking,
                [EnemyCodex.EnemyTypes.Ghosty] = EnemyCodex.EnemyTypes.GhostShip_Hauntie,
                [EnemyCodex.EnemyTypes.Guardian] = EnemyCodex.EnemyTypes.RiftCrystal,
                [EnemyCodex.EnemyTypes.FrostlingRogue] = EnemyCodex.EnemyTypes.FrostlingBoss,
                [EnemyCodex.EnemyTypes.FrostlingScoundrel] = EnemyCodex.EnemyTypes.FrostlingBoss,
                [EnemyCodex.EnemyTypes.MtBloom_CrystalBeetle] = EnemyCodex.EnemyTypes.MtBloom_CrystalBeetleRed,
                [EnemyCodex.EnemyTypes.MtBloom_Shroom] = EnemyCodex.EnemyTypes.MtBloom_FireShroom,
                [EnemyCodex.EnemyTypes.MtBloom_PoisonFlower] = EnemyCodex.EnemyTypes.MtBloom_PoisonFlower_Huge,
                [EnemyCodex.EnemyTypes.MtBloom_Larva] = EnemyCodex.EnemyTypes.TimeTemple_Worm,
                [EnemyCodex.EnemyTypes.TimeTemple_Worm] = EnemyCodex.EnemyTypes.TimeTemple_GiantWorm,
                [EnemyCodex.EnemyTypes.TimeTemple_Echo] = EnemyCodex.EnemyTypes.TimeTemple_Zhamla,
                [EnemyCodex.EnemyTypes.Desert_Solem] = EnemyCodex.EnemyTypes.Desert_SolemBossSolgem,
                [EnemyCodex.EnemyTypes.Marino] = EnemyCodex.EnemyTypes.MarinoV2,
            };

            _chanceMultipliers = new Dictionary<EnemyCodex.EnemyTypes, double>()
            {
                [EnemyCodex.EnemyTypes.MtBloom_PoisonFlower] = 0.12, // Too tanky to spawn often
                [EnemyCodex.EnemyTypes.TimeTemple_Worm] = 0.3,
                [EnemyCodex.EnemyTypes.TimeTemple_Echo] = 0.0, // LOL GOOD LUCK IF HE SPAWNS
                [EnemyCodex.EnemyTypes.Desert_Solem] = 0.0, // Has issues
                [EnemyCodex.EnemyTypes.Desert_OrangeSlime] = 0.3,
                [EnemyCodex.EnemyTypes.PapaSlime] = 1337.0, // Guaranteed replacement
                [EnemyCodex.EnemyTypes.Marino] = 1337.0, // Guaranteed replacement
            };
        }

        public override void Load()
        {
            CommandEntry commands = CreateCommands();

            commands.SetCommand("SetChance", (args, _) =>
            {
                if (args.Length == 0)
                {
                    CAS.AddChatMessage("Reset chance to 0.45!");
                    _upgradeChance = 0.45;
                    return;
                }

                if (Double.TryParse(args[0], out double result) && result >= 0.0 && result <= 1.0)
                {
                    CAS.AddChatMessage("Updated upgrade chance!");
                    _upgradeChance = result;
                }
                else
                {
                    CAS.AddChatMessage("You must enter a valid upgrade chance!");
                }
            });

            Logger.Debug("Patching...");

            _harmony.PatchAll(this.GetType().Assembly);

            Logger.Debug("Patched!");
        }

        public override void Unload()
        {
            Logger.Debug("Unpatching...");

            _harmony.UnpatchAll(_harmony.Id);

            Logger.Debug("Unpatched!");
        }

        public override void OnEnemySpawn(ref EnemyCodex.EnemyTypes enemy, ref Vector2 position, ref bool isElite, ref bool dropsLoot, ref int bitLayer, ref float virtualHeight, float[] behaviourVariables)
        {
            double baseChance = _upgradeChance;

            if (_chanceMultipliers.ContainsKey(enemy))
            {
                baseChance *= _chanceMultipliers[enemy];
            }

            if (Globals.Game.randomInLogic.NextDouble() > baseChance)
            {
                return;
            }

            if (_enemiesWithSwaps.ContainsKey(enemy))
            {
                // Try swap
                enemy = _enemiesWithSwaps[enemy];
            }
            else
            {
                isElite = true;
            }
        }

        public override void PostEnemySpawn(Enemy entity, EnemyCodex.EnemyTypes enemy, EnemyCodex.EnemyTypes original, Vector2 position, bool isElite, bool dropsLoot, int bitLayer, float virtualHeight, float[] behaviourVariables)
        {
            bool isReplacement = _enemiesWithSwaps.ContainsKey(original);

            bool isStandaloneGiga = enemy == EnemyCodex.EnemyTypes.PapaSlime && original == EnemyCodex.EnemyTypes.Desert_OrangeSlime;

            bool isQueenBee = enemy == EnemyCodex.EnemyTypes.QueenBee && original == EnemyCodex.EnemyTypes.Bee;

            bool isSolgem = enemy == EnemyCodex.EnemyTypes.Desert_SolemBossSolgem && original == EnemyCodex.EnemyTypes.Desert_Solem;

            bool isBoss = entity.xEnemyDescription.enCategory != EnemyDescription.Category.Regular;

            EnemySpawner spawner = Globals.Game.EXT_GetContainingSpawner(entity.xTransform.v2Pos);

            if (isBoss)
            {
                entity.rcRegularHPRenderComponent = new RegularEnemyHPRenderComponent(entity);
                entity.v2DamageNumberPosOffset += new Vector2(0f, -4f);
                entity.iOverrideDamageNumberWidth = 46;
                Globals.Game.xRenderMaster.RegisterGUIRenderComponent(entity.rcRegularHPRenderComponent);
                (entity as Boss).xHPRenderComponent.v2OffsetRenderPos = new Vector2(0f, 1000f);
            }

            if (spawner != null)
            {
                if (spawner.recHomeRectangle.IsEmpty)
                {
                    // Set the home rectangle for the spawner
                    spawner.recHomeRectangle = Globals.Game.EXT_GetHomeRectangle(entity);
                }

                List<EnemyCodex.EnemyTypes> spawnedTypes = (List<EnemyCodex.EnemyTypes>)AccessTools.Field(typeof(EnemySpawner), "lenSpawnedTypes").GetValue(spawner);

                if (!spawnedTypes.Contains(enemy))
                {
                    spawnedTypes.Add(enemy);
                }
            }

            if (isStandaloneGiga)
            {
                // Initialize standalone giga
                Behaviours.PapaSlime gigaSlime = (entity.xBehaviour as Behaviours.PapaSlime);
                AccessTools.Field(typeof(Behaviours.PapaSlime), "bSpawnedByMimic").SetValue(gigaSlime, true);

                // Make it a bit tankier
                entity.xBaseStats.iBaseMaxHP = (int)(entity.xBaseStats.iBaseMaxHP * 1.65f);
                entity.xBaseStats.iHP = entity.xBaseStats.iMaxHP;

                entity.xBehaviour.bActive = true;
                entity.bDropsAnyLoot = true;
            }

            if (isSolgem)
            {
                // Initialize standalone solgem
                AccessTools.Field(typeof(Behaviours.SolemAI), "bInited").SetValue(entity.xBehaviour as Behaviours.SolemAI, true);
                (entity.xBehaviour as Behaviours.SolemAI).recFallingRocksRec = entity.xBehaviour.recHomeRectangle;
            }

            if (!isBoss && entity.xBehaviour.bIsElite && Globals.Game.randomInLogic.NextDouble() <= _hyperEliteChance)
            {
                ToHyperElite(entity);
            }
        }

        private void ToHyperElite(Enemy enemy)
        {
            if (!enemy.xBehaviour.bIsElite)
            {
                return;
            }

            enemy.xEliteEffect.xRenderComponent.cColor = Color.PaleVioletRed;
            enemy.xEliteEffect.xRenderComponent.AsAnimated().fAnimationTimeWarp = 2f;
            enemy.bHyperElite = true;
            enemy.iBonusMoney *= 2;
            enemy.xBaseStats.fMaxHPMultiplier *= 1.5f;
            enemy.xBaseStats.iBaseDEF += 40;
            enemy.xBaseStats.iHP = enemy.xBaseStats.iMaxHP;
            enemy.xBaseStats.iBaseATK = (int)(enemy.xBaseStats.iBaseATK * 1.8f);
        }
    }
}
