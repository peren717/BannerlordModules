using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PolicyOverhaul
{
    //[HarmonyPatch(typeof(KingdomManager), "HourlyTick")]
    //class KindomManagerHourlyTickPatch
    //{

    //    [HarmonyPostfix]
    //    static void PostFix(KingdomManager __instance, int ____hourCounter)
    //    {
    //        if (____hourCounter % 24 < 10)
    //        {
    //            int num = 0;
    //            foreach (Kingdom kingdom in Kingdom.All)
    //            {
    //                if (num == ____hourCounter % 24)
    //                {
    //                    if (kingdom.ActivePolicies.Contains(NewPolicies.SplitSystem))
    //                    {
    //                        RedistributeGold(kingdom);
    //                    }
    //                }
    //                num++;
    //            }
    //        }
    //    }


    //    static void RedistributeGold(Kingdom kingdom)
    //    {
    //        float total = 0;
    //        foreach (Clan clan in kingdom.Clans)
    //        {
    //            total += clan.Gold;
    //        }
    //        float dividedGold = total / kingdom.Clans.Count;
    //        foreach (Clan clan in kingdom.Clans)
    //        {
                
    //        }
    //    }
    //}


}
