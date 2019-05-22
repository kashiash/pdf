#include "stdafx.h"
#include "RTFLoger.h"
#include <richedit.h>

RTFLoger	g_Log;

RTFLoger::RTFLoger(HWND h)
{
	m_re = h;
	if (::IsWindow(h))
	{
		*this << L' ';
		PARAFORMAT2 pf2;
		::ZeroMemory(&pf2, sizeof(PARAFORMAT2));
		pf2.cbSize = sizeof(PARAFORMAT2);
		pf2.dwMask = PFM_LINESPACING | PFM_SPACEAFTER | PFM_STARTINDENT | PFM_OFFSETINDENT;
		pf2.bLineSpacingRule = 0;
		pf2.dxStartIndent = 150;
		::SendMessage(m_re, EM_SETPARAFORMAT, 0, (LPARAM)&pf2);
		//m_re.SetParaFormat(pf2);
		::SendMessage(m_re, EM_SETSEL, 0, 1);
		//m_re.SetSel(0, 1);
		//m_re.Clear();
		::SendMessage(m_re, WM_CLEAR, 0, 0L);
		::SendMessage(m_re, EM_SETSEL, 0, 0);
		//m_re.SetSel(0, 0);
	}
}

RTFLoger::~RTFLoger()
{
}

void RTFLoger::Clear()
{
	if (!::IsWindow(m_re))
		return;
	::SendMessage(m_re, EM_SETSEL, 0, -1);
	::SendMessage(m_re, EM_REPLACESEL, 0, (LPARAM)_T(""));
}

void RTFLoger::CopyAll()
{
	if (!::IsWindow(m_re))
		return;
	::SendMessage(m_re, EM_SETSEL, 0, -1);
	::SendMessage(m_re, WM_COPY, 0, 0L);
	::SendMessage(m_re, EM_SETSEL, 0, 0);
}

void RTFLoger::AppendTextW(const WCHAR* wStr)
{
	BOOL bCanUndo = FALSE;
	int textLength = (int)::SendMessage(m_re, WM_GETTEXTLENGTH, 0 , 0);
	//m_re.GetWindowTextLength();
	CHARRANGE cr = { textLength, textLength };
	::SendMessageW(m_re, EM_EXSETSEL, 0, (LPARAM)&cr);
	::SendMessageW(m_re, EM_REPLACESEL, (WPARAM) bCanUndo, (LPARAM)wStr);
}

void RTFLoger::AppendTextA(const char* cStr)
{
	BOOL bCanUndo = FALSE;
	int textLength = (int)::SendMessage(m_re, WM_GETTEXTLENGTH, 0 , 0);
	//m_re.GetWindowTextLength();
	CHARRANGE cr = { textLength, textLength };
	::SendMessageA(m_re, EM_EXSETSEL, 0, (LPARAM)&cr);
	::SendMessageA(m_re, EM_REPLACESEL, (WPARAM) bCanUndo, (LPARAM)cStr);
}

RTFLoger& RTFLoger::operator<<(const WCHAR* wStr)
{
	AppendTextW(wStr);
	return *this;
}

RTFLoger& RTFLoger::operator<<(LPCSTR Str)
{
	AppendTextA(Str);
	return *this;
}

RTFLoger& RTFLoger::operator<<(const CStringW& str)
{
	AppendTextW(str.GetString());
	return *this;
}

RTFLoger& RTFLoger::operator<<(WCHAR wc)
{
	WCHAR str[2];
	str[0] = wc;
	str[1] = L'\0';
	return *this << str;
}

RTFLoger& RTFLoger::operator<<(char c)
{
	char str[2];
	str[0] = c;
	str[1] = '\0';
	return *this << str;
}
RTFLoger& RTFLoger::operator<<(LONG l)
{
	WCHAR temp[128];
	wsprintfW(temp, L"%d", l);
    return *this << temp;
}
RTFLoger& RTFLoger::operator<<(DWORD dw)
{
	return *this << (LONG)dw;
}
RTFLoger& RTFLoger::operator<<(int i)
{
	return *this << (LONG)i;
}
RTFLoger& RTFLoger::operator<<(size_t t)
{
	return operator<<((LONG)t);
}
RTFLoger& RTFLoger::operator<<(double d)
{
	CStringW str;
	str.Format(L"%.3f", d);
	AppendTextW(str.GetString());
	return *this;
}
RTFLoger& RTFLoger::operator<<(__int64 i)
{
	CStringW	str;
	str.Format(L"%I64d", i);
	AppendTextW(str.GetString());
	return *this;
}

RTFLoger& RTFLoger::operator<<(Color& Color)
{
	CHARFORMAT cf;
	::ZeroMemory(&cf, sizeof(CHARFORMAT));
	cf.cbSize = sizeof(CHARFORMAT);
	cf.dwMask = CFM_COLOR;
	cf.crTextColor = Color.color_;	
	//m_re.SetCharFormat(cf, SCF_SELECTION);
	::SendMessage(m_re, EM_SETCHARFORMAT, SCF_SELECTION, (LPARAM)&cf);
	return *this;
}
RTFLoger& RTFLoger::operator<<(FontName& fn)
{
	CHARFORMAT cf;
	::ZeroMemory(&cf, sizeof(CHARFORMAT));
	cf.cbSize = sizeof(cf);
	cf.dwMask = CFM_FACE;
	::lstrcpynW(cf.szFaceName, fn.m_FN, LF_FACESIZE);	
	::SendMessageW(m_re, EM_SETCHARFORMAT, SCF_SELECTION, (LPARAM)&cf);
	//m_re.SetCharFormat(cf, SCF_SELECTION);
	return *this;
}
RTFLoger& RTFLoger::operator<<(FontSize& fs)
{
	CHARFORMATW cf = {0};
	cf.cbSize = sizeof(CHARFORMATW);
	cf.dwMask = CFM_SIZE;
	cf.yHeight = fs.m_FontSz * 20;	// size in 1/20 of point
	//BOOL bd = m_re.SetCharFormat(cf, SCF_SELECTION);
	BOOL bd = (int)::SendMessageW(m_re, EM_SETCHARFORMAT, SCF_SELECTION, (LPARAM)&cf);
	DWORD er = GetLastError();
	return *this;
}
RTFLoger& RTFLoger::operator<<(FontEffects& fe)
{
	CHARFORMAT cf = {0};
	cf.cbSize = sizeof(CHARFORMAT);
	cf.dwMask = CFM_BOLD | CFM_ITALIC;
	cf.dwEffects = fe.m_Effects;
	//m_re.SetCharFormat(cf, SCF_SELECTION);
	::SendMessage(m_re, EM_SETCHARFORMAT, SCF_SELECTION, (LPARAM)&cf);

	return *this;
}
//
RTFLoger& nl(RTFLoger& s)
{
	s << L"\n";
	return s;
}

RTFLoger& tab(RTFLoger& s)
{
	s << L"\t";
	return s;
}

void RTFLoger::HideSel(BOOL bShow)
{
	::SendMessageW(m_re, EM_HIDESELECTION, (WPARAM)!bShow, (LPARAM)0);
}
void RTFLoger::Scrollenable(BOOL bEnable)
{
	LONG oldStyle = ::GetWindowLong(m_re, GWL_STYLE);
	if (bEnable)
	{
		oldStyle |= ES_AUTOVSCROLL;
	}
	else
	{
		oldStyle &= ~ES_AUTOVSCROLL;
	}
	::SetWindowLong(m_re, GWL_STYLE, oldStyle);
	if (bEnable)
	{
		::SendMessage(m_re, EM_SCROLLCARET, 0, 0);
	}
}
