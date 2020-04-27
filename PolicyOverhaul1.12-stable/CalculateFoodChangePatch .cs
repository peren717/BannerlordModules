using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultSettlementFoodModel), "CalculateTownFoodStocksChange")]
    class CalculateFoodChangePatch
    {
        [HarmonyPostfix]
        private static void Postfix(Town town, ref float __result, StatExplainer explanation = null)
        {
            float num = __result;
            ExplainedNumber explainedNumber = new ExplainedNumber(num, explanation, null);
            try
            {
                if (explanation != null)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (town.Settlement.OwnerClan.Kingdom != null)
                {
                    Kingdom kingdom = town.Settlement.OwnerClan.Kingdom;
                    if (kingdom.ActivePolicies.Contains(NewPolicies.Tuntian) && town.Settlement.Party != null && town.GarrisonParty != null)
                    {
                        float totalTroop = 0f;
                        foreach (TroopRosterElement troopType in town.Settlement.Party.MemberRoster)
                        {
                            totalTroop += troopType.Number;
                        }
                        foreach (TroopRosterElement troopType in town.GarrisonParty.MemberRoster)
                        {
                            totalTroop += troopType.Number;
                        }

                        explainedNumber.Add(totalTroop * 0.1f, NewPolicies.Tuntian.Name);
                    }
                    if (kingdom.ActivePolicies.Contains(NewPolicies.Physiocracy) && town.Settlement.Party != null && town.GarrisonParty != null)
                    {
                        float physiNum = town.Prosperity * 0.01f;
                        explainedNumber.Add(physiNum, NewPolicies.Physiocracy.Name);
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
