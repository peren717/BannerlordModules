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
                    if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CouncilOfTheCommons))
                    {
                        clan.Kingdom.RemovePolicy(DefaultPolicies.CouncilOfTheCommons);
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.CouncilOfTheCommens))
                    {
                        int num2 = -(clan.Settlements.Sum((Settlement t) => t.Notables.Count));
                        explainedNumber.Add((float)num2 * 0.2f, DefaultPolicies.CouncilOfTheCommons.Name);
                        foreach (Settlement settlement in clan.Settlements)
                        {
                            foreach (Hero hero in settlement.Notables)
                            {
                                hero.AddPower(1);
                            }
                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Feudalism))
                    {
                        explainedNumber.Add((float)clan.Tier - 3, NewPolicies.Feudalism.Name);
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.HouseOfLords))
                    {
                        if(clan.Kingdom.RulingClan == clan)
                        {
                            explainedNumber.Add((float)clan.Kingdom.Clans.Count, NewPolicies.HouseOfLords.Name);
                        }
                        else
                        {
                            explainedNumber.Add(1f, NewPolicies.HouseOfLords.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(e.Message));
            }

            return explainedNumber.ResultNumber;
        }
    }
}
