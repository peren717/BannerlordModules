using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultSettlementSecurityModel), "CalculatePolicyEffectsOnSecurity")]
    class CalculatePolicyEffectOnSecurityPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Town town, ref ExplainedNumber explainedNumber)
        {
            Kingdom kingdom = town.Settlement.OwnerClan.Kingdom;
            if (kingdom != null)
            {
                if (kingdom.ActivePolicies.Contains(NewPolicies.Polygamy))
                {
                    explainedNumber.Add(-1f, NewPolicies.Polygamy.Name);
                }
                if (kingdom.ActivePolicies.Contains(NewPolicies.Polygamy))
                {
                    explainedNumber.Add(-5f, NewPolicies.WarFury.Name);
                }
                if (kingdom.ActivePolicies.Contains(NewPolicies.Slavery))
                {
                    int pn = town.Settlement.Party.PrisonRoster.Count + town.GarrisonParty.PrisonRoster.Count;
                    float num = pn *0.1f;
                    explainedNumber.Add(-num, NewPolicies.Slavery.Name);
                }

            }
        }

    }
}
