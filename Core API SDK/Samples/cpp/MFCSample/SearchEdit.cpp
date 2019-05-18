#include "stdafx.h"
#include "SearchEdit.h"
#include "Resource.h"
#include "MFCSample.h"

IMPLEMENT_DYNAMIC(CSearchEdit, CEdit)

CSearchEdit::CSearchEdit()
{
	m_rcEdges.SetRectEmpty();
	m_bHasText = false;
	m_nBtnState = bs_Normal;
}

CSearchEdit::~CSearchEdit()
{
}

void CSearchEdit::Initialize()
{
	if (m_Images.m_hImageList != nullptr)
		return;
	//
	HDC hDC = ::GetDC(nullptr);
	LONG nDPI = GetDeviceCaps(hDC, LOGPIXELSY);
	::ReleaseDC(nullptr, hDC);
	m_bLarge = (nDPI >= 144);

	HIMAGELIST hImg = ImageList_LoadImage(theApp.m_hInstance, MAKEINTRESOURCE(m_bLarge ? IDB_EDIT_ICONS20 : IDB_EDIT_ICONS16),
										  m_bLarge ? 20 : 16, 0, CLR_NONE, IMAGE_BITMAP,
										  LR_CREATEDIBSECTION | LR_LOADTRANSPARENT);

	m_Images.Attach(hImg);

	//SetFont(&afxGlobalData.fontRegular);
	//SetMargins(5, 5);

	m_clrBtn[bs_Normal] = GetSysColor(COLOR_WINDOW);
	m_clrBtn[bs_Hover] = GetSysColor(COLOR_HIGHLIGHTTEXT);
	m_clrBtn[bs_Down] = GetSysColor(COLOR_HIGHLIGHTTEXT);
}

void CSearchEdit::PreSubclassWindow()
{
//	__super::PreSubclassWindow();
	Initialize();
	ResizeWindow();
}

BOOL CSearchEdit::PreTranslateMessage(MSG* pMsg)
{
	return __super::PreTranslateMessage(pMsg);
}

int CSearchEdit::GetIdealHeight()
{
	static const int defHeight = 10;

	int nHeight = defHeight;

	HDC hDC = ::GetDC(nullptr);
	LONG nDPI = GetDeviceCaps(hDC, LOGPIXELSY);
	::ReleaseDC(nullptr, hDC);

	HWND hWnd = GetSafeHwnd();
	if (hWnd != nullptr)
	{
		CFont* pFont = GetFont();
		if (pFont != nullptr)
		{
			LOGFONT lf = {0};
			pFont->GetLogFont(&lf);
			if (lf.lfHeight != 0)
				nHeight = lf.lfHeight;
		}
	}

	if (nHeight > 0)
		nHeight = MulDiv(nHeight, nDPI, 72);
	else
		nHeight = abs(nHeight);

	nHeight += MulDiv(8, nDPI, 96);

	return nHeight;
}

void CSearchEdit::ResizeWindow()
{
	HWND hWnd = GetSafeHwnd();
	if (hWnd == nullptr)
		return;
}

void CSearchEdit::GetButtonRect(const CRect& wr, CRect& rc)
{
	rc = wr;
	rc.DeflateRect(m_rcEdges);
	rc.left = rc.right - GetImageSize();
}

void CSearchEdit::InvalidateButton()
{
	RedrawWindow(nullptr, nullptr, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
}

void CSearchEdit::OnNcCalcSize(BOOL bCalcValidRects, NCCALCSIZE_PARAMS* lpncsp)
{
	lpncsp->rgrc[0].right -= GetImageSize() + 2;
	__super::OnNcCalcSize(bCalcValidRects, lpncsp);
}

void CSearchEdit::OnNcPaint()
{
	__super::OnNcPaint();

	CDC* pDC = GetWindowDC();

	CRect rc;
	GetWindowRect(rc);
	rc.OffsetRect(-rc.left, -rc.top);

	rc.DeflateRect(2, 2);
	GetButtonRect(rc, rc);
	rc.left -= 2;
	pDC->FillSolidRect(&rc, m_clrBtn[m_nBtnState]);
	rc.left += 2;

	int w = GetImageSize();
	CPoint pt;
	pt.x = (rc.left + rc.right - w) / 2;
	pt.y = (rc.top + rc.bottom - w) / 2;

	int nIndex = 0;
	if (GetWindowTextLength() > 0)
		nIndex++;
	m_Images.Draw(pDC, nIndex, pt, ILD_NORMAL);
	ReleaseDC(pDC);
}

BOOL CSearchEdit::OnEraseBkgnd(CDC* pDC)
{
	return TRUE;
}

void CSearchEdit::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	InvalidateRect(nullptr);
	return __super::OnKeyDown(nChar, nRepCnt, nFlags);
}

void CSearchEdit::OnLButtonDown(UINT nFlags, CPoint point)
{
	__super::OnLButtonDown(nFlags, point);
}

BOOL CSearchEdit::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
	CPoint ptCursor;
	GetCursorPos(&ptCursor);
	::MapWindowPoints(HWND_DESKTOP, m_hWnd, &ptCursor, 1);
	if (false)
	{
	}
	return __super::OnSetCursor(pWnd, nHitTest, message);
}

int CSearchEdit::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (__super::OnCreate(lpCreateStruct) == -1)
		return -1;
	Initialize();
	ResizeWindow();
	return 0;
}

LRESULT CSearchEdit::OnSetFont(WPARAM wParam, LPARAM lParam)
{
	DefWindowProc(WM_SETFONT, wParam, lParam);
	ResizeWindow();
	return 0;
}

void CSearchEdit::OnSize(UINT nType, int cx, int cy)
{
	__super::OnSize(nType, cx, cy);
	ResizeWindow();
}

void CSearchEdit::OnChanged()
{
	bool bHasText = GetWindowTextLength() > 0;
	if (bHasText != m_bHasText)
	{
		m_bHasText = bHasText;
		InvalidateButton();
	}
}

//////////////////////////////////////////////////////////////////////////
BEGIN_MESSAGE_MAP(CSearchEdit, CEdit)
	ON_WM_CREATE()
	ON_WM_ERASEBKGND()
	ON_WM_CHAR()
	ON_WM_KEYDOWN()
	ON_WM_LBUTTONDOWN()
	ON_WM_SETCURSOR()
	ON_WM_SIZE()
	ON_WM_NCCALCSIZE()
	ON_WM_NCPAINT()
	ON_MESSAGE(WM_SETFONT, OnSetFont)
	ON_CONTROL_REFLECT(EN_CHANGE, OnChanged)
END_MESSAGE_MAP()
