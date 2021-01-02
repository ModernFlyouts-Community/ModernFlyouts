#include <Windows.h>
#include <string>
#include <Shlwapi.h>
#pragma comment(lib, "shlwapi.lib")

void InjectDll(HANDLE hProcess, HANDLE hThread, const std::wstring& path)
{
    size_t strSize = (path.size() + 1) * sizeof(WCHAR);
    LPVOID pBuf = VirtualAllocEx(hProcess, 0, strSize, MEM_COMMIT, PAGE_READWRITE);
    if (pBuf == NULL)
        return;

    SIZE_T written;
    if (!WriteProcessMemory(hProcess, pBuf, path.c_str(), strSize, &written))
        return;

    LPVOID pLoadLibraryW = GetProcAddress(GetModuleHandle(L"kernel32"), "LoadLibraryW");
    QueueUserAPC((PAPCFUNC)pLoadLibraryW, hThread, (ULONG_PTR)pBuf);
}

std::wstring GetExecutableDir(HINSTANCE hInstance)
{
    WCHAR buf[MAX_PATH];
    GetModuleFileName(hInstance, buf, MAX_PATH);
    PathRemoveFileSpec(buf);
    return buf;
}

std::wstring GetBro(std::wstring path)
{
    // You have to, due to restrictions in WindowsApps folder (can't be accessed "normally")
    // What the previous comment try to explain is, packaged applications are restricted from using protected system files.
    // Thus, the RuntimeBroker.exe found inside "C:\\Windows\\System32" can't be used directly to host the CoreCLR.
    // To overcome that we have to keep a local clone of the RuntimeBroker inside our APPX/MSIX package and use that host the CoreCLR.

#if _M_AMD64
    std::wstring cmdline = path + L"\\Bro_x64.exe -Embedding";
#elif _M_IX86
    std::wstring cmdline = path + L"\\Bro_x86.exe -Embedding";
#elif _M_ARM64
    std::wstring cmdline = path + L"\\Bro_A64.exe -Embedding";
#else
    std::wstring cmdline = path + L"\\Bro_A32.exe -Embedding";
#endif

    return cmdline;
}

int CALLBACK wWinMain(_In_ HINSTANCE hInstance, _In_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
    std::wstring path = GetExecutableDir(hInstance);

    STARTUPINFO startInfo = { 0 };
    PROCESS_INFORMATION procInfo = { 0 };

    if (CreateProcess(nullptr, (LPWSTR)GetBro(path).c_str(), nullptr, nullptr, FALSE, CREATE_SUSPENDED, nullptr, path.c_str(), &startInfo, &procInfo))
    {
        // Name of the bridge, on current executable directory
        std::wstring dll = path += L"\\ModernFlyoutsBridge.dll";
        InjectDll(procInfo.hProcess, procInfo.hThread, dll);
        ResumeThread(procInfo.hThread);

        // Sends the command-line arguments to the bridge via window messages.
        // 10 seconds should be more than enough
        for (size_t i = 0; i < 1000; i++)
        {
            auto hwnd = FindWindow(L"ModernFlyoutsBridge", NULL);

            if (hwnd)
            {
                auto lcmdLine = lstrlenW(lpCmdLine);
                if (lcmdLine > 0)
                {
                    COPYDATASTRUCT cDS{};
                    cDS.dwData = NULL;
                    cDS.cbData = lcmdLine * sizeof(WCHAR) + 1;
                    cDS.lpData = lpCmdLine;

                    SendMessage(hwnd, WM_COPYDATA, NULL, (LPARAM)(LPVOID)&cDS);
                }
                else
                {
                    SendMessage(hwnd, WM_QUIT, 0, 0);
                }
                break;
            }
            else
            {
                Sleep(10);
            }
        }
    }
    else
    {
        MessageBox(0, std::to_wstring((ULONG)GetLastError()).c_str(), L"CreateProcess failed", 0);
    }

    return 0;
}