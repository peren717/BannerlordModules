using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v1;
using MBOptionScreen.Settings;

namespace PolicyOverhaul
{
    class PolicyOverhaulSettings : AttributeSettings<PolicyOverhaulSettings>
    {
        public override string ModName => "政策大修";
        public override string ModuleFolderName => "PolicyOverhaul";
        public override string Id { get; set; } = "PolicyOverhaul";

        [SettingProperty("选举间隔", 5,500 , false, "选举间隔")]
        [SettingPropertyGroup("通用")]
        public int ElectionCycle { get; set; } = 30;

        [SettingProperty("领袖可否连任", 5, 500, false, "领袖可否连任")]
        [SettingPropertyGroup("通用")]
        public bool InfiniteTerm { get; set; } = false;

        [SettingProperty("互动频率", 0f, 1f, false, "领主之间互动频率，0为无互动，1为必定互动（会导致刷屏）")]
        [SettingPropertyGroup("通用")]
        public float ActProbablity { get; set; } = 0.2f;

        [SettingProperty("新游戏国家自带政策", 5, 500, false, "开启后，开始新游戏时，每个国家将会根据背景自带国策")]
        [SettingPropertyGroup("通用")]
        public bool EnablePreAssignedPolicies { get; set; } = true;

        [SettingProperty("Debug Mode", 5, 500, false, "Show debug message when enabled")]
        [SettingPropertyGroup("Debug")]
        public bool debugMode { get; set; } = false;
    }


}
