#pragma once

#include "Sample.h"

class CSDKSample_ExportToImage : public CSDKSample
{
public:
	CSDKSample_ExportToImage();
	virtual ~CSDKSample_ExportToImage();
public:
	CWnd* GetDialog(CWnd* pParent) override;
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
public:
	CString					m_sPDFFileName;
	CString					m_sImageFileName;
	DWORD					m_nPageNumber;
	DWORD					m_nImageFormat;
	DWORD					m_nDPI;
	PXC::PXC_TextSmoothMode	m_nTextSmoothMode;
	PXC::IXC_PageFormat		m_nPageFormat;
	bool					m_bSmoothLineArts;
};

