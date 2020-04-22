using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace PolicyOverhaul
{
    class NewKingSelectionKingdomDecision : KingSelectionKingdomDecision
    {
        public NewKingSelectionKingdomDecision(Clan proposerClan) : base(proposerClan)
        {
            this.proposerClan = proposerClan;
        }

        public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
        {
            Kingdom kingdom = proposerClan.Kingdom;
            Clan oldRuler = kingdom.RulingClan;
            base.ApplyChosenOutcome(chosenOutcome);
            oldRuler.Banner.ChangePrimaryColor(kingdom.PrimaryBannerColor);
            oldRuler.Banner.ChangeIconColors(kingdom.SecondaryBannerColor);
        }

        Clan proposerClan;
    }
}
