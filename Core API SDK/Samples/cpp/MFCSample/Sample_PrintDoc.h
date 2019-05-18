#pragma once
#include "Sample.h"


class CSDKSample_PrintDoc : public CSDKSample
{
public:
	CSDKSample_PrintDoc();
	virtual ~CSDKSample_PrintDoc();
public:
	CWnd* GetDialog(CWnd* pParent) override;
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
public:
	CString			m_sFileName;
	void PrintDoc(PXC::IPXC_Document* pDoc, HWND hWnd);
};

