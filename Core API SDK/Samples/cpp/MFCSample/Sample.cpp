#include "stdafx.h"
#include "Sample.h"
#include "resource.h"

//////////////////////////////////////////////////////////////////////////
class CBaseSampleDlg : public CDialogEx
{
public:
	enum { IDD = IDD_DLG_BASE };
	CBaseSampleDlg(CWnd* pParent);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
// Implementation
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCmdRun();
public:
	CSDKSample*		m_pSample;
public:
	BOOL m_bSavePDF;
	BOOL m_bOpenSavedPDF;
	BOOL m_bOpenPreview;
};

CBaseSampleDlg::CBaseSampleDlg(CWnd* pParent) : CDialogEx(IDD, pParent)
{
	m_pSample		= nullptr;
}

void CBaseSampleDlg::DoDataExchange(CDataExchange* pDX)
{
	__super::DoDataExchange(pDX);
	if (m_pSample)
	{
		DDX_Check(pDX, IDC_B_SAVE_DOC,  m_pSample->m_bSavePDF);
		DDX_Check(pDX, IDC_B_OPEN_FILE, m_pSample->m_bOpenPDF);
		DDX_Check(pDX, IDC_B_RUN_PREVIEW, m_pSample->m_bPreviewPDF);
	}
}

void CBaseSampleDlg::OnCmdRun()
{
	UpdateData(TRUE);
	if (m_pSample)
		m_pSample->Perform();
}

BEGIN_MESSAGE_MAP(CBaseSampleDlg, CDialogEx)
	ON_BN_CLICKED(IDC_CMD_RUN, &CBaseSampleDlg::OnCmdRun)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////
CSDKSample::CSDKSample()
{
	m_pWindow	= nullptr;
	//
	m_bSavePDF		= FALSE;
	m_bOpenPDF		= FALSE;
	m_bPreviewPDF	= TRUE;
}

CSDKSample::~CSDKSample()
{
	if (m_pWindow)
	{
		delete m_pWindow;
		m_pWindow = nullptr;
	}
}

CWnd* CSDKSample::GetDialog(CWnd* pParent)
{
	if (m_pWindow == nullptr)
	{
		CBaseSampleDlg* pDlg = new CBaseSampleDlg(pParent);
		pDlg->m_pSample = this;
		pDlg->Create(CBaseSampleDlg::IDD, pParent);
		m_pWindow = pDlg;
	}
	return m_pWindow;
}
