using System;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultMobilePartyFoodConsumptionModel), "DoesPartyConsumeFood")]
    public class FoodConsumptionBehaviorPatch
    {
        [HarmonyPrefix]
        static bool Prefix(ref bool __result, MobileParty mobileParty)
        {
            if (mobileParty.LeaderHero != null && !mobileParty.IsCaravan)
            {
                if (mobileParty.MapFaction != null && mobileParty.MapFaction != mobileParty.LeaderHero.Clan)
                {
                    if (((Kingdom)mobileParty.MapFaction).ActivePolicies.Contains(NewPolicies.NormadicHorde))
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
