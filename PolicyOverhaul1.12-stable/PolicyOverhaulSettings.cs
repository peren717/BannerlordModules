using ModLib;
using ModLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PolicyOverhaul
{
    class PolicyOverhaulSettings : SettingsBase
    {
        public override string ModName => "政策大修";
        public override string ModuleFolderName => "PolicyOverhaul";
        
        [XmlElement]
        public override string ID { get; set; } = "PolicyOverhaulSettings";

        public const string InstanceID = "PolicyOverhaulSettings";
        public static PolicyOverhaulSettings Instance
        {
            get
            {
                return (PolicyOverhaulSettings)SettingsDatabase.GetSettings("PolicyOverhaulSettings");
            }
        }

        [XmlElement]
        [SettingProperty("选举间隔", 5, 500, "选举间隔")]
        [SettingPropertyGroup("通用")]
        public int ElectionCycle { get; set; } = 30;

        [XmlElement]
        [SettingProperty("领袖可否连任", 5, 500, "领袖可否连任")]
        [SettingPropertyGroup("通用")]
        public bool InfiniteTerm { get; set; } = false;

        [XmlElement]
        [SettingProperty("互动频率", 0f, 1f, "领主之间互动频率，0为无互动，1为必定互动（会导致刷屏）")]
        [SettingPropertyGroup("通用")]
        public float ActProbablity { get; set; } = 0.2f;

        [XmlElement]
        [SettingProperty("新游戏国家自带政策", 5, 500, "开启后，开始新游戏时，每个国家将会根据背景自带国策")]
        [SettingPropertyGroup("通用")]
        public bool EnablePreAssignedPolicies { get; set; } = true;

        [XmlElement]
        [SettingProperty("Debug Mode", 5, 500, "Show debug message when enabled")]
        [SettingPropertyGroup("Debug")]
        public bool debugMode { get; set; } = false;
    }


}
