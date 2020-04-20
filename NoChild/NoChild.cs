using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace NoChild
{
    class NoChild: DefaultPregnancyModel
    {
        public override float GetDailyChanceOfPregnancyForHero(Hero hero)
        {
            return 0f;
        }
    }
}
