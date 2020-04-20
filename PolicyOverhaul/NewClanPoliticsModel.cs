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
    class NewClanPoliticsModel : DefaultClanPoliticsModel
    {
        public override float CalculateInfluenceChange(Clan clan, StatExplainer explanation = null)
        {
            float num = base.CalculateInfluenceChange(clan, explanation);
            ExplainedNumber explainedNumber = new ExplainedNumber(num, explanation, null);
            try
            {
                if (explanation != null)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (clan.Kingdom != null && !clan.IsUnderMercenaryService)
                {
                    if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CouncilOfTheCommons))
                    {
                        int num2 = -(clan.Settlements.Sum((Settlement t) => t.Notables.Count));
                        explainedNumber.Add((clan.Settlements.Sum((Settlement t) => t.Notables.Count)) + (float)num2 * 0.2f, DefaultPolicies.CouncilOfTheCommons.Name);
                    }

                    if (clan==clan.Kingdom.RulingClan)
                    {
                        if(clan.Kingdom.ActivePolicies.Contains(NewPolicies.ConstitutionaMonarchy))
                        {
                            explainedNumber.Add(-(explainedNumber.ResultNumber+ clan.Influence), NewPolicies.ConstitutionaMonarchy.Name);
                        }
                    }
                    return explainedNumber.ResultNumber;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return explainedNumber.ResultNumber;
        }

    }
}
