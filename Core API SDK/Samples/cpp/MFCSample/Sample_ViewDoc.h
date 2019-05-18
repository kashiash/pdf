#pragma once
#include "Sample.h"


class CSDKSample_ViewDoc : public CSDKSample
{
public:
	CSDKSample_ViewDoc();
	virtual ~CSDKSample_ViewDoc();
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
};

