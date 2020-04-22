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
            __instance.Policies.Add(NewPolicies.ConstitutionaMonarchy.Initialize(new TextObject("Constitutional Monarchy", null), new TextObject("The leader becomes only a visible symbol of national unity.", null),
                new TextObject("constitutional monarchy。", null),
                new TextObject("The influence of the ruling family is set to 0 when calculating their influence change.", null), -0.9f, -0.5f, 0.2f));

            __instance.Policies.Add(NewPolicies.ProfessionalArmy.Initialize(new TextObject("Professional Soldier", null), new TextObject("Enabling every soldier in the country to focus their time and energy on martial training.", null),
                new TextObject("professional soldier.", null),
                new TextObject("Troops in the settlements recieve double XP from training.\nTroops in a mobile party recieve double wages.", null), -0.1f, -0.2f, 0.3f));
            __instance.Policies.Add(NewPolicies.Abdicate.Initialize(new TextObject("Republic", null),
                new TextObject("a state in which supreme power is held by the people and their elected representatives, and which has an elected or nominated president rather than a monarch.", null),
                new TextObject("Republic", null),
                new TextObject("Every 30 days, an election will be held to determine the next ruler.", null), -0.9f, -0.9f, 0.1f));


            return true;
        }


    }
}
