#pragma once

#include "Common_routines.h"

class CSDKSample
{
public:
	CSDKSample();
	virtual ~CSDKSample();
public:
	virtual LPCWSTR GetGroup() = 0;
	virtual LPCWSTR GetTitle() = 0;
	virtual LPCWSTR GetDescription() = 0;
	//
	virtual CWnd*	GetDialog(CWnd* pParent);
	//
	virtual HRESULT Perform() = 0;
protected:
	CWnd*	m_pWindow;
public:
	BOOL	m_bSavePDF;
	BOOL	m_bOpenPDF;
	BOOL	m_bPreviewPDF;
};
