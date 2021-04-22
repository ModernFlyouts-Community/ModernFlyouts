namespace ModernFlyouts
{
    public enum FlyoutTriggerType
    {
        Volume,
        Media,
        AirplaneMode,
        Brightness
    }

    public class FlyoutTriggerData
    {
        public FlyoutTriggerType TriggerType { get; set; }

        public object Data { get; set; }

        public bool IsExpired;
    }
}
