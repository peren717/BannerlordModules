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
    [HarmonyPatch(typeof(DefaultSettlementProsperityModel), "CalculateProsperityChange")]
    class CalculateProsperityChangePatch
    {
        [HarmonyPostfix]
        private static void Postfix(Town fortification, ref float __result, StatExplainer explanation = null)
        {
            float num = __result;
            ExplainedNumber explainedNumber = new ExplainedNumber(num, explanation, null);
            try
            {
                if (explanation != null)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (fortification.Settlement.OwnerClan.Kingdom != null)
                {
                    Kingdom kingdom = fortification.Settlement.OwnerClan.Kingdom;
                    if (kingdom.ActivePolicies.Contains(NewPolicies.Slavery) && fortification.Settlement.Party != null & fortification.Settlement.Party.PrisonRoster != null && fortification.GarrisonParty != null && fortification.GarrisonParty.PrisonRoster != null)
                    {
                        int totalSlave = 0;
                        foreach (TroopRosterElement troopType in fortification.Settlement.Party.PrisonRoster)
                        {
                            totalSlave += troopType.Number;
                        }
                        foreach (TroopRosterElement troopType in fortification.GarrisonParty.PrisonRoster)
                        {
                            totalSlave += troopType.Number;
                        }
                        explainedNumber.Add(totalSlave * 0.1f, NewPolicies.Slavery.Name);
                    }
                    if (kingdom.ActivePolicies.Contains(NewPolicies.WarFury))
                    {
                        explainedNumber.Add(-3f, NewPolicies.WarFury.Name);
                    }
                    if (kingdom.ActivePolicies.Contains(NewPolicies.NormadicHorde))
                    {
                        explainedNumber.Add(-3f, NewPolicies.NormadicHorde.Name);
                    }
                    if (kingdom.ActivePolicies.Contains(NewPolicies.Physiocracy))
                    {
                        float physiNum = -fortification.Prosperity * 0.01f;
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
