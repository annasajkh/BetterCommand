using BetterCommand.Source.Utils;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

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
                Log.Error("Cannot apply the modifier because this function is not run on the server");
                return;
            }

            GameObject playerGameObject = Util.FindNetworkObject(characterMasterNetId);

            if (!playerGameObject)
            {
                Log.Error("Cannot find player network game object");
                return;
            }

            Helper.ApplyBetterCommandOnServer(playerGameObject, pickupPickerMessageType);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(characterMasterNetId);
            writer.Write((int)pickupPickerMessageType);
        }
    }
}
