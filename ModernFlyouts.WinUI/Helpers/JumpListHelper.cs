using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.StartScreen;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ModernFlyouts.WinUI.Helpers
{
    public class JumpListHelper
    {
        public static async Task CreateJumpListAsync()
        {
            if (!JumpList.IsSupported()) return;

            try
            {
                //var jumpList = await JumpList.LoadCurrentAsync();
                JumpList jumpList = await JumpList.LoadCurrentAsync();
                jumpList.Items.Clear();
                jumpList.SystemGroupKind = JumpListSystemGroupKind.None;
                string packageId = Package.Current.Id.Name;

                foreach (string jumpitem in new[] { "Settings", "RestoreDefault", "Exit"})
                {
                    var item = JumpListItem.CreateWithArguments(jumpitem, $"ms-resource://{packageId}/Resources/{jumpitem}NavItem/Content");
                    item.GroupName = $"ms-resource://{packageId}/Resources/JumpListGroupColors";
                    item.Logo = new Uri($"ms-appx:///Assets/JumpList/{jumpitem}.png");
                    jumpList.Items.Add(item);
                }

                //var item = JumpListItem.CreateWithArguments(Arguments.Text, DisplayName.Text);
                //item.Description = "ms-resource:///Resources/CustomJumpListDescription";
                //item.GroupName = "ms-resource:///Resources/CustomJumpListGroupName";
                //item.Logo = new Uri("ms-appx:///Assets/SmallTile.scale-100.png");
               // jumpList.Items.Add(item);

                await jumpList.SaveAsync();
            }

            catch { }

        }

    }
}