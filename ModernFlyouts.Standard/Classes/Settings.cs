using System;
using System.Collections.Generic;
using System.Text;

namespace ModernFlyouts.Standard.Classes
{
    public class Settings
    {
        public bool IsAudioflyoutEnabled { get; set; }
        public bool IsBrighnessEnabled { get; set; }
        public bool IsAirplanemodeEnabled { get; set; }
        public bool IsMixertrayEnabled { get; set; }
        public bool IsBatteryEnabled { get; set; }
        public BatteryProperties BatteryProps { get; set; }
        public bool IsLockkeysEnabled { get; set; }
        public LockkeysProperties LockkeysProps { get; set; }
        public PersonalisationProps PersonalisationProps { get; set; }
        public LayoutProperties LayoutProps { get; set; }
        public SettingsProperties SettingsProps { get; set; }
    }
}
