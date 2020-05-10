using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
    class PartyMoralePatch
    {
        static bool debug = PolicyOverhaulSettings.Instance.debugMode;
        
        [HarmonyPostfix]
        static void Postfix(ref float __result, MobileParty mobileParty, ref StatExplainer explanation)
        {
            try
            {
                if (mobileParty.LeaderHero != null && mobileParty.MapFaction.IsKingdomFaction)
                {
                    if (((Kingdom)mobileParty.MapFaction).ActivePolicies.Contains(NewPolicies.TrainedCivilian))
                    {
                        if(explanation!=null)
                        {
                            explanation.AddLine(NewPolicies.TrainedCivilian.Name.ToString(), 20, StatExplainer.OperationType.Add);
                        }
                        __result += 20;
                    }
                }
            }
            catch(Exception e)
            {
                if (debug)
                {
                    GameLog.Warn("MoralePatch Error:" + e.Message);
                }
            }

        }

    }
}
