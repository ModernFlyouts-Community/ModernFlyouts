using System;
using System.Collections.Generic;
using System.Text;

namespace ModernFlyouts.Standard.Classes
{
    public class LayoutProperties
    {
        public bool IsAuto { get; set; }
        public AutoLayoutProperties AutoLayoutProps { get; set; }
        public ManualLayoutProperties ManualLayoutProps { get; set; }
    }
}
