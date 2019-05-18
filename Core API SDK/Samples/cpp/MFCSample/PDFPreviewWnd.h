#pragma once

#include "PDFPreviewView.h"

class CPDFPreviewToolbar : public CMFCToolBar
{
public:
	virtual BOOL AllowShowOnList() const { return FALSE; }
};

// CPDFPreviewWnd
class CPDFPreviewWnd : public CFrameWndEx
{
	DECLARE_DYNAMIC(CPDFPreviewWnd)
public:
	CPDFPreviewWnd();
	virtual ~CPDFPreviewWnd();
public:
	BOOL Create(const RECT& rect, CWnd* pParentWnd, CCreateContext* pContext = NULL);
public:
	void SetDocument(PXC::IPXC_Document* pDoc)
	{
		m_wndView.SetDocument(pDoc);
	}

protected:
	void AdjustLayout();
public:
	CPDFPreviewToolbar		m_wndToolbar;
	CPDFPreviewView			m_wndView;
protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnViewZoomin();
	afx_msg void OnViewZoomout();
//	afx_msg void OnViewTransparency();
//	afx_msg void OnUpdateViewTransparency(CCmdUI *pCmdUI);
	afx_msg void OnUpdateViewZoomin(CCmdUI *pCmdUI);
	afx_msg void OnUpdateViewZoomout(CCmdUI *pCmdUI);
	afx_msg void OnViewPrevPage();
	afx_msg void OnUpdateViewPrevPage(CCmdUI *pCmdUI);
	afx_msg void OnViewNextPage();
	afx_msg void OnUpdateViewNextPage(CCmdUI *pCmdUI);
};


