using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace PolicyOverhaul
{
    [HarmonyPatch(typeof(DefaultPolicies), "InitializeAll")]
    class InitializeAllPatch
    {
        [HarmonyPrefix]
        static bool PreFix(DefaultPolicies __instance)
        {
            __instance.Policies.Add(NewPolicies.ConstitutionaMonarchy.Initialize(new TextObject("君主立宪", null), new TextObject("君主虽贵为国家的元首，却无法干预国家政策，不过以为装饰品，无丝毫实权，号为神圣，等于偶像。", null),
                new TextObject("确立了君主立宪体制，元首将无权干政。", null),
                new TextObject("国家元首影响力每日结算时清零。", null), -0.9f, -0.5f, 0.9f));

            //__instance.Policies.Add(NewPolicies.ProfessionalArmy.Initialize(new TextObject("职业军人", null), new TextObject("职业军人就是以当兵为职业，效率大大高过募兵，但要国家要付更多薪水。", null),
            //    new TextObject("开始实行职业军人制。", null),
            //    new TextObject("待在城中的士兵每日训练经验翻倍。\n国家元首影响力固定为0。", null), -0.9f, -0.5f, 0.9f));


            return true;
        }


    }
}
