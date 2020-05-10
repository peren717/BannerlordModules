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
                if (kingdom.ActivePolicies.Contains(NewPolicies.WarFury))
                {
                    explainedNumber.Add(-2f, NewPolicies.WarFury.Name);
                }
                if (kingdom.ActivePolicies.Contains(NewPolicies.Slavery))
                {
                    if(town.Settlement.Party!=null & town.Settlement.Party.PrisonRoster!=null && town.GarrisonParty != null && town.GarrisonParty.PrisonRoster != null)
                    {
                        int totalSlave = 0;
                        foreach(TroopRosterElement troopType in town.Settlement.Party.PrisonRoster)
                        {
                            totalSlave += troopType.Number;
                        }
                        foreach (TroopRosterElement troopType in town.GarrisonParty.PrisonRoster)
                        {
                            totalSlave += troopType.Number;
                        }
                        float num = totalSlave * 0.1f;
                        explainedNumber.Add(-num, NewPolicies.Slavery.Name);
                    }
                }

            }
        }

    }
}
