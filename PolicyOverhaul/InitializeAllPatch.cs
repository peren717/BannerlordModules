using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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
                new TextObject("君主立宪体制。", null),
                new TextObject("国家元首影响力每日结算时清零。", null), -0.9f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.ProfessionalArmy.Initialize(new TextObject("职业军人", null), new TextObject("职业军人就是以当兵为职业，效率大大高过募兵，但要国家要付更多薪水。", null),
                new TextObject("职业军人制。", null),
                new TextObject("待在城中的士兵每日训练经验翻倍。\n非驻扎的士兵每日工资翻倍。", null), -0.1f, -0.2f, 0.3f));
            __instance.Policies.Add(NewPolicies.Republic.Initialize(new TextObject("共和制", null), new TextObject("国家将由最有影响力的家族统领。", null),
                new TextObject("共和制。", null),
                new TextObject("每隔一段时间，国家元首将由贵族们选举产生。", null), -0.9f, -0.9f, 0.1f));
            __instance.Policies.Add(NewPolicies.Feudalism.Initialize(new TextObject("封建主义", null), new TextObject("封建等级制，附庸在封建时得到了领主的保护，举凡其生命、声誉、财产受到威胁时，领主有义务要保护。领主也不能对附庸有损害其权利的行为，否则附庸可以提出终止关系的要求。", null),
                new TextObject("封建主义。", null),
                new TextObject("家族将根据自身等级得到影响力调整 \n统治者家族首领部队上限增加50。 \n非统治者家族将很难得到声望", null), -0.9f, -0.9f, 0.1f));
            __instance.Policies.Add(NewPolicies.Vassalism.Initialize(new TextObject("附庸制", null), new TextObject("我附庸的附庸，不是我的附庸。", null),
                new TextObject("附庸制。", null),
                new TextObject("家族所属领地要人友好度固定为100。\n非家族所属领地要人友好度为0。", null), 0.5f, 0.5f, -0.9f));
            __instance.Policies.Add(NewPolicies.Polygamy.Initialize(new TextObject("重婚制", null), new TextObject("为什么不多娶几个老婆呢？", null),
                new TextObject("重婚制。", null),
                new TextObject("国家里的贵族可以同时与多个异性结婚 \n国家所有定居点安全每日减1", null), 0.0f, -0.8f, -0.9f));
            __instance.Policies.Add(NewPolicies.Slavery.Initialize(new TextObject("奴隶制", null), new TextObject("奴隶制，是指奴隶主拥有奴隶的制度。劳力活动须以奴隶为主，无报酬，且无人身自由。", null),
                new TextObject("奴隶制。", null),
                new TextObject("每有一个俘虏，定居点每日繁荣度加 0.1 \n每有一个俘虏，定居点每日安全减 0.1", null), 0.0f, 0.2f, -0.9f));
            __instance.Policies.Add(NewPolicies.WarFury.Initialize(new TextObject("以战养战", null), new TextObject("利用战争中获取来的人力、物力和财力，继续进行战争，以此来扩大战果。", null),
                new TextObject("以战养战。", null),
                new TextObject("每有一点影响力，就多一点部队上限(最多500)。 \n所有定居点繁荣度减5。\n所有定居点安全减5。", null), 0.5f, -0.5f, -0.1f));
            __instance.Policies.Add(NewPolicies.CouncilOfTheCommens.Initialize(new TextObject("民众大会制", null), new TextObject("权力下放，允许领地内的要人参与政治。", null),
                new TextObject("民众大会制。", null), new TextObject("每个要人每日消耗其领主0.2影响力 \n每个要人每日声望增加1", null), 0.0f, -0.1f, 0.4f));
            __instance.Policies.Add(NewPolicies.Centralization.Initialize(new TextObject("中央集权", null), new TextObject("中央集权是一种国家政权的制度，以国家职权统一于中央政府，削弱地方政府力量为标志。", null),
                new TextObject("中央集权制。", null),
                new TextObject("国家所有城镇收归于统治者家族。 \n统治者家族要向所有封臣每日发放俸禄。", null), 0.9f, -0.9f, -0.9f));



            return true;
        }

        [HarmonyPostfix]
        static void PostFix(DefaultPolicies __instance)
        {
            foreach (PolicyObject policyObject in __instance.Policies)
            {
                if (((PropertyObject)policyObject).Name.Equals("{民众大会}") || ((PropertyObject)policyObject).Name.Equals("{=bMSI9Bt3}Council of the Commons"))
                {
                    __instance.Policies.Remove(policyObject);
                    break;
                }
            }

        }



        static void addPolicy(DefaultPolicies instance, string policyName, string longDescription, string message, string effect, float authMod, float oligaMod, float egalMod)
        {
            instance.Policies.Add(NewPolicies.Feudalism.Initialize(new TextObject(policyName, null), new TextObject(longDescription, null), new TextObject(message, null), new TextObject(effect, null), authMod, oligaMod, egalMod));
        }


    }
}
