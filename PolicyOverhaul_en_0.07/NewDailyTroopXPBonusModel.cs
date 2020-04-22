using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace PolicyOverhaul
{
    class NewDailyTroopXPBonusModel : DefaultDailyTroopXpBonusModel
    {
        public override float CalculateGarrisonXpBonusMultiplier(Town town, StatExplainer explanation = null)
        {
            float num = base.CalculateGarrisonXpBonusMultiplier(town, explanation);
            try
            {
                if(town.Settlement.OwnerClan.Kingdom.HasPolicy(NewPolicies.ProfessionalArmy))
                {
                    num = 2 * num;
                }
            }
            catch(Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(e.Message));
            }
            return num;

        }
    }
}
