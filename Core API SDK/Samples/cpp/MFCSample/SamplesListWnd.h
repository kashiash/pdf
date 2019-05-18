#pragma once

#include "SearchEdit.h"

class CSamplesListWnd : public CDockablePane
{
// Construction
public:
	CSamplesListWnd();
	virtual ~CSamplesListWnd();
// Attributes
protected:
	CSearchEdit		m_FilterEdit;
	CTreeCtrl		m_SamplesTree;
public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
public:
	void UpdateFonts();
protected:
	void FillSamplesTree();
	void AdjustLayout();
protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnSampleSelected(NMHDR* pNotifyStruct, LRESULT* pResult);
	afx_msg void OnSearchChange();

	DECLARE_MESSAGE_MAP()
};
