using ModernFlyouts.Standard.Classes;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ModernFlyouts.Standard.Services
{
    public class PowerPlanService
    {

        /// <summary>
        /// Retrieves the corresponding slider value for the current power scheme
        /// </summary>
        /// <returns>Returns a double representing slider position for the power scheme</returns>
        /// NOTE THIS CODE IS OUTDATED AFTER THE SWITCH TO TUPLES. TO DO: ADAPT TO TUPLE SYSTEM
        public static int GetPlanIndex()
        {
            PowerGetEffectiveOverlayScheme(out Guid currentMode);
            if (currentMode == PowerMode.PowerSaver.Item2)
            {
                return 0;
            }
            if (currentMode == PowerMode.Recommended.Item2 || currentMode == PowerMode.Recommended11.Item2)
            {
                return 1;
            }
            else if (currentMode == PowerMode.BetterPerformance.Item2)
            {
                return 2;
            }
            else if (currentMode == PowerMode.BestPerformance.Item2 || currentMode == PowerMode.BestPerformance11.Item2)
            {
                return 3;
            }
            else
            {
                return 1; //The Recommended power scheme will be set as default
            }
        }

        /// <summary>
        /// Retrieves the active overlay power scheme and returns a GUID that identifies the scheme.
        /// </summary>
        /// <returns>Returns a GUID of the active power scheme</returns>
        public static Guid GetCurrentPlan()
        {
            PowerGetEffectiveOverlayScheme(out Guid currentMode);
            return currentMode;
        }

        /// <summary>
        /// Sets a power scheme in the system from the GUID provided
        /// </summary>
        /// <param name="guid">A PowerPlan guid</param>
        public static void SetPowerPlan(Guid guid)
        {
            var process = new Process { StartInfo = _startInfo };
            process.Start();
            process.StandardInput.WriteLine("powercfg /SETACTIVE " + guid);
            process.StandardInput.WriteLine("exit");
            process.StandardOutput.ReadToEnd();
            process.Dispose();
        }

        /// <summary>
        /// Common StartInfo properties for a Process to hide the window
        /// </summary>
        private static readonly ProcessStartInfo _startInfo = new ProcessStartInfo("cmd")
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        /// <summary>
        /// Retrieves the active overlay power scheme and returns a GUID that identifies the scheme.
        /// </summary>
        /// <param name="EffectiveOverlayPolicyGuid">A pointer to a GUID structure.</param>
        /// <returns>Returns zero if the call was successful, and a nonzero value if the call failed.</returns>
        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        public static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayPolicyGuid);
    }
}
