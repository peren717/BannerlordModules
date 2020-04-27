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
    [HarmonyPatch(typeof(DefaultSettlementProsperityModel), "CalculateHearthChange")]
    class CalculateHearthChangePatch
    {
        [HarmonyPostfix]
        private static void Postfix(Village village, ref float __result, StatExplainer explanation = null)
        {
            float num = __result;
            ExplainedNumber explainedNumber = new ExplainedNumber(num, explanation, null);
            try
            {
                if (explanation != null && explanation.Lines.Count > 0)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (village.Settlement.OwnerClan.Kingdom != null)
                {
                    Kingdom kingdom = village.Settlement.OwnerClan.Kingdom;
                    if (kingdom.ActivePolicies.Contains(NewPolicies.PublicHealth))
                    {
                        if (village.VillageState == Village.VillageStates.Normal)
                        {
                            explainedNumber.Add(0.5f, NewPolicies.PublicHealth.Name);
                        }
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
