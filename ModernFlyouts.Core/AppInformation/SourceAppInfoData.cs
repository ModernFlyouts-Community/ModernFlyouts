using System;

namespace ModernFlyouts.Core.AppInformation
{
    public enum SourceAppInfoDataType
    {
        FromProcessId,
        FromAppUserModelId
    }

    public class SourceAppInfoData
    {
        public string AppUserModelId { get; init; }

        public IntPtr MainWindowHandle { get; init; }

        public uint ProcessId { get; init; }

        public SourceAppInfoDataType DataType { get; init; }
    }
}
