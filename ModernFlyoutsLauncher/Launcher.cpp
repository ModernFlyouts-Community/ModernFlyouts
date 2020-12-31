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

int CALLBACK WinMain(_In_ HINSTANCE hInstance, _In_ HINSTANCE hPrevInstance, _In_ LPSTR lpCmdLine, _In_ int nCmdShow)
{
	std::wstring path = GetExecutableDir(hInstance);

	STARTUPINFO startInfo = { 0 };
	PROCESS_INFORMATION procInfo = { 0 };

	//You have to, due to restrictions in WindowsApps folder (can't be accessed "normally")
#if _M_AMD64
	std::wstring cmdline = path + L"\\Bro_x64.exe -Embedding";
#elif _M_IX86
	std::wstring cmdline = path + L"\\Bro_x86.exe -Embedding";
#elif _M_ARM64
	std::wstring cmdline = path + L"\\Bro_A64.exe -Embedding";
#else
	std::wstring cmdline = path + L"\\Bro_A32.exe -Embedding";
#endif

	if (CreateProcess(nullptr, (LPWSTR)cmdline.c_str(), nullptr, nullptr, FALSE, CREATE_SUSPENDED, nullptr, path.c_str(), &startInfo, &procInfo))
	{
		//Name of the bridge, on current executable directory
		std::wstring dll = path += L"\\ModernFlyoutsBridge.dll";
		InjectDll(procInfo.hProcess, procInfo.hThread, dll);
		ResumeThread(procInfo.hThread);
	}
	else
	{
		MessageBox(0, std::to_wstring((ULONG)GetLastError()).c_str(), L"CreateProcess fail", 0);
	}

	return 0;
}