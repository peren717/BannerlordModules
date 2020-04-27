using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.SaveSystem;
using static TaleWorlds.CampaignSystem.Election.NewKingSelectionKingdomDecision;

namespace PolicyOverhaul
{
    public class CustomSaveDefiner : SaveableTypeDefiner
    {
        public CustomSaveDefiner() : base(404208334) { }

        protected override void DefineClassTypes()
        {
            // The Id's here are local and will be related to the Id passed to the constructor
            AddClassDefinition(typeof(NewKingSelectionKingdomDecision), 8899174);
            AddClassDefinition(typeof(NewKingSelectionKingdomDecision.KingSelectionDecisionOutcome), 370202);

        }

        protected override void DefineBasicTypes()
        {

        }
    }
}
