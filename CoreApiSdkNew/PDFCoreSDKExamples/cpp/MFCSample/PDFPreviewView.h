#pragma once

// CPDFPreviewView view

// CScrollView

// constants for minimum and maximum zoom level
#define Preview_Min_Zoom		10.0
#define Preview_Max_Zoom		800.0

class CPDFPreviewView : public CWnd
{
	DECLARE_DYNAMIC(CPDFPreviewView)
public:
	CPDFPreviewView();           // protected constructor used by dynamic creation
	virtual ~CPDFPreviewView();
public:
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif
public:
	BOOL Create(DWORD dwStyle, RECT& rc, CWnd* pParentWnd, ULONG nID);
public:
	void SetDocument(PXC::IPXC_Document* pDoc);
	void SetZoom(double nZoomLevel);
	void ZoomIn();
	void ZoomOut();
	ULONG GetNumPages() const;
	void SetCurPage(ULONG nPage);
	ULONG GetCurPage() const
	{
		return m_nCurPage;
	}
	double GetZoom() const
	{
		return m_ZoomLevel;
	}
public:
	CSize GetPageSize();
protected:
	CSize CalcPageSize();
protected:
	void ReleaseCache();
	bool EnsureCache(const CRect& pr);
	void UpdateScrolls();
	bool FixupScrolls(CPoint& offs) const;
	// returns rect of the page (w/o gaps) relative to the client area
	CRect GetPageRect() const;
	// returns size of view area: page + gaps. in pixels
	CSize GetTotalSize() const;
	void PaintRect(CDC* pDC, const CRect& pr);
	void SetPos(LONG posX, LONG posY);
protected:
	virtual void OnInitialUpdate();
protected:
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnPaint();
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	DECLARE_MESSAGE_MAP()
protected:
	CComPtr<PXC::IPXC_Document>		m_pDoc;
	DWORD							m_nCurPage;
	CComPtr<PXC::IIXC_Page>			m_pCache;
	CRect							m_rCachedRect;		// cached part of page, subrect from [0, 0, m_szPageSize.cx, m_szPageSize.cy]
	CSize							m_szPageSize;		// page size in pixels with current zoom
	CPoint							m_ptOffset;			// scroll offset
	double							m_nCoef;
	double							m_ZoomLevel;
	PXC::PXC_Matrix					m_Matrix;
	// navigation
	BOOL							m_bInTrack;
	CPoint							m_ptStartPos;
	CPoint							m_ptStartOffset;
	HCURSOR							m_cursors[2];
	//
	COLORREF						m_bgColor;
public:
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
};


