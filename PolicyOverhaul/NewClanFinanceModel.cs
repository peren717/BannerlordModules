using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace PolicyOverhaul
{
    class NewClanFinanceModel : DefaultClanFinanceModel
    {
        public override int CalculatePartyWage(MobileParty mobileParty, bool applyWithdrawals)
        {

            int num = base.CalculatePartyWage(mobileParty, applyWithdrawals);
            try
            {
                //if (mobileParty.Party.Owner.Clan.Kingdom.ActivePolicies.Contains(NewPolicies.ProfessionalArmy))
                //{
                //    num = 2 * num;
                //}





                return num;
            }
            catch (Exception e)
            {
                MessageBox.Show("NewClanFinaceModel Error: " + e.Message);
            }

            return num;
        }
    }
}
