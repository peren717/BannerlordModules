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
using TaleWorlds.Library;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultClanPoliticsModel), "CalculateInfluenceChange")]
    class ClanPoliticsModelPatch
    {
        [HarmonyPostfix]
        static void PostFix(ref float __result, Clan clan, ref StatExplainer explanation)
        {
            float num = __result;
            ExplainedNumber explainedNumber = new ExplainedNumber(num, explanation, null);
            try
            {
                if (explanation != null && explanation.Lines.Count > 0)
                {
                    explanation.Lines.Remove(explanation.Lines.Last<StatExplainer.ExplanationLine>());
                }
                if (clan.Kingdom != null && !clan.IsUnderMercenaryService)
                {

                    if (clan == clan.Kingdom.RulingClan)
                    {
                        if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.ConstitutionaMonarchy))
                        {
                            explainedNumber.Add(-(explainedNumber.ResultNumber + clan.Influence), NewPolicies.ConstitutionaMonarchy.Name);
                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CouncilOfTheCommons))
                    {
                        clan.Kingdom.RemovePolicy(DefaultPolicies.CouncilOfTheCommons);
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Feudalism))
                    {
                        explainedNumber.Add((float)clan.Tier - 3, NewPolicies.Feudalism.Name);
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.WarFury) && clan.Influence>0)
                    {
                        if(clan.Influence>0)
                        {
                            explainedNumber.Add(-(float)clan.Tier*0.5f, NewPolicies.WarFury.Name);
                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.HouseOfLords))
                    {
                        if(clan.Kingdom.RulingClan == clan)
                        {
                            explainedNumber.Add(-(float)clan.Kingdom.Clans.Count, NewPolicies.HouseOfLords.Name);
                        }
                        else
                        {
                            explainedNumber.Add(1f, NewPolicies.HouseOfLords.Name);
                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Republic))
                    {
                        if (clan.Kingdom.RulingClan != clan)
                        {
                            explainedNumber.Add(1f, NewPolicies.Republic.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(e.Message));
            }

            __result = explainedNumber.ResultNumber;
        }
    }
}
