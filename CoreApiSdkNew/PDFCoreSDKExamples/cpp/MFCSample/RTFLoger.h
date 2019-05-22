#pragma once
#include <richedit.h>

// Text Color
class Color
{
public:
	Color(COLORREF c) : color_(c) {}
	Color(BYTE R, BYTE G, BYTE B) : color_(RGB(R, G, B)) {}
	COLORREF color_;
};

static Color BlackColor(0);
static Color RedColor(250, 0, 0);
static Color GreenColor(0, 120, 0);
static Color BlueColor(0, 0, 255);

// Font name
class FontName
{
public:
	FontName(WCHAR* fn)
	{
		::lstrcpynW(m_FN, fn, LF_FACESIZE);
	}
public:
	WCHAR	m_FN[LF_FACESIZE];
};

// Font size
class FontSize
{
public: 
	FontSize(int fs) : m_FontSz(fs){}
public:
	 int	m_FontSz;
};

// Font effects
// CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_COLOR | CFM_STRIKEOUT | CFE_PROTECTED | CFM_LINK
class FontEffects
{
public:
	FontEffects(DWORD ef) : m_Effects(ef) {}
public:
	DWORD m_Effects;
};

class RTFLoger
{
public:
	RTFLoger(HWND h);
	RTFLoger() : m_re(NULL) {}
	~RTFLoger();
public:
	void SetHWND(HWND h) { m_re = h; }
	void HideSel(BOOL bShow = FALSE);
	void Scrollenable(BOOL bEnable = TRUE);
public:
	void Clear();
	void CopyAll();
public:
	RTFLoger& operator<<(const CStringW& str);
	RTFLoger& operator<<(LPCWSTR wStr);
	RTFLoger& operator<<(LPCSTR wStr);
	RTFLoger& operator<<(LONG l);
	RTFLoger& operator<<(DWORD dw);
	RTFLoger& operator<<(int i);
	RTFLoger& operator<<(size_t t);
	RTFLoger& operator<<(WCHAR wc);
	RTFLoger& operator<<(char c);
	RTFLoger& operator<<(__int64 i);
	RTFLoger& operator<<(double d);

	RTFLoger& operator<<(RTFLoger& (*f)(RTFLoger&)) { return f(*this); }
	RTFLoger& operator<<(Color& Color);
	RTFLoger& operator<<(FontName& fn);
	RTFLoger& operator<<(FontSize& fs);
	RTFLoger& operator<<(FontEffects& fe);
	
public:
	HWND	m_re;
private:
	void AppendTextW(const WCHAR* wStr);
	void AppendTextA(const char* cStr);
};

// Send "new line" to the log
RTFLoger& nl(RTFLoger& s);
RTFLoger& tab(RTFLoger& s);

extern RTFLoger	g_Log;
