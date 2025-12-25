using BepInEx;
using BetterCommand.Source.NetworkMessages;
using BetterCommand.Source.Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine.Networking;

namespace BetterCommand.Source
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(NetworkingAPI.PluginGUID)]
    public class BetterCommand : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "AnnasVirtual";
        public const string PluginName = "BetterCommand";
        public const string PluginVersion = "1.0.1";

        public void Awake()
        {
            Log.Init(Logger);

            NetworkingAPI.RegisterMessageType<PickupPickerMessage>();

            On.RoR2.PickupPickerController.OnDisplayBegin += (On.RoR2.PickupPickerController.orig_OnDisplayBegin orig, PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController) =>
            {
                if (NetworkServer.active)
                {
                    Helper.ApplyBetterCommandOnServer(networkUIPromptController.currentParticipantMaster.gameObject, PickupPickerMessageType.Add);

                    orig(self, networkUIPromptController, localUser, cameraRigController);
                    return;
                }

                new PickupPickerMessage(networkUIPromptController.currentParticipantMaster.netId, PickupPickerMessageType.Add).Send(NetworkDestination.Server);

                orig(self, networkUIPromptController, localUser, cameraRigController);
            };

            On.RoR2.PickupPickerController.OnDisplayEnd += (On.RoR2.PickupPickerController.orig_OnDisplayEnd orig, PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController) =>
            {
                if (NetworkServer.active)
                {
                    Helper.ApplyBetterCommandOnServer(networkUIPromptController.currentParticipantMaster.gameObject, PickupPickerMessageType.Remove);

                    orig(self, networkUIPromptController, localUser, cameraRigController);
                    return;
                }

                new PickupPickerMessage(networkUIPromptController.currentParticipantMaster.netId, PickupPickerMessageType.Remove).Send(NetworkDestination.Server);

                orig(self, networkUIPromptController, localUser, cameraRigController);
            };

#if DEBUG
            Log.Info("Better Command is Test 21");
#endif

            Log.Info("Better Command is loaded");
        }
    }
}