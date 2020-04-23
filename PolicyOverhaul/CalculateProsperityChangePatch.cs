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
                    if (kingdom.ActivePolicies.Contains(NewPolicies.Slavery))
                    {
                        float pn = 0;
                        pn = fortification.Settlement.Party.PrisonRoster.Count + fortification.GarrisonParty.PrisonRoster.Count;
                        pn*=0.1f;
                        explainedNumber.Add(pn, NewPolicies.Slavery.Name);
                    }
                    if (kingdom.ActivePolicies.Contains(NewPolicies.WarFury))
                    {
                        explainedNumber.Add(-5f, NewPolicies.WarFury.Name);
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
