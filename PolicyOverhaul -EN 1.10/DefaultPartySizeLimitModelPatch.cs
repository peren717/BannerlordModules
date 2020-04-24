using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using HarmonyLib;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultPartySizeLimitModel), "CalculateMobilePartyMemberSizeLimit")]
    class DefaultPartySizeLimitModelPatch
    {
        [HarmonyPostfix]
        private static void Postfix(MobileParty party, StatExplainer explanation, ref int __result)
        {
            if (party.LeaderHero != null && !party.IsCaravan)
            {
                if (party.MapFaction != null && party.MapFaction != party.LeaderHero.Clan)
                {
                    if (((Kingdom)party.MapFaction).ActivePolicies.Contains(NewPolicies.Feudalism) && party.LeaderHero == party.LeaderHero.Clan.Leader && party.MapFaction.Leader == party.LeaderHero)
                    {
                        __result += 50;
                        if (explanation != null)
                        {
                            explanation.AddLine(NewPolicies.Feudalism.Name.ToString(), 50f, StatExplainer.OperationType.Add);
                        }
                    }
                    if (((Kingdom)party.MapFaction).ActivePolicies.Contains(NewPolicies.WarFury))
                    {
                        int num = (int)party.LeaderHero.Clan.Influence;
                        if(num>500)
                        {
                            num = 500;
                        }
                        __result += num;
                        if (explanation != null)
                        {
                            explanation.AddLine(NewPolicies.WarFury.Name.ToString(), num, StatExplainer.OperationType.Add);
                        }
                    }
                    if (((Kingdom)party.MapFaction).ActivePolicies.Contains(NewPolicies.HouseOfLords) && ((Kingdom)party.MapFaction).RulingClan != party.LeaderHero.Clan)
                    {
                        if (explanation != null)
                        {
                            explanation.AddLine(NewPolicies.HouseOfLords.Name.ToString(), 20, StatExplainer.OperationType.Add);
                        }
                        __result += 20;

                    }
                    if (((Kingdom)party.MapFaction).ActivePolicies.Contains(NewPolicies.Tyrant) && party.MapFaction.Leader == party.LeaderHero)
                    {
                        __result += 50;
                        if (explanation != null)
                        {
                            explanation.AddLine(NewPolicies.Tyrant.Name.ToString(), 50, StatExplainer.OperationType.Add);
                        }
                    }

                }
            }

        }
    }
}
