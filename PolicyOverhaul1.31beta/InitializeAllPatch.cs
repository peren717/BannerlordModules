﻿using HarmonyLib;
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
                new TextObject("每隔一段时间，国家元首将由贵族们选举产生。\n如果国家存在僭主制或封建世袭，将无法进行选举。", null), -0.9f, 0.1f, 0.5f));
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
                new TextObject("每有一点影响力，就多一点部队上限(最多500)。 \n所有定居点繁荣度减3。\n所有定居点安全减3。", null), 0.5f, -0.5f, -0.1f));
            __instance.Policies.Add(NewPolicies.CouncilOfTheCommens.Initialize(new TextObject("民众大会制", null), new TextObject("权力下放，允许领地内的要人参与政治。", null),
                new TextObject("民众大会制。", null), new TextObject("每个要人每日消耗其领主0.2影响力 \n每个要人每日声望增加1", null), 0.0f, -0.1f, 0.4f));
            __instance.Policies.Add(NewPolicies.Centralization.Initialize(new TextObject("中央集权", null), new TextObject("中央集权是一种国家政权的制度，以国家职权统一于中央政府，削弱地方政府力量为标志。", null),
                new TextObject("中央集权制。", null),
                new TextObject("国家所有城镇收归于统治者家族。 \n统治者家族要向所有封臣每日发放俸禄。", null), 0.9f, -0.9f, -0.9f));
            __instance.Policies.Add(NewPolicies.HouseOfLords.Initialize(new TextObject("贵族联盟", null), new TextObject("所有拥有直接财产的贵族的权力相当，并掌控强大的权利和特权。", null),
                new TextObject("贵族联盟。", null),
                new TextObject("国家每有一个贵族，统治者影响力减1 \n非统治者家族部队上限加20，每日影响力加1", null), -0.2f, 0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.NormadicHorde.Initialize(new TextObject("游牧传统", null), new TextObject("军队行进过程中将进行狩猎，畜养。", null),
                new TextObject("游牧传统。", null),
                new TextObject("所有城镇每日繁荣度-3 \n军队将会在行军过程中不会消耗食物。", null), 0.0f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.Tyrant.Initialize(new TextObject("僭主制", null), new TextObject("地方军事将领越过元老院和公民大会的权威，国家将由最有影响力的军事将领所领导。", null),
                new TextObject("僭主制。", null),
                new TextObject("共和制将不会生效 \n每个选举周期结束后，国家将由影响力加部队上限最高者领导（家族全部军队）。 \n统治者部队上限增加50。", null), 0.8f, -0.5f, 0.2f));
            //__instance.Policies.Add(NewPolicies.Monotheism.Initialize(new TextObject("mo一神论", null), new TextObject("一神论，认为只存在一个神的信仰。", null),
            //    new TextObject("一神论。", null),
            //    new TextObject("", null), 0f, .05f, -0.5f));
            //__instance.Policies.Add(NewPolicies.polytheism.Initialize(new TextObject("po多神论", null), new TextObject("对于一神论或一神教而言，指崇拜或信仰许多神的信仰体系或者宗教教条。", null),
            //    new TextObject("多神论。", null),
            //    new TextObject("", null), 0.5f, -0.5f, 0f));
            //__instance.Policies.Add(NewPolicies.atheism.Initialize(new TextObject("at无神论", null), new TextObject("一种否认、否定、不相信神明存在的信念。", null),
            //    new TextObject("无神论。", null),
            //    new TextObject("", null), -0.5f, 0f, 0.5f));
            __instance.Policies.Add(NewPolicies.Tuntian.Initialize(new TextObject("屯田", null), new TextObject("城堡里的士兵在闲暇之余将从事农业生产。", null),
                new TextObject("屯田。", null),
                new TextObject("职业军人制将不会生效 \n每有一名士兵，城堡的粮食产量加0.1 \n城堡内驻军只能得到半数训练经验", null), 0.1f, -0.1f, 0.1f));
            __instance.Policies.Add(NewPolicies.Physiocracy.Initialize(new TextObject("重农抑商", null), new TextObject("国家财富的根本来源为土地生产及土地发展，我们当大力发展农业，打击投机倒把者。", null),
                new TextObject("重农抑商。", null),
                new TextObject("城镇每日将把1%的繁荣度转化为粮食。", null), 0.1f, -0.1f, 0.1f));
            __instance.Policies.Add(NewPolicies.PublicHealth.Initialize(new TextObject("公众卫生", null), new TextObject("国家将使用税收来防治疾病，强化公众健康。", null),
                new TextObject("公众卫生。", null),
                new TextObject("领主每日收入将根据其拥有乡村户数减少。\n 每个户数每日花费0.5个金币。 \n城镇及乡村户数每日加0.5。", null), 0.1f, -0.1f, 0.1f));
            __instance.Policies.Add(NewPolicies.BigCaravan.Initialize(new TextObject("大型商队", null), new TextObject("为了保护我们的商队，必须增加其人手以放不测。", null),
                new TextObject("大型商队。", null),
                new TextObject("商队规模上限+50。\n商队可能因规模过大而行动减缓。", null), 0.1f, -0.1f, 0.1f));



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