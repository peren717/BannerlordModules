using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace DiplomacyVote
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                new Harmony("UniversityofUtah.peren717.DiplomayVote").PatchAll();

            }
            catch
            {

            }
            
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                campaignGameStarter.AddBehavior(new DiplomacyVoteBehavior());

            }

        }

    }
}