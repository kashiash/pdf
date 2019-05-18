// PreviewWnd.cpp : implementation file
//

#include "stdafx.h"
#include "MFCSample.h"
#include "PDFPreviewWnd.h"

// CPDFPreviewWnd

IMPLEMENT_DYNAMIC(CPDFPreviewWnd, CFrameWndEx)

BEGIN_MESSAGE_MAP(CPDFPreviewWnd, CFrameWndEx)
	ON_WM_CREATE()
	ON_WM_SIZE()
	ON_COMMAND(ID_VIEW_ZOOMIN, &CPDFPreviewWnd::OnViewZoomin)
	ON_COMMAND(ID_VIEW_ZOOMOUT, &CPDFPreviewWnd::OnViewZoomout)
	ON_UPDATE_COMMAND_UI(ID_VIEW_ZOOMIN, &CPDFPreviewWnd::OnUpdateViewZoomin)
	ON_UPDATE_COMMAND_UI(ID_VIEW_ZOOMOUT, &CPDFPreviewWnd::OnUpdateViewZoomout)
	ON_COMMAND(ID_VIEW_PREV_PAGE, &CPDFPreviewWnd::OnViewPrevPage)
	ON_UPDATE_COMMAND_UI(ID_VIEW_PREV_PAGE, &CPDFPreviewWnd::OnUpdateViewPrevPage)
	ON_COMMAND(ID_VIEW_NEXT_PAGE, &CPDFPreviewWnd::OnViewNextPage)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NEXT_PAGE, &CPDFPreviewWnd::OnUpdateViewNextPage)
END_MESSAGE_MAP()

CPDFPreviewWnd::CPDFPreviewWnd()
{
}

CPDFPreviewWnd::~CPDFPreviewWnd()
{
}

BOOL CPDFPreviewWnd::Create(const RECT& rect, CWnd* pParentWnd, CCreateContext* pContext)
{
	const DWORD dwWndStyle = WS_POPUPWINDOW | WS_CAPTION | WS_THICKFRAME | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_MAXIMIZEBOX;
	const DWORD dwExtStyle = 0;

	CString strMyClass;
	strMyClass = AfxRegisterWndClass(CS_VREDRAW | CS_HREDRAW,
									 ::LoadCursor(NULL, IDC_ARROW),
									 (HBRUSH)::GetStockObject(WHITE_BRUSH),
									 0);

	BOOL bRes = __super::CreateEx(dwExtStyle, strMyClass, _T("Preview"), dwWndStyle, rect, pParentWnd, 0, pContext);
	DWORD err = GetLastError();
	return bRes;
}

int CPDFPreviewWnd::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (__super::OnCreate(lpCreateStruct) == -1)
		return -1;
	//
	CRect rectDummy;
	rectDummy.SetRectEmpty();
	// view
	m_wndView.Create(AFX_WS_DEFAULT_VIEW, rectDummy, this, AFX_IDW_PANE_FIRST);

	// toolbar

	// Load view images:
	m_wndToolbar.CreateEx(this, TBSTYLE_FLAT, WS_CHILD | WS_VISIBLE | CBRS_TOP | CBRS_GRIPPER | CBRS_TOOLTIPS | CBRS_FLYBY | CBRS_SIZE_DYNAMIC);
	m_wndToolbar.LoadToolBar(IDR_PREVIEW_TOOLBAR);
	//m_wndToolbar.CleanUpLockedImages();
	//m_wndToolbar.LoadBitmap(IDR_PREVIEW_TOOLBAR, 0, 0, TRUE /* Locked */);
	//m_wndToolbar.SetPaneStyle(m_wndToolbar.GetPaneStyle() | CBRS_TOOLTIPS | CBRS_FLYBY);
	//m_wndToolbar.SetPaneStyle(m_wndToolbar.GetPaneStyle() & ~(CBRS_GRIPPER | CBRS_SIZE_DYNAMIC | CBRS_BORDER_TOP | CBRS_BORDER_BOTTOM | CBRS_BORDER_LEFT | CBRS_BORDER_RIGHT));

	m_wndToolbar.EnableDocking(CBRS_ALIGN_ANY);
	m_wndToolbar.SetOwner(this);

	// All commands will be routed via this control , not via the parent frame:
	// m_wndToolbar.SetRouteCommandsViaFrame(FALSE);

	EnableDocking(CBRS_ALIGN_ANY);
	DockPane(&m_wndToolbar);

	m_wndView.SetZoom(100.0);
	m_wndView.SetFocus();
	//
//	AdjustLayout();

	return 0;
}

void CPDFPreviewWnd::OnSize(UINT nType, int cx, int cy)
{
	__super::OnSize(nType, cx, cy);
//	AdjustLayout();
}

void CPDFPreviewWnd::AdjustLayout()
{
	if (GetSafeHwnd() == NULL)
		return;
	CRect rectClient;
	GetClientRect(rectClient);
	int cyTlb = m_wndToolbar.CalcFixedLayout(FALSE, TRUE).cy;
	m_wndToolbar.SetWindowPos(nullptr, rectClient.left, rectClient.top, rectClient.Width(), cyTlb, SWP_NOACTIVATE | SWP_NOZORDER);
	rectClient.top += cyTlb;
	m_wndView.SetWindowPos(nullptr, rectClient.left, rectClient.top, rectClient.Width(), rectClient.Height(), SWP_NOACTIVATE | SWP_NOZORDER);
}

void CPDFPreviewWnd::OnViewZoomin()
{
	m_wndView.ZoomIn();
}

void CPDFPreviewWnd::OnViewZoomout()
{
	m_wndView.ZoomOut();
}

void CPDFPreviewWnd::OnUpdateViewZoomin(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(m_wndView.GetZoom() < Preview_Max_Zoom);
}

void CPDFPreviewWnd::OnUpdateViewZoomout(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(m_wndView.GetZoom() > Preview_Min_Zoom);
}

void CPDFPreviewWnd::OnViewPrevPage()
{
	ULONG nPage = m_wndView.GetCurPage();
	if (nPage > 0)
		m_wndView.SetCurPage(nPage - 1);
}

void CPDFPreviewWnd::OnUpdateViewPrevPage(CCmdUI *pCmdUI)
{
	ULONG nPage = m_wndView.GetCurPage();
	pCmdUI->Enable(nPage > 0);
}

void CPDFPreviewWnd::OnViewNextPage()
{
	ULONG nPage = m_wndView.GetCurPage();
	m_wndView.SetCurPage(nPage + 1);
}

void CPDFPreviewWnd::OnUpdateViewNextPage(CCmdUI *pCmdUI)
{
	ULONG nPage = m_wndView.GetCurPage();
	pCmdUI->Enable((nPage + 1) < m_wndView.GetNumPages());
}
