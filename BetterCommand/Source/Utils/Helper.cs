using BetterCommand.Source.NetworkMessages;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterCommand.Source.Utils
{
    public static class Helper
    {
        public static void ApplyBetterCommandOnServer(GameObject playerGameObject, PickupPickerMessageType pickupPickerMessageType)
        {
            if (!NetworkServer.active)
            {
                Log.Error("Cannot apply the modifier because this function is not run on the server");
                return;
            }

            if (playerGameObject.TryGetComponent(out CharacterMaster characterMaster))
            {
                CharacterBody characterBody = characterMaster.GetBody();

                switch (pickupPickerMessageType)
                {
                    case PickupPickerMessageType.Add:
                        characterBody.healthComponent.enabled = false;
                        characterBody.AddBuff(DLC3Content.Buffs.Untargetable);
                        characterBody.AddBuff(RoR2Content.Buffs.Immune); // this is just for visual actually cuz disabling healthComponent also remove damage, but idk if this is safe so i'm testing
                        break;

                    case PickupPickerMessageType.Remove:
                        characterBody.healthComponent.enabled = true;
                        characterBody.RemoveBuff(DLC3Content.Buffs.Untargetable);
                        characterBody.RemoveBuff(RoR2Content.Buffs.Immune);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                Log.Error("Error player game object doesn't have the CharacterMaster component");
            }
        }
    }
}
