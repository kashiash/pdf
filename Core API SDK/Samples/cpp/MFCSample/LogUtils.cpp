#include "stdafx.h"
#include "RTFLoger.h"

extern BOOL _ASSERTDISPLAYFUNCTION(LPCWSTR sz);

void __cdecl _Log(LPCWSTR sFmt, ...)
{
	va_list args;
	va_start(args, sFmt);
	_LogVA(sFmt, args);
	va_end(args);

}

void __cdecl _LogVA(LPCWSTR sFmt, va_list args)
{
	CStringW str;
	str.FormatV(sFmt, args);
	str.AppendChar(L'\n');
	_ASSERTDISPLAYFUNCTION(str.GetString());
}
