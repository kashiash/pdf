#include "stdafx.h"
#include "Sample_DigiSig.h"
#include "MainFrm.h"
#include "MFCSample.h"
#include "resource.h"

//////////////////////////////////////////////////////////////////////////
class CDlg_SignDoc : public CDialogEx
{
public:
	enum { IDD = IDD_DLG_DIGISIG };
	CDlg_SignDoc(CWnd* pParent);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	// Implementation
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCmdRun();
	afx_msg void OnPDFBrowse();
	afx_msg void OnPFXBrowse();
public:
	CSDKSample_DigiSig*		m_pSample;
};

CDlg_SignDoc::CDlg_SignDoc(CWnd* pParent) : CDialogEx(IDD, pParent)
{
	m_pSample = nullptr;
}

void CDlg_SignDoc::DoDataExchange(CDataExchange* pDX)
{
	__super::DoDataExchange(pDX);
	if (m_pSample)
	{
		DDX_Text(pDX, IDC_FILE_NAME, m_pSample->m_sPDFFileName);
		DDX_Text(pDX, IDC_PFX_FILE_NAME, m_pSample->m_sPFXFileName);
		DDX_Text(pDX, IDC_PASSWORD, m_pSample->m_sPFXPassword);
	}
}

void CDlg_SignDoc::OnCmdRun()
{
	UpdateData(TRUE);
	if (m_pSample)
		m_pSample->Perform();
}

void CDlg_SignDoc::OnPDFBrowse()
{
	if (m_pSample == nullptr)
		return;
	UpdateData(TRUE);
	CFileDialog dlg(TRUE, L"pdf", m_pSample->m_sPDFFileName, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
					L"PDF Document|*.pdf|All Files|*.*||", this);
	if (dlg.DoModal())
	{
		m_pSample->m_sPDFFileName = dlg.GetPathName();
		UpdateData(FALSE);
	}
}

void CDlg_SignDoc::OnPFXBrowse()
{
	if (m_pSample == nullptr)
		return;
	UpdateData(TRUE);
	CFileDialog dlg(TRUE, L"pfx", m_pSample->m_sPFXFileName, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
					L"Personal Information Exchange (*.pfx, *.p12)|*.pfx;*.p12||", this);
	if (dlg.DoModal())
	{
		m_pSample->m_sPFXFileName = dlg.GetPathName();
		UpdateData(FALSE);
	}
}

BEGIN_MESSAGE_MAP(CDlg_SignDoc, CDialogEx)
	ON_BN_CLICKED(IDC_CMD_RUN, &OnCmdRun)
	ON_BN_CLICKED(IDC_BROWSE, &OnPDFBrowse)
	ON_BN_CLICKED(IDC_BROWSE_PFX, &OnPFXBrowse)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////
CSDKSample_DigiSig::CSDKSample_DigiSig()
{
}


CSDKSample_DigiSig::~CSDKSample_DigiSig()
{
}

LPCWSTR CSDKSample_DigiSig::GetGroup()
{
	return L"Digital Sign";
}

LPCWSTR CSDKSample_DigiSig::GetTitle()
{
	return L"Sign Existing PDF";
}

LPCWSTR CSDKSample_DigiSig::GetDescription()
{
	return L"Sign existing PDF document creating new signature field with widget on the first page.";
}

CWnd* CSDKSample_DigiSig::GetDialog(CWnd* pParent)
{
	if (m_pWindow == nullptr)
	{
		CDlg_SignDoc* pDlg = new CDlg_SignDoc(pParent);
		pDlg->m_pSample = this;
		pDlg->Create(CDlg_SignDoc::IDD, pParent);
		m_pWindow = pDlg;
	}
	return m_pWindow;
}

HRESULT CSDKSample_DigiSig::Perform()
{
	HRESULT hr = S_OK;
	CComPtr<PXC::IPXC_Document> pDoc;
	do
	{
		hr = OpenDocFromFile(m_sPDFFileName, pDoc);
		if (pDoc == nullptr)
			break;
		CString sNewFileName;
		int nPos = m_sPDFFileName.ReverseFind(L'.');
		if (nPos < 0)
			sNewFileName = m_sPDFFileName;
		else
			sNewFileName = m_sPDFFileName.Left(nPos);
		sNewFileName.Append(L"_signed.pdf");
		//
		DWORD nFlags = PXC::Sign_GR_Name |
			PXC::Sign_TX_Name | PXC::Sign_TX_Date | PXC::Sign_TX_Reason | PXC::Sign_TX_Labels | PXC::Sign_TX_Subject | PXC::Sign_TX_Logo;
		PXC::PXC_Rect pr;
		pr.left = 72.0;
		pr.bottom = 72.0;
		pr.right = pr.left + 216;
		pr.top = pr.bottom + 72;
		hr = pDoc->DeferedDigitalPFX((LPWSTR)(LPCWSTR)m_sPFXFileName, (LPWSTR)(LPCWSTR)m_sPFXPassword, nFlags, 0, &pr, L"PDF-XChange Core API demonstation", L"", L"support@tracker-software.com", L"");
		BreakOnFailure(hr, L"Error adding defer signature information (hr = 0x%.8lx)", hr);
		hr = SaveDocument(pDoc, sNewFileName, TRUE);
	} while (false);
	return hr;
}
