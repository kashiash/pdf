#pragma once

class CSearchEdit : public CEdit
{
	DECLARE_DYNCREATE(CSearchEdit)
protected:
	enum BtnState
	{
		bs_Normal,
		bs_Hover,
		bs_Down,
		//
		bs_Max_
	};

	bool			m_bLarge;
	bool			m_bHasText;
	BtnState		m_nBtnState;
	CImageList		m_Images;
	CRect			m_rcEdges;
	COLORREF		m_clrBtn[bs_Max_];
public:
	CSearchEdit();
	virtual ~CSearchEdit();
public:
	virtual void PreSubclassWindow() override;
	virtual BOOL PreTranslateMessage(MSG* pMsg) override;
public:
	int GetIdealHeight();
protected:
	void Initialize();
	void ResizeWindow();
	void GetButtonRect(const CRect& wr, CRect& rc);
	int GetImageSize() const { return m_bLarge ? 20 : 16; }
	void InvalidateButton();
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg LRESULT OnSetFont(WPARAM wParam, LPARAM lParam);
	afx_msg void OnNcCalcSize(BOOL bCalcValidRects, NCCALCSIZE_PARAMS* lpncsp);
	afx_msg void OnNcPaint();
	afx_msg void OnChanged();
};
