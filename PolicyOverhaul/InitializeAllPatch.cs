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
            __instance.Policies.Add(NewPolicies.ConstitutionaMonarchy.Initialize(new TextObject("{=ConstitutionaMonarchy_name}Constitutional Monarchy", null), new TextObject("{=ConstitutionaMonarchy_desc}The leader becomes only a visible symbol of national unity.", null),
                new TextObject("{=ConstitutionaMonarchy_desc}Constitutional Monarchy.", null),
                new TextObject("{=ConstitutionaMonarchy_effect}The influence of the ruler clan is set to 0 everyday.", null), -0.9f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.ProfessionalArmy.Initialize(new TextObject("{=ProfessionalArmy_name}Professional Soldiers", null), new TextObject("{=ProfessionalArmy_desc}Enabling every soldier to focus their time and energy on martial training.", null),
                new TextObject("{=ProfessionalArmy_name}Professional Soldiers.", null),
                new TextObject("{=ProfessionalArmy_effect}Troops in the settlements recieve double XP from training.\nTroops in a mobile party recieve double wages.", null), -0.1f, -0.2f, 0.3f));
            __instance.Policies.Add(NewPolicies.Republic.Initialize(new TextObject("{=Republic_name}Republic", null), new TextObject("{=Republic_desc}A state in which supreme power is held by the elected representatives, and which has an elected or nominated president rather than a monarch.", null),
                new TextObject("{=Republic_name}Republic.", null),
                new TextObject("{=Republic_effect}An election will be held to determine the next ruler. \nHas no effect when Feudal inheritance or Tyrant active. \nNonruler clan gain 1 influence per day", null), -0.9f, 0.1f, 0.5f));
            __instance.Policies.Add(NewPolicies.Feudalism.Initialize(new TextObject("{=Feudalism_name}Feudalism", null), new TextObject("{=Feudalism_desc}A set of reciprocal legal and military obligations which existed among the warrior nobility and revolved around the three key concepts of lords, vassals and fiefs.", null),
                new TextObject("{=Feudalism_name}Feudalism.", null),
                new TextObject("{=Feudalism_effect}Clans recieve influence based on their tiers. \nRuler clan party limit +50. \nNon-ruler clan will get little renown.", null), -0.9f, -0.9f, 0.1f));
            __instance.Policies.Add(NewPolicies.Vassalism.Initialize(new TextObject("{=Vassalism_name}Vassalism", null), new TextObject("{=Vassalism_desc}The vassal's vassal is not my vassal.", null),
                new TextObject("{=Vassalism_name}Vassalism.", null),
                new TextObject("{=Vassalism_effect}Notables of your owned settlement get relation boost to 100. \nYour relations with other notables are set to 0.", null), 0.5f, 0.5f, -0.9f));
            __instance.Policies.Add(NewPolicies.Polygamy.Initialize(new TextObject("{=Polygamy_name}Polygamy", null), new TextObject("{=Polygamy_desc}The practice or custom of having more than one wife or husband at the same time.", null),
                new TextObject("{=Polygamy_name}Polygamy.", null),
                new TextObject("{=Polygamy_effect}Nobles can marry more than one person. \nTown security is decreased by 1 per day.", null), 0.0f, -0.8f, -0.9f));
            __instance.Policies.Add(NewPolicies.Slavery.Initialize(new TextObject("{=Slavery_name}Slavery", null), new TextObject("{=Slavery_desc}The practice or system of owning slaves.", null),
                new TextObject("{=Slavery_name}Slavery.", null),
                new TextObject("{=Slavery_effect}Settlement prosperity is increased by 0.1 per prisoner per day \nSettlement security is decreased by 0.1 per prisoner per day", null), 0.0f, 0.2f, -0.9f));
            __instance.Policies.Add(NewPolicies.WarFury.Initialize(new TextObject("{=WarFury_name}Supporting War With War", null), new TextObject("{=WarFury_desc}Sustaining the war by means of war, fuel war with warfare.", null),
                new TextObject("{=WarFury_name}Supporting War With War.", null),
                new TextObject("{=WarFury_effect}Party limit is increased by 1 per influence you have.(max 200) \nSettlement prosperity decreased by 3 per day.\nSettlement security decreased by 3 per day.\nConsumes 1 influence per day per clan tier.", null), 0.5f, -0.5f, -0.1f));
            __instance.Policies.Add(NewPolicies.CouncilOfTheCommens.Initialize(new TextObject("{=CouncilOfTheCommens_name}The Council of the Commons", null), new TextObject("{=CouncilOfTheCommens_desc}Town notables will be involved in local politics.", null),
                new TextObject("{=CouncilOfTheCommens_name}The Council of the Commons.", null), new TextObject("{=CouncilOfTheCommens_effect}Every notable costs their lord 0.2 influence per day. \nNotable power increased by 1 per day.", null), 0.0f, -0.1f, 0.4f));
            __instance.Policies.Add(NewPolicies.Centralization.Initialize(new TextObject("{=Centralization_name}Centralism", null), new TextObject("{=Centralization_desc}The control of different activities and organizations under a single authority.", null),
                new TextObject("{=Centralization_name}Centralism.", null),
                new TextObject("{=Centralization_effect}All town ownership goes to the ruler clan. \nCastle ownership will be changed after every election cycle.\nThe ruler clan pays wages to their subordinates.", null), 0.9f, -0.9f, -0.9f));
            __instance.Policies.Add(NewPolicies.HouseOfLords.Initialize(new TextObject("{=HouseOfLords_name}House of Lords", null), new TextObject("{=HouseOfLords_desc}An alliance of nobles that limits the power of the ruler.", null),
                new TextObject("{=HouseOfLords_name}House of Lords.", null),
                new TextObject("{=HouseOfLords_effect}Ruler influence decreased by 1 per clan in thier realm. \nNon-ruler clan party limit increased by 20. \nNon-ruler clan influence increased by 1 per day", null), -0.2f, 0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.NormadicHorde.Initialize(new TextObject("{=NormadicHorde_name}Normadic Tradition", null), new TextObject("{=NormadicHorde_desc}Nomadic pastoralism is a form of pastoralism when livestock are herded in order to find fresh pastures on which to graze.", null),
                new TextObject("{=NormadicHorde_name}Normadic Tradition.", null),
                new TextObject("{=NormadicHorde_effect}Hearth change decreased by 0.5 per day. \nTown prosperity decreased by 2 per day. \nParty consumes no food.", null), 0.0f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.Tyrant.Initialize(new TextObject("{=Tyrant_name}Military Government", null), new TextObject("{=Tyrant_desc}A military government is generally any government that is administrated by military forces.", null),
                new TextObject("{=Tyrant_name}Military Government.", null),
                new TextObject("{=Tyrant_effect}After every election cycle, the clan who has the most influence + party size limits becomes the ruler \nRuler clan party size limit +50.", null), 0.8f, -0.5f, 0.2f));
            __instance.Policies.Add(NewPolicies.Tuntian.Initialize(new TextObject("{=Tuntian_name}Peasant Army", null), new TextObject("{=Tuntian_desc}Army will be used to produce food during peace time.", null),
                new TextObject("{=Tuntian_name}Peasant Army", null),
                new TextObject("{=Tuntian_effect}Professional Army has no effect when this is active. \nEvery troop in castle increases its food production by 0.1 per day.", null), 0.1f, -0.1f, 0.1f));
            __instance.Policies.Add(NewPolicies.Physiocracy.Initialize(new TextObject("{=Physiocracy_name}Physiocracy", null), new TextObject("{=Physiocracy_desc}The wealth of nations derived solely from the value of land agriculture and that agricultural products should be highly priced.", null),
                new TextObject("{=Physiocracy_name}Physiocracy.", null),
                new TextObject("{=Physiocracy_effect}Town converts 1% of its prosperity to food production per day.", null), 0.1f, -0.1f, 0.1f));
            __instance.Policies.Add(NewPolicies.PublicHealth.Initialize(new TextObject("{=PublicHealth_name}Public Health", null), new TextObject("{=PublicHealth_desc}Prolonging life and improving quality of life through organized efforts and informed choices of society.", null),
                new TextObject("{=PublicHealth_name}Public Health.", null),
                new TextObject("{=PublicHealth_effect}Income decreased by 0.5 per hearth per day.\nHearth increases by 0.5 per day.", null), 0.1f, -0.1f, 0.1f));
            __instance.Policies.Add(NewPolicies.BigCaravan.Initialize(new TextObject("{=BigCaravan_name}Armed Caravans", null), new TextObject("{=BigCaravan_desc}Arm our caravans with enough military power to fight agaist bandits and other outlawas.", null),
                new TextObject("{=BigCaravan_name}Armed Caravan.", null),
                new TextObject("{=BigCaravan_effect}Caravan size limit increases by 50. \nCaravan speed may decrease due to its large size.", null), 0.1f, -0.1f, 0.1f));



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
