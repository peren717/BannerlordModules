using HarmonyLib;
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
    [HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculateClanGoldChange")]
    class DefaultClanFinanceModelPatch
    {
        [HarmonyPostfix]
        private static void PostFix(ref float __result, Clan clan, ref StatExplainer explanation)
        {
            float result = __result;
            ExplainedNumber explainedNumber = new ExplainedNumber(result, explanation, null);
            try
            {
                if (explanation != null && explanation.Lines.Count > 0)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (clan.Kingdom != null)
                {
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Centralization))
                    {
                        if (clan == Clan.PlayerClan )
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
                                        explainedNumber.Add(-wage, new TextObject(otherClan.Name + "{=wage_give}'s wages"));
                                    }
                                }
                            }
                            else if(!clan.IsUnderMercenaryService)
                            {
                                float wage = clan.Tier * 500;
                                if (clan.Influence > 0)
                                {
                                    wage += clan.Influence;
                                }
                                explainedNumber.Add(wage, new TextObject("{=wage_get}Wages"));
                            }

                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.PublicHealth))
                    {
                        float totalVillagers = 0;
                        foreach (Village village in clan.Villages)
                        {
                            if (village.VillageState == Village.VillageStates.Normal)
                            {
                                totalVillagers += village.Hearth;
                            }
                        }
                        explainedNumber.Add(-totalVillagers * 0.5f, NewPolicies.PublicHealth.Name);
                    }

                }

            }
            catch
            {

            }
            __result = explainedNumber.ResultNumber;
        }

    }
}
