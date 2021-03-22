#include <string>
#include <Windows.h>
#include <Shlwapi.h>

#define NETHOST_USE_AS_STATIC
#include "inc\hostfxr.h"
#include <vector>

#pragma comment(lib, "shlwapi.lib")
#pragma warning(disable : 4996)

hostfxr_initialize_for_dotnet_command_line_fn init_cmdline;
hostfxr_close_fn close_fptr;
hostfxr_run_app_fn run_fptr;

//OUTPUT DLL IS "DXGI.dll"
//DO *NOT* CHANGE.

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

HRESULT LoadCLR()
{
    auto host_path = GetExecutableDir() + L"\\";
    auto exec_path = host_path + L"ModernFlyouts.dll";

    if (!load_hostfxr())
    {
        // Nope, not necessary if you use Debug (x64) configuration.
        MessageBox(0, L"Framework-dependent net core? Then copy hostfxr.dll to the APPX output directory", L"Hey", 0);
        auto pl = GetCurrentProcess();
        TerminateProcess(pl, EXIT_SUCCESS);
        return EXIT_FAILURE;
    }

    hostfxr_initialize_parameters params{};
    params.dotnet_root = host_path.c_str();
    params.host_path = exec_path.c_str();
    params.size = sizeof(hostfxr_initialize_parameters);

    hostfxr_handle handle{};

    // The CoreCLR executes with empty arguments if there are no arguments.
    int argc = 0;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLine(), &argc);

    if (argc > 1)
    {
        const char_t** dotnet_args = new const char_t * [argc + 1];

        dotnet_args[0] = exec_path.c_str(); // The 1st argument has to be the path of the main ModernFlyouts.dll (.NET) library
        for (size_t i = 0; i < argc; i++)
            dotnet_args[i + 1] = *(argv + i); // Subsequent arguments are passed after that

        // TODO: We should clear up the array BTW.
        // But the process will exit after .NET CoreCLR shuts down anyway so... *shrug*

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

    //It will never happen :)
    return S_OK;
}

extern "C"
{
    __declspec(dllexport) HRESULT CreateDXGIFactory(REFIID riid, void** ppFactory)
    {
        return LoadCLR();
    }

    __declspec(dllexport) HRESULT CreateDXGIFactory1(REFIID riid, void** ppFactory)
    {
        return LoadCLR();
    }

    //This is in case you get an error about entry point being meme for D3D11 (lol? d3d11 does not export CreateDXGIFactory*)
    __declspec(dllexport) HRESULT CreateDXGIFactory2(UINT Flags, REFIID riid, void** ppFactory)
    {
        return LoadCLR();
    }

    //Our main target.
    __declspec(dllexport) HRESULT DXGIDeclareAdapterRemovalSupport()
    {
        return LoadCLR();
    }
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved)
{
    return TRUE;
}
