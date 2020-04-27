using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace PolicyOverhaul
{
    class NewMarriageModel : DefaultMarriageModel
    {
        public override bool IsSuitableForMarriage(Hero maidenOrSuitor)
        {
            if (maidenOrSuitor.Clan != null && maidenOrSuitor.Clan.Kingdom != null)
            {
                if (maidenOrSuitor.Clan.Kingdom.ActivePolicies.Contains(NewPolicies.Polygamy))
                {
                    if (!maidenOrSuitor.IsNoble || maidenOrSuitor.IsTemplate)
                    {
                        return false;
                    }
                    if (maidenOrSuitor.IsFemale)
                    {
                        return maidenOrSuitor.CharacterObject.Age < (float)this.MaximumMarriageAgeFemale && maidenOrSuitor.CharacterObject.Age >= (float)this.MinimumMarriageAgeFemale;
                    }
                    return maidenOrSuitor.CharacterObject.Age < (float)this.MaximumMarriageAgeMale && maidenOrSuitor.CharacterObject.Age >= (float)this.MinimumMarriageAgeMale;
                }
                return base.IsSuitableForMarriage(maidenOrSuitor);
            }
            else
            {
                return base.IsSuitableForMarriage(maidenOrSuitor);
            }
        }
    }
}
