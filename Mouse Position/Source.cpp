#include <iostream>
#include <Windows.h>
#include <stdio.h>

HHOOK hHock = NULL;

LRESULT CALLBACK MyLowLevelHook(int nCode, WPARAM wParam, LPARAM lParam)
{
	POINT p;
	if (GetCursorPos(&p))
		printf("%cx%i y%i\t\t",char(13),p.x,p.y);
	return CallNextHookEx(hHock, nCode, wParam, lParam);
	//Message
}

int main()
{
	MSG msg;
	hHock = SetWindowsHookEx(WH_MOUSE_LL, MyLowLevelHook, NULL, NULL);

	while (!GetMessage(&msg, NULL, NULL, NULL))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	UnhookWindowsHookEx(hHock);

}
