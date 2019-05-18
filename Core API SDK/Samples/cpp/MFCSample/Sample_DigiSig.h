#pragma once
#include "Sample.h"

class CSDKSample_DigiSig : public CSDKSample
{
public:
	CSDKSample_DigiSig();
	virtual ~CSDKSample_DigiSig();
public:
	CWnd* GetDialog(CWnd* pParent) override;
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
public:
	CString			m_sPDFFileName;
	CString			m_sPFXFileName;
	CString			m_sPFXPassword;
};

