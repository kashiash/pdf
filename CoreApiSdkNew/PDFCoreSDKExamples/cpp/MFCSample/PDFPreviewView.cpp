// PDFPreviewView.cpp : implementation file
//

#include "stdafx.h"
#include "MFCSample.h"
#include "PDFPreviewView.h"
#include "InitializeSDK.h"

static const LONG g_XGap = 10;
static const LONG g_YGap = 10;
static const int CacheStep	= 64;

// CPDFPreviewView

IMPLEMENT_DYNAMIC(CPDFPreviewView, CWnd)


BEGIN_MESSAGE_MAP(CPDFPreviewView, CWnd)
	ON_WM_ERASEBKGND()
	ON_WM_SIZE()
	ON_WM_PAINT()
	ON_WM_VSCROLL()
	ON_WM_HSCROLL()
	ON_WM_MOUSEWHEEL()
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_KEYDOWN()
	ON_WM_SETCURSOR()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////
CPDFPreviewView::CPDFPreviewView()
{
	m_nCurPage		= 0;
	m_ZoomLevel		= 0.0;
	m_nCoef			= 1.0;
	m_szPageSize.cx	=
	m_szPageSize.cy	= 0;
	m_ptOffset.x	=
	m_ptOffset.y	= 0;
	m_bInTrack		= FALSE;
	//
	m_rCachedRect.SetRectEmpty();
	m_bgColor		= GetSysColor(COLOR_APPWORKSPACE);
	//
	m_cursors[0]	= ::LoadCursor(theApp.m_hInstance, MAKEINTRESOURCE(IDC_CURSOR_HAND_UP));
	m_cursors[1]	= ::LoadCursor(theApp.m_hInstance, MAKEINTRESOURCE(IDC_CURSOR_HAND_DOWN));
}

CPDFPreviewView::~CPDFPreviewView()
{
}

//////////////////////////////////////////////////////////////////////////

// CPDFPreviewView diagnostics

#ifdef _DEBUG
void CPDFPreviewView::AssertValid() const
{
	__super::AssertValid();
}

#ifndef _WIN32_WCE
void CPDFPreviewView::Dump(CDumpContext& dc) const
{
	__super::Dump(dc);
}
#endif
#endif //_DEBUG

//////////////////////////////////////////////////////////////////////////
BOOL CPDFPreviewView::Create(DWORD dwWndStyle, RECT& rc, CWnd* pParentWnd, ULONG nID)
{
	//const DWORD dwWndStyle = WS_CHILD | WS_VISIBLE | WS_VSCROLL | WS_HSCROLL;
	const DWORD dwExtStyle = WS_EX_CLIENTEDGE;

	CString strMyClass;
	strMyClass = AfxRegisterWndClass(CS_VREDRAW | CS_HREDRAW,
									 m_cursors[0],
									 (HBRUSH)::GetStockObject(WHITE_BRUSH),
									 0);

	BOOL bOK = __super::CreateEx(dwExtStyle, strMyClass, _T(""), dwWndStyle, rc, pParentWnd, nID, nullptr);
	return bOK;
}

void CPDFPreviewView::ReleaseCache()
{
	m_pCache = nullptr;
	m_rCachedRect.SetRectEmpty();
}

bool CPDFPreviewView::EnsureCache(const CRect& pr)
{
	if (m_pCache != nullptr)
	{
		CRect ir;
		if (ir.IntersectRect(m_rCachedRect, pr) && ir.EqualRect(pr))
			return true;// m_rCachedRect contains pr
	}
	if (m_pDoc == nullptr)
	{
		m_pCache = nullptr;
		m_rCachedRect.SetRectEmpty();
		return false;
	}

	// todo: make more advanced cache
	// for now - rerender all
	CRect r = pr;
	r.InflateRect(CacheStep, CacheStep);
	r.IntersectRect(r, CRect(0, 0, m_szPageSize.cx, m_szPageSize.cy));
	HRESULT hr = S_OK;
	if ((r.Width() != m_rCachedRect.Width()) || (r.Height() != m_rCachedRect.Height()))
	{
		CComPtr<PXC::IIXC_Page> temp;
		hr = g_ImgCore->Page_CreateEmpty(r.Width(), r.Height(), PXC::PageFormat_8RGB, RGB(255, 255, 255), &temp);
		if (FAILED(hr))
			return false;
		m_pCache = temp;
	}

	CComPtr<PXC::IPXC_Pages> pages;
	CComPtr<PXC::IPXC_Page> page;
	hr = m_pDoc->get_Pages(&pages);
	if (FAILED(hr))
		return false;
	hr = pages->get_Item(m_nCurPage, &page);
	if (FAILED(hr))
		return false;
	PXC::PXC_Matrix partMatrix = m_Matrix;
	partMatrix.e -= r.left;
	partMatrix.f -= r.top;
	CRect DrawRect(0, 0, r.Width(), r.Height());
	hr = page->DrawToIXCPage(m_pCache, DrawRect, &partMatrix, nullptr, nullptr, nullptr);
	if (FAILED(hr))
		return false;
	m_rCachedRect = r;
	return true;
}

bool CPDFPreviewView::FixupScrolls(CPoint& offs) const
{
	CRect cr;
	CSize ts = GetTotalSize();
	GetClientRect(cr);
	LONG mx = max(0, ts.cx - cr.Width());
	LONG my = max(0, ts.cy - cr.Height());
	if ((offs.x >= 0) && (offs.x <= mx) && (offs.y >= 0) && (offs.y <= my))
		return false;
	offs.x = min(mx, max(0, offs.x));
	offs.y = min(my, max(0, offs.y));
	return true;
}

CRect CPDFPreviewView::GetPageRect() const
{
	CRect pr, cr;
	CSize ts = GetTotalSize();
	GetClientRect(cr);
	// X
	if (ts.cx <= cr.Width())
		pr.left = (cr.right + cr.left - m_szPageSize.cx) / 2;
	else
		pr.left = g_XGap - m_ptOffset.x;
	pr.right = pr.left + m_szPageSize.cx;
	// Y
	if (ts.cy <= cr.Height())
		pr.top = (cr.bottom + cr.top - m_szPageSize.cy) / 2;
	else
		pr.top = g_YGap - m_ptOffset.y;
	pr.bottom = pr.top + m_szPageSize.cy;
	return pr;
}

CSize CPDFPreviewView::GetTotalSize() const
{
	CSize sz = m_szPageSize;
	sz.cx += 2 * g_XGap;
	sz.cy += 2 * g_YGap;
	return sz;
}

void CPDFPreviewView::UpdateScrolls()
{
	CRect cr;
	CSize sz = GetTotalSize();
	GetClientRect(cr);
	SCROLLINFO si = {0};
	si.cbSize	= sizeof(si);
	si.fMask	= SIF_RANGE | SIF_PAGE | SIF_POS | SIF_DISABLENOSCROLL;
	si.nMin		= 0;
	// horizontal
	si.nMax		= sz.cx - 1;
	si.nPage	= cr.Width();
	si.nPos		= m_ptOffset.x;
	SetScrollInfo(SB_HORZ, &si);
	// vertical
	si.nMax		= sz.cy - 1;
	si.nPage	= cr.Height();
	si.nPos		= m_ptOffset.y;
	SetScrollInfo(SB_VERT, &si);
}

void CPDFPreviewView::SetDocument(PXC::IPXC_Document* pDoc)
{
	if (m_pDoc == pDoc)
		return;
	ReleaseCache();
	m_pDoc = pDoc;
	m_nCurPage = 0;
	m_ptOffset.x = m_ptOffset.y = 0;
	m_szPageSize = CalcPageSize();
	UpdateScrolls();
	Invalidate();
}

ULONG CPDFPreviewView::GetNumPages() const
{
	ULONG nCount = 0;
	if (m_pDoc)
	{
		CComPtr<PXC::IPXC_Pages> pages;
		m_pDoc->get_Pages(&pages);
		pages->get_Count(&nCount);
	}
	return nCount;
}

void CPDFPreviewView::SetCurPage(ULONG nPage)
{
	if (m_pDoc == nullptr)
		return;
	CComPtr<PXC::IPXC_Pages> pages;
	m_pDoc->get_Pages(&pages);
	ULONG nCount = 0;
	pages->get_Count(&nCount);
	if (nPage >= nCount)
		return;
	if (nPage == m_nCurPage)
		return;
	ReleaseCache();
	m_nCurPage = nPage;
	m_ptOffset.x = m_ptOffset.y = 0;
	m_szPageSize = CalcPageSize();
	UpdateScrolls();
	Invalidate();
}

void CPDFPreviewView::ZoomIn()
{
	double zoom = GetZoom();
	if (zoom < 100.0)
		zoom += 10.0;
	else
		zoom += 100.0;
	SetZoom(zoom);
}

void CPDFPreviewView::ZoomOut()
{
	double zoom = GetZoom();
	if (zoom <= 100.0)
		zoom -= 10.0;
	else
		zoom -= 100.0;
	SetZoom(zoom);
}

void CPDFPreviewView::SetZoom(double nZoomLevel)
{
	nZoomLevel = max(Preview_Min_Zoom, min(Preview_Max_Zoom, nZoomLevel));
	if (nZoomLevel == m_ZoomLevel)
		return;

	CDC* pDC = GetWindowDC();
	LONG nDPI = pDC->GetDeviceCaps(LOGPIXELSY);
	double coef = (double)nDPI * nZoomLevel / 7200.0;
	ReleaseDC(pDC);
	if (coef == m_nCoef)
		return;

	ReleaseCache();
	LONG nx = (LONG)(((double)m_ptOffset.x / m_nCoef) * coef + 0.5);
	LONG ny = (LONG)(((double)m_ptOffset.y / m_nCoef) * coef + 0.5);
	m_nCoef = coef;
	m_ZoomLevel = nZoomLevel;
	m_szPageSize = CalcPageSize();
	m_ptOffset.x = nx;
	m_ptOffset.y = ny;
	FixupScrolls(m_ptOffset);
	UpdateScrolls();
	Invalidate();
}

CSize CPDFPreviewView::CalcPageSize()
{
	CSize sz;
	sz.cx = sz.cy = 0;
	if (m_pDoc == nullptr)
		return sz;
	CComPtr<PXC::IPXC_Pages> pages;
	CComPtr<PXC::IPXC_Page> page;
	m_pDoc->get_Pages(&pages);
	pages->get_Item(m_nCurPage, &page);
	double pw, ph;
	page->GetDimension(&pw, &ph);

	sz.cx = (LONG)(pw * m_nCoef + 0.5);
	sz.cy = (LONG)(ph * m_nCoef + 0.5);
	page->GetMatrix(PXC::PBox_PageBox, &m_Matrix);
	m_Matrix.a *= m_nCoef;
	m_Matrix.b *= -m_nCoef;
	m_Matrix.c *= m_nCoef;
	m_Matrix.d *= -m_nCoef;
	m_Matrix.e *= m_nCoef;
	m_Matrix.f = (double)sz.cy - m_Matrix.f * m_nCoef;

	return sz;
}

CSize CPDFPreviewView::GetPageSize()
{
	return m_szPageSize;
}

// CPDFPreviewView drawing
void CPDFPreviewView::OnInitialUpdate()
{
	SetZoom(100.0);
	CSize sizeTotal = GetPageSize();
//	SetScrollSizes(MM_TEXT, sizeTotal);
}

// CPDFPreviewView message handlers
afx_msg BOOL CPDFPreviewView::OnEraseBkgnd(CDC* pDC)
{
	if (pDC == nullptr)
		return TRUE;
	CRect cr, pr;
	GetClientRect(cr);
	CRgn rgn;
	rgn.CreateRectRgn(cr.left, cr.top, cr.right, cr.bottom);
	int res = SIMPLEREGION;
	if (m_pDoc != nullptr)
	{
		pr = GetPageRect();
		CRgn rgn2;
		rgn2.CreateRectRgn(pr.left, pr.top, pr.right, pr.bottom);
		res = rgn.CombineRgn(&rgn, &rgn2, RGN_DIFF);
		rgn2.DeleteObject();
	}
	if ((res != NULLREGION) && (res != ERROR))
	{
		CBrush br(m_bgColor);
		pDC->FillRgn(&rgn, &br);
		//
		if (m_pDoc != nullptr)
		{
			CBrush br(RGB(0, 0, 0));
			pr.InflateRect(1, 1);
			pDC->FrameRect(&pr, &br);
		}
	}
	rgn.DeleteObject();
	return TRUE;
}

afx_msg int CPDFPreviewView::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (__super::OnCreate(lpCreateStruct) == -1)
		return -1;

	OnInitialUpdate();
	return 0;
}

afx_msg void CPDFPreviewView::OnSize(UINT nType, int cx, int cy)
{
	__super::OnSize(nType, cx, cy);
	UpdateScrolls();
	SetPos(m_ptOffset.x, m_ptOffset.y);
}

void CPDFPreviewView::PaintRect(CDC* pDC, const CRect& paintRect)
{
	CRect pageRect = GetPageRect();
	CRect dr;
	dr.IntersectRect(pageRect, paintRect);
	if (dr.IsRectEmpty())
		return;
	// now lets calculate how dr maps to entire page in pixels
	CRect client;
	GetClientRect(client);
	client.IntersectRect(client, pageRect);
	client.OffsetRect(-pageRect.left, -pageRect.top);
	CRect cacheRect = dr;
	cacheRect.OffsetRect(-pageRect.left, -pageRect.top);
	if (EnsureCache(client))
	{
		// draw cached image
		m_pCache->DrawToDC((PXC::HANDLE_T)pDC->m_hDC, dr.left, dr.top, dr.Width(), dr.Height(),
						   cacheRect.left - m_rCachedRect.left, cacheRect.top - m_rCachedRect.top, 0);
	}
	else
	{
		pDC->FillSolidRect(&dr, RGB(255, 255, 255));
	}
}

afx_msg void CPDFPreviewView::OnPaint()
{
	// to have better performance we will redraw only portions that need to be redrawn
	// we have to get these regions before BeginPaint would be called

	CRect r;
	GetUpdateRect(&r);
	CRgn rgn;
	rgn.CreateRectRgnIndirect(&r);
	int reg = GetUpdateRgn(&rgn, FALSE);
	RGNDATA* data = nullptr;
	if (reg == COMPLEXREGION)
	{
		DWORD sz = GetRegionData(rgn, 0, NULL);
		if (sz)
		{
			data = (RGNDATA*)malloc(sz);
			GetRegionData(rgn, sz, data);
		}
		else
		{
			reg = ERROR;
		}
	}
	DeleteObject(rgn);
	//
	CPaintDC dc(this);
	if (reg == ERROR)
		return;
	if (data && (data->rdh.nCount > 0))
	{
		RECT* pr = (RECT*)data->Buffer;
		for (DWORD i = 0; i < data->rdh.nCount; i++)
		{
			PaintRect(&dc, pr[i]);
		}
	}
	else
	{
		if (r.IsRectEmpty())
			GetClientRect(r);
		PaintRect(&dc, r);
	}

	if (data != nullptr)
		free(data);
}

void CPDFPreviewView::SetPos(LONG posX, LONG posY)
{
	CPoint newOffset(posX, posY);
	FixupScrolls(newOffset);
	if (newOffset == m_ptOffset)
		return;
	LONG dx = m_ptOffset.x - newOffset.x;
	LONG dy = m_ptOffset.y - newOffset.y;
	m_ptOffset = newOffset;

	if (dx != 0 || dy != 0)
	{
		UpdateScrolls();
		ScrollWindow(dx, dy);
	}
}

afx_msg void CPDFPreviewView::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	LONG nNewPos = m_ptOffset.x;
	CRect cr;
	GetClientRect(cr);
	switch (nSBCode)
	{
	case SB_LEFT:
		nNewPos = 0;
		break;
	case SB_LINELEFT:
		nNewPos--;
		break;
	case SB_LINERIGHT:
		nNewPos++;
		break;
	case SB_PAGELEFT:
		nNewPos -= cr.Width();
		break;
	case SB_PAGERIGHT:
		nNewPos += cr.Width();
		break;
	case SB_RIGHT:
		nNewPos = GetTotalSize().cx;
		break;
	case SB_THUMBPOSITION:
	case SB_THUMBTRACK:
		nNewPos = (LONG)nPos;
		break;
	}
	SetPos(nNewPos, m_ptOffset.y);
}

afx_msg void CPDFPreviewView::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	LONG nNewPos = m_ptOffset.y;
	CRect cr;
	GetClientRect(cr);
	switch (nSBCode)
	{
	case SB_TOP:
		nNewPos = 0;
		break;
	case SB_LINEUP:
		nNewPos--;
		break;
	case SB_LINEDOWN:
		nNewPos++;
		break;
	case SB_PAGEUP:
		nNewPos -= cr.Height();
		break;
	case SB_PAGEDOWN:
		nNewPos += cr.Height();
		break;
	case SB_BOTTOM:
		nNewPos = GetTotalSize().cy;
		break;
	case SB_THUMBPOSITION:
	case SB_THUMBTRACK:
		nNewPos = (LONG)nPos;
		break;
	}
	SetPos(m_ptOffset.x, nNewPos);
}

BOOL CPDFPreviewView::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	if (nFlags & MK_CONTROL)
	{
		if (zDelta > 0)
			ZoomIn();
		else
			ZoomOut();
	}
	else
	{
		int dx = 0;
		int dy = 0;
		if (nFlags & MK_SHIFT)	// horz;
			dx = zDelta;
		else
			dy = zDelta;
		CPoint pt = m_ptOffset;
		pt.Offset(-dx, -dy);
		SetPos(pt.x, pt.y);
	}
	return TRUE;
}


void CPDFPreviewView::OnMouseMove(UINT nFlags, CPoint point)
{
	CPoint pnt(-1, -1);
	if (m_bInTrack)
	{
		CPoint pos = m_ptStartOffset + (m_ptStartPos - point);
		SetPos(pos.x, pos.y);
	}
}


void CPDFPreviewView::OnLButtonDown(UINT nFlags, CPoint point)
{
	if (GetFocus() != this)
		SetFocus();
	m_bInTrack = TRUE;
	SetCapture();
	SetCursor(m_cursors[1]);
	m_ptStartPos = point;
	m_ptStartOffset = m_ptOffset;
}


void CPDFPreviewView::OnLButtonUp(UINT nFlags, CPoint point)
{
	if (m_bInTrack)
	{
		m_bInTrack = FALSE;
		ReleaseCapture();
	}
	__super::OnLButtonUp(nFlags, point);
}


void CPDFPreviewView::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	// TODO: Add your message handler code here and/or call default

	CWnd::OnKeyDown(nChar, nRepCnt, nFlags);
}


BOOL CPDFPreviewView::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
	if (nHitTest == HTCLIENT)
	{
		SetCursor(m_cursors[0]);
		return TRUE;
	}
	else
	{
		return __super::OnSetCursor(pWnd, nHitTest, message);
	}
}
