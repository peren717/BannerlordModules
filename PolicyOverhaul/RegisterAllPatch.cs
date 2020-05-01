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
using TaleWorlds.Localization;

namespace PolicyOverhaul
{

    [HarmonyPatch(typeof(DefaultPolicies), "RegisterAll")]
    public class RegisterPolicyPatch
    {
        [HarmonyPrefix]
        static bool PreFix(DefaultPolicies __instance, Game game)
        {
            NewPolicies.ConstitutionaMonarchy = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("policy_constitutionaMonarchy"));
            NewPolicies.ProfessionalArmy = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("policy_professionalArmy"));
            NewPolicies.Republic = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("policy_Abdicate"));
            NewPolicies.Feudalism = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("Feudailism"));
            NewPolicies.Vassalism = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("vassalism"));
            NewPolicies.Polygamy = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("polygamy"));
            NewPolicies.Slavery = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("slavery"));
            NewPolicies.WarFury = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("WarFury"));
            NewPolicies.CouncilOfTheCommens = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("CouncilOfTheCommens"));
            NewPolicies.Centralization = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("Centralization"));
            NewPolicies.HouseOfLords = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("HouseOfLords"));
            NewPolicies.NormadicHorde = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("NormadicHorde"));
            NewPolicies.Tyrant = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("Tyrant"));
            NewPolicies.Tuntian = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("Tuntian"));
            NewPolicies.Physiocracy = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("Physiocracy"));
            NewPolicies.PublicHealth = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("PublicHealth"));
            NewPolicies.BigCaravan = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("BigCaravan"));











            return true;
        }

        private static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                MessageBox.Show(__exception.Message);
            }
        }
    }









}
