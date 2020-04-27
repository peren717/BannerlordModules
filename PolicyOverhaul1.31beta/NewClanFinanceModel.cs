using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace PolicyOverhaul
{
    class NewClanFinanceModel : DefaultClanFinanceModel
    {
        public override float CalculateClanGoldChange(Clan clan, StatExplainer explanation = null, bool applyWithdrawals = false)
        {
            float result = base.CalculateClanGoldChange(clan, explanation, applyWithdrawals);
            ExplainedNumber explainedNumber = new ExplainedNumber(result, explanation, null);
            try
            {
                if (explanation != null)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (clan.Kingdom != null)
                {
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Centralization))
                    {
                        this.CalculateKingdomWagesForPlayer(clan, ref explainedNumber, applyWithdrawals);
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.PublicHealth))
                    {
                        float totalVillagers = 0;
                        foreach(Village village in clan.Villages)
                        {
                            if(village.VillageState == Village.VillageStates.Normal)
                            {
                                totalVillagers += village.Hearth;
                            }
                        }
                        explainedNumber.Add(-totalVillagers*0.5f, new TextObject("公共卫生"));
                    }

                }

            }
            catch
            {

            }
            return explainedNumber.ResultNumber;
        }

        public override int CalculatePartyWage(MobileParty mobileParty, bool applyWithdrawals)
        {

            int num = base.CalculatePartyWage(mobileParty, applyWithdrawals);
            try
            {
                if (mobileParty.Party.Owner.Clan.Kingdom != null)
                {
                    if (mobileParty.Party.Owner.Clan.Kingdom.ActivePolicies.Contains(NewPolicies.ProfessionalArmy))
                    {
                        num = 2 * num;
                    }
                }
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(e.Message));
            }
            return num;
        }



        public void CalculateKingdomWagesForPlayer(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false)
        {
            if (clan == Clan.PlayerClan)
            {
                if (clan == clan.Kingdom.RulingClan)
                {
                    foreach (Clan otherClan in clan.Kingdom.Clans)
                    {
                        if (otherClan != clan)
                        {
                            float wage = otherClan.Tier * 500;
                            if (otherClan.Influence > 0)
                            {
                                wage += otherClan.Influence;
                            }
                            goldChange.Add(-wage, new TextObject(otherClan.Name + "的俸禄"));
                        }
                    }
                }
                else
                {
                    float wage = clan.Tier;
                    if(clan.Influence>0)
                    {
                        wage += clan.Influence;
                    }
                    goldChange.Add(wage, new TextObject("俸禄"));
                }
                
            }
        }

    }
}
