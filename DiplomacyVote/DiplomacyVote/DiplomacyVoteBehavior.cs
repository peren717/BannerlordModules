using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace DiplomacyVote
{
    class DiplomacyVoteBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnDailyTick()
        {
            try
            {
                Kingdom kingdom = Clan.PlayerClan.Kingdom;
                foreach(Kingdom k in Kingdom.All)
                {
                    if(k != kingdom && k.Settlements.Count()>0 && !k.IsAtWarWith(kingdom))
                    {
                        Campaign.Current.AddDecision(new NewDeclareWarKingdomDecision(kingdom.RulingClan, k));
                        break;
                    }
                }

            }
            catch
            {

            }

        }
    }
}
