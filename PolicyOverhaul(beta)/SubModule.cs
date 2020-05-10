using HarmonyLib;
using ModLib;
using ModLib.Definitions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace PolicyOverhaul
{
    public class SubModule : MBSubModuleBase // Must have a class inherting MBSubModuleBase, the entry point of the mod
    {
        protected override void OnSubModuleLoad() // Called during the first loading screen of the game, always the first override to be called, this is where you should be doing the bulk of your initial setup
        {
            base.OnSubModuleLoad();
            try
            {
                new Harmony("UniversityofUtah.peren717.policyOverhaul").PatchAll();
            }
            catch (Exception exception)
            {
                MessageBox.Show("harmony NOT Patched: "+exception.Message);
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage("PolicyOverhaul Loaded!", Colors.Green));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) // Called immediately upon loading after selecting a game mode (submodule) from the main menu
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                campaignGameStarter.AddBehavior(new PolicyOverhaulCampaignBehavior());
                campaignGameStarter.AddBehavior(new SeparateBehaviour());
            }
        }

    }
}
