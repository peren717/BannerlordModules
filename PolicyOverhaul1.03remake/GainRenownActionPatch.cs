using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(Clan), "AddRenown")]
    class GainRenownActionPatch
    {
        [HarmonyPrefix]
        static bool PreFix(Clan __instance, ref float value)
        {
            if(__instance.Kingdom != null)
            {
                if (__instance.Kingdom.ActivePolicies.Contains(NewPolicies.Feudalism))
                {
                    value = value/10;
                }
            }

            return true;
        }


    }
}
