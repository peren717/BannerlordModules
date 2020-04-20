using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NoChild
{
    public class SubModule : MBSubModuleBase // Must have a class inherting MBSubModuleBase, the entry point of the mod
    {      
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) // Called immediately upon loading after selecting a game mode (submodule) from the main menu
        {
            base.OnGameStart(game, gameStarterObject);
            if(game.GameType is Campaign)
            {
                gameStarterObject.AddModel(new NoChild());
            }
        }

    }
}
