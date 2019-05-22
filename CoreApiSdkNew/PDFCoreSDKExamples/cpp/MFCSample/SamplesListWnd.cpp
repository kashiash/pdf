#include "stdafx.h"

#include "SamplesListWnd.h"
#include "Resource.h"
#include "MainFrm.h"
#include "MFCSample.h"

#include <map>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define TREE_LIST_ID		2
#define EDIT_SEARCH_ID		3

/////////////////////////////////////////////////////////////////////////////
// CSamplesListWnd

CSamplesListWnd::CSamplesListWnd()
{
}

CSamplesListWnd::~CSamplesListWnd()
{
}

BOOL CSamplesListWnd::PreTranslateMessage(MSG* pMsg)
{
	if (m_FilterEdit.m_hWnd != nullptr)
	{
		if (m_FilterEdit.PreTranslateMessage(pMsg))
			return TRUE;
	}
	return __super::PreTranslateMessage(pMsg);
}

int CSamplesListWnd::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CDockablePane::OnCreate(lpCreateStruct) == -1)
		return -1;

	CRect rectDummy;
	rectDummy.SetRectEmpty();

	// Create filter edit
	if (!m_FilterEdit.Create(WS_CHILD | WS_VISIBLE | ES_AUTOHSCROLL, rectDummy, this, EDIT_SEARCH_ID))
	{
		TRACE0("Failed to filter edit\n");
		return -1;      // fail to create
	}
	m_FilterEdit.ModifyStyleEx(0, WS_EX_CLIENTEDGE | WS_EX_NOPARENTNOTIFY);

	CString strCue;
	strCue.LoadString(IDS_FILTER_CUE);
	m_FilterEdit.SetCueBanner(strCue);


	// Create tree
	const DWORD dwStyle = WS_CHILD | WS_VISIBLE | TVS_HASBUTTONS | TVS_HASLINES | TVS_LINESATROOT | TVS_DISABLEDRAGDROP | TVS_SHOWSELALWAYS | TVS_INFOTIP | TVS_TRACKSELECT | TVS_FULLROWSELECT;
	if (!m_SamplesTree.Create(dwStyle, rectDummy, this, TREE_LIST_ID))
	{
		TRACE0("Failed to create samples tree window\n");
		return -1;      // fail to create
	}
	m_SamplesTree.ModifyStyleEx(0, WS_EX_CLIENTEDGE |  WS_EX_NOPARENTNOTIFY | TVS_EX_DOUBLEBUFFER | TVS_EX_AUTOHSCROLL);

	SetWindowTheme(m_SamplesTree, L"Explorer", NULL);
	FillSamplesTree();

	UpdateFonts();
	return 0;
}

void CSamplesListWnd::OnSize(UINT nType, int cx, int cy)
{
	CDockablePane::OnSize(nType, cx, cy);
	AdjustLayout();
}

BOOL CSamplesListWnd::OnEraseBkgnd(CDC* pDC)
{
	CRect rc;
	GetClientRect(rc);
	pDC->FillSolidRect(rc, GetSysColor(COLOR_WINDOW));
	return TRUE;
}

void CSamplesListWnd::AdjustLayout()
{
	if (GetSafeHwnd() == NULL)
		return;

	CRect rectClient;
	GetClientRect(rectClient);

	int cyTlb = m_FilterEdit.GetIdealHeight();	// m_wndToolBar.CalcFixedLayout(FALSE, TRUE).cy;

	m_FilterEdit.SetWindowPos(NULL, rectClient.left, rectClient.top, rectClient.Width(), cyTlb, SWP_NOACTIVATE | SWP_NOZORDER);
	m_SamplesTree.SetWindowPos(NULL, rectClient.left + 1, rectClient.top + cyTlb + 1, rectClient.Width() - 2, rectClient.Height() - cyTlb - 2, SWP_NOACTIVATE | SWP_NOZORDER);

	//int w = rectClient.Width() - 2 - GetSystemMetrics(SM_CXVSCROLL);
	//m_wndFontsView.SetColumnWidth(0, w);
}

bool CheckSampleByFilter(CSDKSample* pSample, const CStringW& sFilter)
{
	return true;
}

void GetFilteredList(std::vector<CSDKSample*>& slist, const CStringW& sFilter)
{
	for (auto* pS : theApp.m_Samples)
	{
		if (CheckSampleByFilter(pS, sFilter))
			slist.push_back(pS);
	}
}

void CSamplesListWnd::FillSamplesTree()
{
	std::vector<CSDKSample*> slist;
	CStringW sFilter;
	m_FilterEdit.GetWindowText(sFilter);
	sFilter.Trim();
	GetFilteredList(slist, sFilter);
	// build a tree
	m_SamplesTree.DeleteAllItems();

	std::map<CString, HTREEITEM> roots;

	for (auto* pS : slist)
	{
		HTREEITEM hParent = TVI_ROOT;
		CString gName = pS->GetGroup();
		auto& pos = roots.find(gName);
		if (pos == roots.end())
		{
			// need to add new root item
			hParent = m_SamplesTree.InsertItem(gName);
			roots[gName] = hParent;
		}
		else
		{
			hParent = pos->second;
		}
		HTREEITEM hItem = m_SamplesTree.InsertItem(pS->GetTitle(), hParent);
		m_SamplesTree.SetItemData(hItem, (DWORD_PTR)pS);
	}
	// expand all
	for (auto& it : roots)
	{
		m_SamplesTree.Expand(it.second, TVE_EXPAND);
	}
}

void CSamplesListWnd::UpdateFonts()
{
	m_FilterEdit.SetFont(&afxGlobalData.fontRegular);
	m_SamplesTree.SetFont(&afxGlobalData.fontRegular);
	AdjustLayout();
}

void CSamplesListWnd::OnSampleSelected(NMHDR* pNotifyStruct, LRESULT* pResult)
{
	LPNMTREEVIEW pNotify = (LPNMTREEVIEW)pNotifyStruct;
	*pResult = 0;
	CSDKSample* pSample = (CSDKSample*)m_SamplesTree.GetItemData(pNotify->itemNew.hItem);
	if (pSample == nullptr)
		return;
	static_cast<CMainFrame*>(theApp.GetMainWnd())->SetCurrentSample(pSample);
}

void CSamplesListWnd::OnSearchChange()
{
	FillSamplesTree();
}

//////////////////////////////////////////////////////////////////////////
BEGIN_MESSAGE_MAP(CSamplesListWnd, CDockablePane)
	ON_WM_CREATE()
	ON_WM_SIZE()
	ON_WM_ERASEBKGND()
	ON_NOTIFY(TVN_SELCHANGED, TREE_LIST_ID, OnSampleSelected)
	//ON_WM_PARENTNOTIFY
	ON_EN_CHANGE(EDIT_SEARCH_ID, OnSearchChange)
END_MESSAGE_MAP()
