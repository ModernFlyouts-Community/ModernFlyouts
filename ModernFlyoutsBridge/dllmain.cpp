#include <string>
#include <Windows.h>
#include <Shlwapi.h>
#include <iostream>

#define NETHOST_USE_AS_STATIC
#include "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.0\hostfxr.h"
#include <vector>

#pragma comment(lib, "shlwapi.lib")
#pragma warning(disable : 4996)

#define PtrFromRva( base, rva ) ( ( ( PBYTE ) base ) + rva )
BOOL HookIAT(const char* szModuleName, const char* szFuncName, PVOID pNewFunc, PVOID* pOldFunc)
{
    PIMAGE_DOS_HEADER pDosHeader = (PIMAGE_DOS_HEADER)GetModuleHandle(NULL);
    PIMAGE_NT_HEADERS pNtHeader = (PIMAGE_NT_HEADERS)PtrFromRva(pDosHeader, pDosHeader->e_lfanew);

    if (pNtHeader->Signature != IMAGE_NT_SIGNATURE)
        return FALSE;

    PIMAGE_IMPORT_DESCRIPTOR pImportDescriptor = (PIMAGE_IMPORT_DESCRIPTOR)PtrFromRva(pDosHeader, pNtHeader->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);

    for (UINT uIndex = 0; pImportDescriptor[uIndex].Characteristics != 0; uIndex++)
    {
        char* szDllName = (char*)PtrFromRva(pDosHeader, pImportDescriptor[uIndex].Name);

        if (_strcmpi(szDllName, szModuleName) != 0)
            continue;

        if (!pImportDescriptor[uIndex].FirstThunk || !pImportDescriptor[uIndex].OriginalFirstThunk)
            return FALSE;

        PIMAGE_THUNK_DATA pThunk = (PIMAGE_THUNK_DATA)PtrFromRva(pDosHeader, pImportDescriptor[uIndex].FirstThunk);
        PIMAGE_THUNK_DATA pOrigThunk = (PIMAGE_THUNK_DATA)PtrFromRva(pDosHeader, pImportDescriptor[uIndex].OriginalFirstThunk);

        for (; pOrigThunk->u1.Function != NULL; pOrigThunk++, pThunk++)
        {
            if (pOrigThunk->u1.Ordinal & IMAGE_ORDINAL_FLAG)
                continue;

            PIMAGE_IMPORT_BY_NAME import = (PIMAGE_IMPORT_BY_NAME)PtrFromRva(pDosHeader, pOrigThunk->u1.AddressOfData);

            if (_strcmpi(szFuncName, (char*)import->Name) != 0)
                continue;

            DWORD dwJunk = 0;
            MEMORY_BASIC_INFORMATION mbi;

            VirtualQuery(pThunk, &mbi, sizeof(MEMORY_BASIC_INFORMATION));
            if (!VirtualProtect(mbi.BaseAddress, mbi.RegionSize, PAGE_EXECUTE_READWRITE, &mbi.Protect))
                return FALSE;

            *pOldFunc = (PVOID*)(DWORD_PTR)pThunk->u1.Function;

#ifdef _WIN64
            pThunk->u1.Function = (ULONGLONG)(DWORD_PTR)pNewFunc;
#else
            pThunk->u1.Function = (DWORD)(DWORD_PTR)pNewFunc;
#endif

            if (VirtualProtect(mbi.BaseAddress, mbi.RegionSize, mbi.Protect, &dwJunk))
                return TRUE;
        }
    }
    return FALSE;
}

hostfxr_initialize_for_dotnet_command_line_fn init_cmdline;
hostfxr_close_fn close_fptr;
hostfxr_run_app_fn run_fptr;
HWND serverBridge;
WCHAR* args = nullptr;

LRESULT CALLBACK BridgeWndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COPYDATA:
    {
        COPYDATASTRUCT* pcds = (COPYDATASTRUCT*)lParam;
        LPCWSTR lpszString = (LPCWSTR)(pcds->lpData);
        args = new WCHAR[pcds->cbData * sizeof(WCHAR) + 1];
        wcscpy(args, lpszString);
        PostQuitMessage(0);
    }
    break;
    case WM_QUIT:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hwnd, message, wParam, lParam);
    }
    return 0;
}

HWND CreateMicroWindow(HINSTANCE hInstance)
{
    const auto className = L"ModernFlyoutsBridge";

    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = BridgeWndProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = className;

    if (!RegisterClass(&wc))
        return NULL;

    const auto hwnd = CreateWindowEx(0, className, className,
        0, 0, 0, 0, 0, HWND_MESSAGE, NULL, hInstance, NULL);

    if (!hwnd)
        return NULL;

    return hwnd;
}

std::wstring GetExecutableDir()
{
    WCHAR buf[MAX_PATH];
    GetModuleFileName(nullptr, buf, MAX_PATH);
    PathRemoveFileSpec(buf);
    return buf;
}

bool load_hostfxr()
{
    auto fxr_path = GetExecutableDir() + L"\\hostfxr.dll";

    HMODULE lib = LoadLibraryW(fxr_path.c_str());
    init_cmdline = (hostfxr_initialize_for_dotnet_command_line_fn)GetProcAddress(lib, "hostfxr_initialize_for_dotnet_command_line");
    run_fptr = (hostfxr_run_app_fn)GetProcAddress(lib, "hostfxr_run_app");
    close_fptr = (hostfxr_close_fn)GetProcAddress(lib, "hostfxr_close");

    return (init_cmdline && run_fptr && close_fptr);
}

PVOID pOldProc;
PVOID pNewProc;
HRESULT WINAPI hookCoResumeClassObjects()
{
    //This is a very low quality BAIT
    //Bypasses CO_E_WRONG_SERVER_IDENTITY by returning S_OK when running as packaged

    //Or we could never return lol. Either this or a mutex that never gets signaled, which one is more cruel it's up to you.
    while (true)
        Sleep(345600);

    HookIAT("api-ms-win-core-com-l1-1-1.dll", "CoResumeClassObjects", pOldProc, &pNewProc);
    return S_OK + 1;
}

DWORD WINAPI LocalThread(LPVOID lpParam)
{
    auto host_path = GetExecutableDir() + L"\\";
    auto exec_path = host_path + L"ModernFlyouts.dll";

    serverBridge = CreateMicroWindow((HINSTANCE)lpParam);

    //TODO: it will actually never exits if the launcher doesn't send args (e.g. it crashed)
    //Should we use mutexes?

    MSG msg = {};
    while (GetMessage(&msg, 0, 0, 0) > 0)
        DispatchMessage(&msg);

	if (!load_hostfxr())
	{
        MessageBox(0, L"Framework-dependent net core? Then copy hostfxr to the appx output", L"Hey", 0);
		auto pl = GetCurrentProcess();
		TerminateProcess(pl, EXIT_SUCCESS);
		return EXIT_FAILURE;
	}

    hostfxr_initialize_parameters params{};
    params.dotnet_root = host_path.c_str();
    params.host_path = exec_path.c_str();
    params.size = sizeof(hostfxr_initialize_parameters);

    hostfxr_handle handle{};

    if (args)
    {
        int argc = 0;
        LPWSTR* argv = CommandLineToArgvW(args, &argc);

        const char_t** dotnet_args = new const char_t*[argc + 1];

        dotnet_args[0] = exec_path.c_str();
        for (size_t i = 0; i < argc; i++)
            dotnet_args[i + 1] = *(argv + i);

        //TODO: we should delete the array btw - but anyway the process will exit after .NET CRT shutdown so... shrug

        auto hmm = init_cmdline(1 + argc, dotnet_args, &params, &handle);
    }
    else
    {
        const char_t* dotnet_args[1] = { exec_path.c_str() };
        auto hmm = init_cmdline(1, dotnet_args, &params, &handle);
    }

    run_fptr(handle);
    close_fptr(handle);

	auto p = GetCurrentProcess();
	TerminateProcess(p, EXIT_SUCCESS);

    return EXIT_SUCCESS;
}

int main()
{
    return LocalThread(GetModuleHandle(NULL));
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        HookIAT("api-ms-win-core-com-l1-1-1.dll", "CoResumeClassObjects", (PVOID)hookCoResumeClassObjects, &pOldProc);
        CreateThread(NULL, 0, LocalThread, hModule, NULL, NULL);
		break;
	default:
        break;
    }
    return TRUE;
}

