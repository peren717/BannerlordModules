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
            NewPolicies.Abdicate = game.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject("policy_Abdicate"));



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
