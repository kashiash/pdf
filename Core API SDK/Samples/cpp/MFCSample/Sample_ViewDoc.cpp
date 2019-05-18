#include "stdafx.h"
#include "Sample_ViewDoc.h"
#include "MainFrm.h"
#include "MFCSample.h"
#include "resource.h"
#include "InitializeSDK.h"

//////////////////////////////////////////////////////////////////////////
class CDlg_OpenDoc : public CDialogEx
{
public:
	enum { IDD = IDD_DLG_OPEN_DOC };
	CDlg_OpenDoc(CWnd* pParent);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
// Implementation
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCmdRun();
	afx_msg void OnBrowse();
public:
	CSDKSample_ViewDoc*		m_pSample;
};

CDlg_OpenDoc::CDlg_OpenDoc(CWnd* pParent) : CDialogEx(IDD, pParent)
{
	m_pSample		= nullptr;
}

void CDlg_OpenDoc::DoDataExchange(CDataExchange* pDX)
{
	__super::DoDataExchange(pDX);
	if (m_pSample)
		DDX_Text(pDX, IDC_FILE_NAME, m_pSample->m_sFileName);
}

void CDlg_OpenDoc::OnCmdRun()
{
	UpdateData(TRUE);
	if (m_pSample)
		m_pSample->Perform();
}

void CDlg_OpenDoc::OnBrowse()
{
	if (m_pSample == nullptr)
		return;
	UpdateData(TRUE);
	CFileDialog dlg(TRUE, L"pdf", m_pSample->m_sFileName, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
					L"PDF Document|*.pdf|All Files|*.*||", this);
	if (dlg.DoModal())
	{
		m_pSample->m_sFileName = dlg.GetPathName();
		UpdateData(FALSE);
	}
}

BEGIN_MESSAGE_MAP(CDlg_OpenDoc, CDialogEx)
	ON_BN_CLICKED(IDC_CMD_RUN, &CDlg_OpenDoc::OnCmdRun)
	ON_BN_CLICKED(IDC_BROWSE, &CDlg_OpenDoc::OnBrowse)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////
CSDKSample_ViewDoc::CSDKSample_ViewDoc()
{
}


CSDKSample_ViewDoc::~CSDKSample_ViewDoc()
{
}

LPCWSTR CSDKSample_ViewDoc::GetGroup()
{
	return L"View";
}

LPCWSTR CSDKSample_ViewDoc::GetTitle()
{
	return L"Document Preview";
}

LPCWSTR CSDKSample_ViewDoc::GetDescription()
{
	return L"";
}

CWnd* CSDKSample_ViewDoc::GetDialog(CWnd* pParent)
{
	if (m_pWindow == nullptr)
	{
		CDlg_OpenDoc* pDlg = new CDlg_OpenDoc(pParent);
		pDlg->m_pSample = this;
		pDlg->Create(CDlg_OpenDoc::IDD, pParent);
		m_pWindow = pDlg;
	}
	return m_pWindow;
}

HRESULT CSDKSample_ViewDoc::Perform()
{
	HRESULT hr = S_OK;
	CComPtr<PXC::IPXC_Document> pDoc;
	do
	{
		hr = OpenDocFromFile(m_sFileName, pDoc);
		if (pDoc)
		{
			//do 
			//{
			//	CComPtr<PXC::IPXC_XMPMetadata> pMeta;
			//	pDoc->GetXMPMetadata(VARIANT_TRUE, &pMeta);

			//	HANDLE hF = CreateFile(LR"(e:\temp\47\ZUGFeRD_extension_schema.xmp)", GENERIC_READ, FILE_SHARE_READ, nullptr, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);
			//	if (hF == INVALID_HANDLE_VALUE)
			//		break;
			//	DWORD nSize = GetFileSize(hF, nullptr);
			//	BYTE* pBuf = (BYTE*)malloc(nSize);
			//	DWORD nRead = 0;
			//	ReadFile(hF, pBuf, nSize, &nRead, nullptr);
			//	CloseHandle(hF);

			//	CComPtr<PXC::IMemBlock> mb;
			//	g_AUX->CreateMemBlock(nSize, &mb);
			//	mb->SetData(pBuf, nSize);
			//	free(pBuf);
			//	pMeta->SetXMP(mb);
			//	//
			//	CStringW fn;
			//	SaveDocument(pDoc, fn);
			//} while (false);
		}
		if (pDoc != nullptr)
			static_cast<CMainFrame*>(theApp.GetMainWnd())->ShowPreview(pDoc);
	} while (false);
	return hr;
}
