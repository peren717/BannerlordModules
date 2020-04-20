using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;

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

                    if (clan == clan.Kingdom.RulingClan)
                    {
                        if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.ConstitutionaMonarchy))
                        {
                            explainedNumber.Add(-(explainedNumber.ResultNumber + clan.Influence), NewPolicies.ConstitutionaMonarchy.Name);
                        }
                    }

                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Abdicate))
                    {
                        if ((clan != clan.Kingdom.RulingClan) && (clan == GetMostInfluencialClan(clan.Kingdom)))
                        {
                            clan.Kingdom.RulingClan = clan;
                            InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "通过禅让成为了"+clan.Kingdom.ToString()+"的新领袖", Colors.Green));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("NewClanFinaceModel Error: " + e.Message);
            }

            return explainedNumber.ResultNumber;
        }

        static Clan GetMostInfluencialClan(Kingdom kingdom)
        {
            float zero = 0;
            float max = -1 / zero;
            Clan result = null;
            foreach (Clan clan in kingdom.Clans)
            {
                if (clan.Influence > max)
                {
                    result = clan;
                    max = clan.Influence;
                }
            }
            return result;
        }

    }
}
