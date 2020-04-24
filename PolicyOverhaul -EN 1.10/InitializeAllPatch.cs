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
            __instance.Policies.Add(NewPolicies.ConstitutionaMonarchy.Initialize(new TextObject("Constitutional Monarchy", null), new TextObject("The leader becomes only a visible symbol of national unity.", null), 
                new TextObject("constitutional monarchy.", null), new TextObject("The influence of the ruler clan is set to 0 when calculating their influence change.", null), -0.9f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.ProfessionalArmy.Initialize(new TextObject("Professional Soldier", null), 
                new TextObject("Enabling every soldier in the country to focus their time and energy on martial training.", null), 
                new TextObject("professional soldier.", null), new TextObject("Troops in the settlements recieve double XP from training.\nTroops in a mobile party recieve double wages.", null), -0.1f, -0.2f, 0.3f));
            __instance.Policies.Add(NewPolicies.Republic.Initialize(new TextObject("Republic", null), new TextObject("a state in which supreme power is held by the people and their elected representatives, and which has an elected or nominated president rather than a monarch.", null), 
                new TextObject("Republic", null), new TextObject("An election will be held to determine the next ruler. \nHas no effect when Feudal inheritance or Tyrant active.", null), -0.9f, -0.9f, 0.1f));
            __instance.Policies.Add(NewPolicies.Feudalism.Initialize(new TextObject("Feudalism", null), new TextObject("A set of reciprocal legal and military obligations which existed among the warrior nobility and revolved around the three key concepts of lords, vassals and fiefs.", null),
                new TextObject("Feudalism.", null),
                new TextObject("Clans recieve influence based on their tiers. \nRuler clan party limit +50. \nNon-ruler clan will get little influence", null), -0.9f, -0.9f, 0.1f));
            __instance.Policies.Add(NewPolicies.Vassalism.Initialize(new TextObject("Vassalism", null), new TextObject("The vassal's vassal is not my vassal.", null),
                new TextObject("Vassalism.", null),
                new TextObject("Notables of your owned settlement get relation boost to 100. \nYour relations with other notables are set to 0", null), 0.5f, 0.5f, -0.9f));
            __instance.Policies.Add(NewPolicies.Polygamy.Initialize(new TextObject("Polygamy", null), new TextObject("The practice or custom of having more than one wife or husband at the same time.", null),
                new TextObject("Polygamy.", null),
                new TextObject("You can marry more than one person. \nTown security is decreased by 1 per day.", null), 0.0f, -0.8f, -0.9f));
            __instance.Policies.Add(NewPolicies.Slavery.Initialize(new TextObject("Slavery", null), new TextObject("The practice or system of owning slaves.", null),
                new TextObject("Slavery", null),
                new TextObject("Settlement prosperity is increased by 0.1 per prisoner per day \nSettlement security is decreased by 0.1 per prisoner per day", null), 0.0f, 0.2f, -0.9f));
            __instance.Policies.Add(NewPolicies.WarFury.Initialize(new TextObject("Supporting War With War", null), new TextObject("Sustaining the war by means of war, fuel war with warfare.", null),
                new TextObject("Supporting war with war.", null),
                new TextObject("Party limit is increased by 1 per influence you have. \nSettlement prosperity decreased by 5 per day.\nSettlement security decreased by 5 per day.", null), 0.5f, -0.5f, -0.1f));
            __instance.Policies.Add(NewPolicies.CouncilOfTheCommens.Initialize(new TextObject("The Council of the Commons", null), new TextObject("Town notables will be involved in local politics.", null),
                new TextObject("The council of the commons.", null), new TextObject("Every notable costs their lord 0.2 influence per day \nNotable power increased by 1 per day", null), 0.0f, -0.1f, 0.4f));
            __instance.Policies.Add(NewPolicies.Centralization.Initialize(new TextObject("Centralism", null), new TextObject("The control of different activities and organizations under a single authority.", null),
                new TextObject("Centralism", null),
                new TextObject("All town ownership goes to the ruler clan. \nThe ruler clan pays wages to their subordinates.", null), 0.9f, -0.9f, -0.9f));
            __instance.Policies.Add(NewPolicies.HouseOfLords.Initialize(new TextObject("House of Lords", null), new TextObject("An alliance of nobles that limits the power of the ruler.", null),
                new TextObject("House of Lords.", null),
                new TextObject("Ruler influence decreased by 1 per clan in thier realm. \nNon-ruler clan party limit increased by 20. \nNon-ruler clan influence increased by 1 per day", null), -0.2f, 0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.NormadicHorde.Initialize(new TextObject("Normadic Tradition", null), new TextObject("Nomadic pastoralism is a form of pastoralism when livestock are herded in order to find fresh pastures on which to graze.", null),
                new TextObject("Normadic Tradition.", null),
                new TextObject("Party speed-5%. \nParty consumes no food.", null), 0.0f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.Tyrant.Initialize(new TextObject("Tyrant Rules", null),
                new TextObject("A tyrant as a person who rules without law, using extreme and cruel methods against both his own people and others.", null),
                new TextObject("Tyrant rule.", null),
                new TextObject("After every election cycle，the clan who has the most influence + party size limits becomes the ruler \nRuler clan party size limit +50.", null), 0.8f, -0.5f, 0.2f));



            return true;
        }

        [HarmonyPostfix]
        static void PostFix(DefaultPolicies __instance)
        {
            foreach (PolicyObject policyObject in __instance.Policies)
            {
                if (((PropertyObject)policyObject).Name.Equals("{Council of the Commons}") || ((PropertyObject)policyObject).Name.Equals("{=bMSI9Bt3}Council of the Commons"))
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
