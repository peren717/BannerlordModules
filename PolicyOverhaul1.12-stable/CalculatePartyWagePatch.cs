using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculatePartyWage")]
    class CalculatePartyWagePatch
    {
        static readonly bool debug = PolicyOverhaulSettings.Instance.debugMode;

        [HarmonyPostfix]
        public static void PostFix(ref int __result, MobileParty mobileParty, bool applyWithdrawals)
        {

            try
            {
                if (mobileParty.Party.Owner.Clan.Kingdom != null)
                {
                    if (mobileParty.Party.Owner.Clan.Kingdom.ActivePolicies.Contains(NewPolicies.ProfessionalArmy))
                    {
                        __result = 2 * __result;
                    }
                }
            }
            catch (Exception e)
            {
                if(debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage(e.Message));
                }
            }
        }

    }
}
