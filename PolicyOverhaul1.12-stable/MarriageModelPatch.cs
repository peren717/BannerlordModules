using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultMarriageModel), "IsSuitableForMarriage")]
    class MarriageModelPatch
    {
        [HarmonyPrefix]
        static bool Prefix(DefaultMarriageModel __instance, ref bool __result, Hero maidenOrSuitor)
        {
            if (maidenOrSuitor.Clan != null && maidenOrSuitor.Clan.Kingdom != null)
            {
                if (maidenOrSuitor.Clan.Kingdom.ActivePolicies.Contains(NewPolicies.Polygamy))
                {
                    if (!maidenOrSuitor.IsNoble || maidenOrSuitor.IsTemplate)
                    {
                        return true;
                    }
                    if (maidenOrSuitor.IsFemale)
                    {
                        __result = maidenOrSuitor.CharacterObject.Age < (float)__instance.MaximumMarriageAgeFemale && maidenOrSuitor.CharacterObject.Age >= (float)__instance.MinimumMarriageAgeFemale;
                    }
                    __result= maidenOrSuitor.CharacterObject.Age < (float)__instance.MaximumMarriageAgeMale && maidenOrSuitor.CharacterObject.Age >= (float)__instance.MinimumMarriageAgeMale;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
    }
}
