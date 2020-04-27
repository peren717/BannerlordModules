using System;
using HarmonyLib;
using MBOptionScreen.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    public class PartySpeedModelForPatrols
    {
        [HarmonyPostfix]
         static void Postfix(MobileParty mobileParty, StatExplainer explanation, ref float __result)
        {
            if (mobileParty.Party.LeaderHero != null)
            {
                if (mobileParty.Party.MapFaction != null && mobileParty.Party.MapFaction != mobileParty.Party.LeaderHero.Clan && mobileParty.MapFaction.IsKingdomFaction)
                {
                    if (((Kingdom)mobileParty.Party.MapFaction).ActivePolicies.Contains(NewPolicies.NormadicHorde))
                    {
                        float num = __result * 0.05f;
                        __result *= 0.95f;
                        if (explanation != null)
                        {
                            explanation.AddLine(NewPolicies.NormadicHorde.Name.ToString(), -num, StatExplainer.OperationType.Add);
                        }
                    }
                }
            }

        }

    }
}
