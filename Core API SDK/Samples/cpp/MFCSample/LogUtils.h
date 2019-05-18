#pragma once

void __cdecl _Log(LPCWSTR sFmt, ...);
void __cdecl _LogVA(LPCWSTR sFmt, va_list args);

#define LOG(x, f, ...)		_Log(f, __VA_ARGS__)
#define BreakFunc()		break;

#define BreakOnFailure(x, f, ...)				if (FAILED(x)) { LOG(x, f, __VA_ARGS__);  BreakFunc(); }
#define BreakOnNull(p, x, e, f,...)				if (nullptr == (p)) { x = e; LOG(x, f, __VA_ARGS__); BreakFunc(); }
#define BreakOnNullWithLastError(p, x, f, ...)	if (nullptr == (p)) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (!FAILED(x)) { x = E_FAIL; } LOG(x, f, __VA_ARGS__); BreakFunc(); }
