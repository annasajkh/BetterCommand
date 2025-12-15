using BepInEx;
using BetterCommand.Source.DataStructures;
using BetterCommand.Source.NetworkMessages;
using BetterCommand.Source.Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using R2API.Utils;
using Rewired;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
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
        public const string PluginVersion = "0.0.1";

        public static List<PlayerHealthData> currentlyInItemPickerPlayers = new();

        public void Awake()
        {
            Log.Init(Logger);

            NetworkingAPI.RegisterMessageType<PickupPickerMessage>();

#if DEBUG
            On.RoR2.Run.BeginStage += (On.RoR2.Run.orig_BeginStage orig, Run self) =>
            {
                if (!NetworkServer.active)
                {
                    orig(self);
                    return;
                }

                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    playerCharacterMasterController.master.GiveMoney(10_000_000);
                }

                orig(self);
            };
#endif

            On.RoR2.PickupPickerController.OnDisplayBegin += (On.RoR2.PickupPickerController.orig_OnDisplayBegin orig, PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController) =>
            {
                new PickupPickerMessage(localUser.cachedMaster.netId, PickupPickerMessageType.Add).Send(NetworkDestination.Server);

                orig(self, networkUIPromptController, localUser, cameraRigController);
            };

            On.RoR2.PickupPickerController.OnDisplayEnd += (On.RoR2.PickupPickerController.orig_OnDisplayEnd orig, PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController) =>
            {
                new PickupPickerMessage(localUser.cachedMaster.netId, PickupPickerMessageType.Remove).Send(NetworkDestination.Server);

                orig(self, networkUIPromptController, localUser, cameraRigController);
            };

            Log.Info("Better Command is loaded");
        }

        public void Update()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            foreach (PlayerHealthData playerHealthData in currentlyInItemPickerPlayers)
            {
                if (playerHealthData.Player.TryGetComponent(out CharacterMaster characterMaster))
                {
                    CharacterBody characterBody = characterMaster.GetBody();
                    characterBody.healthComponent.health = playerHealthData.CurrentHealth;
                }
            }
        }
    }
}