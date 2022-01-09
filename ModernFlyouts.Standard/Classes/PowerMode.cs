using System;

namespace ModernFlyouts.Standard.Classes
{
    // Plans with 11 seem to be used only in Windows 11 
    // I switched to tuples because I hope to link it with slider position

    public static class PowerMode
    {
        public static Tuple<double, Guid> PowerSaver = new Tuple<double, Guid>(0, new Guid("a1841308-3541-4fab-bc81-f71556f20b4a"));

        public static Tuple<double, Guid> Recommended = new Tuple<double, Guid>(1, new Guid("381b4222-f694-41f0-9685-ff5bb260df2e"));

        public static Tuple<double, Guid> BetterPerformance = new Tuple<double, Guid>(2, new Guid("3af9B8d9-7c97-431d-ad78-34a8bfea439f"));

        public static Tuple<double, Guid> BestPerformance = new Tuple<double, Guid>(3, new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"));

        public static Tuple<double, Guid> Recommended11 = new Tuple<double, Guid>(4, new Guid("00000000-0000-0000-0000-000000000000"));

        public static Tuple<double, Guid> BestPerformance11 = new Tuple<double, Guid>(5, new Guid("ded574b5-45a0-4f42-8737-46345c09c238"));
    }
}
