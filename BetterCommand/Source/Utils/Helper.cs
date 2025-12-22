using BetterCommand.Source.NetworkMessages;
using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BaseAI = RoR2.CharacterAI.BaseAI;
using Idle = EntityStates.Idle;

namespace BetterCommand.Source.Utils
{
    public static class Helper
    {
        public static List<MinionOwnership> GetMinionOfPlayer(CharacterMaster playerMaster)
        {
            List<MinionOwnership> playerMinions = new();

            MinionOwnership[] minionOwnerships = Object.FindObjectsOfType<MinionOwnership>();

            foreach (MinionOwnership minionOwnership in minionOwnerships)
            {
                if (minionOwnership.ownerMaster == playerMaster)
                {
                    playerMinions.Add(minionOwnership);
                }
            }

            return playerMinions;
        }

        public static void ApplyBetterCommandOnServer(GameObject gameObject, PickupPickerMessageType pickupPickerMessageType)
        {
            if (!NetworkServer.active)
            {
                Log.Error("Cannot apply the modifiers because this function is not run on the server");
                return;
            }

            if (gameObject.TryGetComponent(out CharacterMaster characterMaster))
            {
#if DEBUG
                switch (pickupPickerMessageType)
                {
                    case PickupPickerMessageType.Add:
                        Log.Info($"Adding Better Command effect to {Util.GetBestMasterName(characterMaster)}");
                        break;
                    case PickupPickerMessageType.Remove:
                        Log.Info($"Removing Better Command effect from {Util.GetBestMasterName(characterMaster)}");
                        break;
                }
#endif

                CharacterBody playerCharacterBody = characterMaster.GetBody();

                List<MinionOwnership> playerMinions = GetMinionOfPlayer(characterMaster);

                switch (pickupPickerMessageType)
                {
                    case PickupPickerMessageType.Add:
                        foreach (MinionOwnership minionOwnership in playerMinions)
                        {
                            if (minionOwnership is null)
                            {
                                continue;
                            }

                            CharacterMaster minionCharacterMaster = minionOwnership.gameObject.GetComponent<CharacterMaster>();

                            if (minionCharacterMaster is null)
                            {
                                continue;
                            }

                            if (minionCharacterMaster.GetBody() is null)
                            {
                                continue;
                            }

                            minionCharacterMaster.GetBody().AddBuff(RoR2Content.Buffs.HealingDisabled);
                            minionCharacterMaster.GetBody().AddBuff(DLC3Content.Buffs.Untargetable);
                            minionCharacterMaster.GetBody().AddBuff(RoR2Content.Buffs.Immune);
                            minionCharacterMaster.GetBody().AddBuff(RoR2Content.Buffs.Nullified);

                            foreach (BaseAI aiComponent in minionCharacterMaster.AiComponents)
                            {
                                foreach (AISkillDriver skillDriver in aiComponent.skillDrivers)
                                {
                                    skillDriver.aimType = AISkillDriver.AimType.None;
                                }
                            }
                        }

                        playerCharacterBody.AddBuff(RoR2Content.Buffs.HealingDisabled);
                        playerCharacterBody.AddBuff(DLC3Content.Buffs.Untargetable);
                        playerCharacterBody.AddBuff(RoR2Content.Buffs.Immune);
                        break;

                    case PickupPickerMessageType.Remove:
                        foreach (MinionOwnership minionOwnership in playerMinions)
                        {
                            if (minionOwnership is null)
                            {
                                continue;
                            }

                            CharacterMaster minionCharacterMaster = minionOwnership.gameObject.GetComponent<CharacterMaster>();

                            if (minionCharacterMaster is null)
                            {
                                continue;
                            }

                            if (minionCharacterMaster.GetBody() is null)
                            {
                                continue;
                            }

                            minionCharacterMaster.GetBody().RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                            minionCharacterMaster.GetBody().RemoveBuff(DLC3Content.Buffs.Untargetable);
                            minionCharacterMaster.GetBody().RemoveBuff(RoR2Content.Buffs.Immune);
                            minionCharacterMaster.GetBody().RemoveBuff(RoR2Content.Buffs.Nullified);

                            foreach (BaseAI aiComponent in minionCharacterMaster.AiComponents)
                            {
                                foreach (AISkillDriver skillDriver in aiComponent.skillDrivers)
                                {
                                    skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
                                }
                            }
                        }

                        playerCharacterBody.RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                        playerCharacterBody.RemoveBuff(DLC3Content.Buffs.Untargetable);
                        playerCharacterBody.RemoveBuff(RoR2Content.Buffs.Immune);
                        break;
                }
            }
            else
            {
                Log.Error("Cannot apply the modifiers because this gameObject doesn't have CharacterMaster component");
            }
        }
    }
}
