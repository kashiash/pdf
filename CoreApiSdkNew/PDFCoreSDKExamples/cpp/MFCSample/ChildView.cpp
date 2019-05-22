// This MFC Samples source code demonstrates using MFC Microsoft Office Fluent User Interface 
// (the "Fluent UI") and is provided only as referential material to supplement the 
// Microsoft Foundation Classes Reference and related electronic documentation 
// included with the MFC C++ library software.  
// License terms to copy, use or distribute the Fluent UI are available separately.  
// To learn more about our Fluent UI licensing program, please visit 
// http://go.microsoft.com/fwlink/?LinkId=238214.
//
// Copyright (C) Microsoft Corporation
// All rights reserved.

// ChildView.cpp : implementation of the CChildView class
//

#include "stdafx.h"
#include "MFCSample.h"
#include "ChildView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CChildView

CChildView::CChildView()
{
	m_pSampleDlg	= nullptr;
}

CChildView::~CChildView()
{
}

BEGIN_MESSAGE_MAP(CChildView, CWnd)
	ON_WM_SIZE()
	ON_WM_PAINT()
END_MESSAGE_MAP()

void CChildView::SetSampleDlg(CWnd* pSampleDlg)
{
	if (pSampleDlg == m_pSampleDlg)
		return;
	if (m_pSampleDlg)
	{
		m_pSampleDlg->ShowWindow(SW_HIDE);
	}
	m_pSampleDlg = pSampleDlg;
	if (m_pSampleDlg)
	{
		CRect cr;
		GetClientRect(cr);
		m_pSampleDlg->SetWindowPos(nullptr, cr.left, cr.top, cr.Width(), cr.Height(), SWP_NOZORDER);
		m_pSampleDlg->ShowWindow(SW_SHOW);
	}
}

// CChildView message handlers

BOOL CChildView::PreCreateWindow(CREATESTRUCT& cs) 
{
	if (!CWnd::PreCreateWindow(cs))
		return FALSE;

	cs.dwExStyle |= WS_EX_CLIENTEDGE;
	cs.style &= ~WS_BORDER;
	cs.lpszClass = AfxRegisterWndClass(CS_HREDRAW|CS_VREDRAW|CS_DBLCLKS, 
		::LoadCursor(NULL, IDC_ARROW), reinterpret_cast<HBRUSH>(COLOR_WINDOW+1), NULL);

	return TRUE;
}

void CChildView::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
}

void CChildView::OnSize(UINT nType, int cx, int cy)
{
	CWnd::OnSize(nType, cx, cy);
	if (m_pSampleDlg)
	{
		CRect cr;
		GetClientRect(cr);
		m_pSampleDlg->SetWindowPos(nullptr, cr.left, cr.top, cr.Width(), cr.Height(), SWP_NOZORDER | SWP_NOACTIVATE);
		m_pSampleDlg->ShowWindow(SW_SHOW);
	}
}