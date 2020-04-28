using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace InsultAnyLord
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                try
                {
                    campaignGameStarter.AddBehavior(new InsultingBehavior());
                }
                catch (Exception e)
                {
                    InformationManager.DisplayMessage(new InformationMessage("OnGameStart Error: " + e.Message));
                }


            }
        }

    }
}