using BetterCommand.Source.Utils;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using BetterCommand.Source.DataStructures;

namespace BetterCommand.Source.NetworkMessages
{

    public enum PickupPickerMessageType
    {
        Add,
        Remove
    }

    public class PickupPickerMessage : INetMessage
    {
        NetworkInstanceId characterMasterNetId;
        PickupPickerMessageType pickupPickerMessageType;

        public PickupPickerMessage()
        {

        }

        public PickupPickerMessage(NetworkInstanceId characterMasterNetId, PickupPickerMessageType pickupPickerMessageType)
        {
            this.characterMasterNetId = characterMasterNetId;
            this.pickupPickerMessageType = pickupPickerMessageType;
        }

        public void Deserialize(NetworkReader reader)
        {
            characterMasterNetId = reader.ReadNetworkId();
            pickupPickerMessageType = (PickupPickerMessageType)reader.ReadInt32();
        }

        public void OnReceived()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            GameObject gameObject = Util.FindNetworkObject(characterMasterNetId);

            if (!gameObject)
            {
#if DEBUG   
                Log.Error("Cannot apply buff because the target couldn't be found");
#endif
                return;
            }

            if (gameObject.TryGetComponent(out CharacterMaster characterMaster))
            {
                CharacterBody characterBody = characterMaster.GetBody();

                switch (pickupPickerMessageType)
                {
                    case PickupPickerMessageType.Add:
                        BetterCommand.currentlyInItemPickerPlayers.Add(new PlayerHealthData(gameObject, characterBody.healthComponent.health));

                        characterBody.AddBuff(DLC3Content.Buffs.Untargetable);
                        characterBody.AddBuff(RoR2Content.Buffs.Immune);
                        break;
                    case PickupPickerMessageType.Remove:
                        BetterCommand.currentlyInItemPickerPlayers.RemoveAll(currentlyInItemPickerPlayer => currentlyInItemPickerPlayer.Player == gameObject);

                        characterBody.RemoveBuff(DLC3Content.Buffs.Untargetable);
                        characterBody.RemoveBuff(RoR2Content.Buffs.Immune);
                        break;
                    default:
                        break;
                }
            }

        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(characterMasterNetId);
            writer.Write((int)pickupPickerMessageType);
        }
    }
}
