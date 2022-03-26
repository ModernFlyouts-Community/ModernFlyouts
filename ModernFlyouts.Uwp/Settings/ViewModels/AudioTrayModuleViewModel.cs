using System;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ModernFlyouts.Settings.ViewModels
{
    public class AudioTrayModuleViewModel : ObservableObject
    {
        public AudioTrayModuleViewModel()
        {
        }
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));

                    throw new NotImplementedException();
                    //// Set the status of ColorPicker in the general settings
                    //GeneralSettingsConfig.Enabled.ColorPicker = value;
                    //var outgoing = new OutGoingGeneralSettings(GeneralSettingsConfig);

                    //SendConfigMSG(outgoing.ToString());
                }
            }
        }
    }
}
