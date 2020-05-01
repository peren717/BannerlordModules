using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultDailyTroopXpBonusModel), "CalculateGarrisonXpBonusMultiplier")]
    class DailyTroopXpBonusModelPatch
    {
        static readonly bool debug = PolicyOverhaulSettings.Instance.debugMode;

        [HarmonyPostfix]
        static void PostFix(ref float __result, Town town, ref StatExplainer explanation)
        {
            float num = __result;
            try
            {
                if (town.Settlement.OwnerClan.Kingdom.HasPolicy(NewPolicies.ProfessionalArmy))
                {
                    num = 2 * num;
                }
                if (town.Settlement.OwnerClan.Kingdom.HasPolicy(NewPolicies.Tuntian))
                {
                    num = 0.5f * num;
                }
            }
            catch (Exception e)
            {
                if(debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage(e.Message));
                }
            }
            __result = num;

        }
    }
}
