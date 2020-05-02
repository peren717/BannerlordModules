using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CommunityPatchPartySizeCompabilityPatch
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() 
        {
            base.OnSubModuleLoad();
            try
            {
                new Harmony("UniversityofUtah.peren717.CPPSCP").PatchAll();
            }
            catch
            {
                InformationManager.DisplayMessage(new InformationMessage("CommunityPatchPartySizeCompabilityPatch Not Loaded!", Colors.Red));
            }

        }

    }
}