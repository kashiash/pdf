#include "stdafx.h"
#include "Sample_PrintDoc.h"
#include "MainFrm.h"
#include "MFCSample.h"
#include "resource.h"
#include "InitializeSDK.h"

//////////////////////////////////////////////////////////////////////////
class CDlg_OpenPrintDoc : public CDialogEx
{
public:
	enum { IDD = IDD_DLG_OPEN_DOC };
	CDlg_OpenPrintDoc(CWnd* pParent);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	// Implementation
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCmdRun();
	afx_msg void OnBrowse();
public:
	CSDKSample_PrintDoc*		m_pSample;
};

CDlg_OpenPrintDoc::CDlg_OpenPrintDoc(CWnd* pParent) : CDialogEx(IDD, pParent)
{
	m_pSample = nullptr;
}

void CDlg_OpenPrintDoc::DoDataExchange(CDataExchange* pDX)
{
	__super::DoDataExchange(pDX);
	if (m_pSample)
		DDX_Text(pDX, IDC_FILE_NAME, m_pSample->m_sFileName);
}

void CDlg_OpenPrintDoc::OnCmdRun()
{
	UpdateData(TRUE);
	if (m_pSample)
		m_pSample->Perform();
}

void CDlg_OpenPrintDoc::OnBrowse()
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

BEGIN_MESSAGE_MAP(CDlg_OpenPrintDoc, CDialogEx)
	ON_BN_CLICKED(IDC_CMD_RUN, &CDlg_OpenPrintDoc::OnCmdRun)
	ON_BN_CLICKED(IDC_BROWSE, &CDlg_OpenPrintDoc::OnBrowse)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////
CSDKSample_PrintDoc::CSDKSample_PrintDoc()
{
}


CSDKSample_PrintDoc::~CSDKSample_PrintDoc()
{
}

LPCWSTR CSDKSample_PrintDoc::GetGroup()
{
	return L"Print";
}

LPCWSTR CSDKSample_PrintDoc::GetTitle()
{
	return L"Document Printing";
}

LPCWSTR CSDKSample_PrintDoc::GetDescription()
{
	return L"";
}

CWnd* CSDKSample_PrintDoc::GetDialog(CWnd* pParent)
{
	if (m_pWindow == nullptr)
	{
		CDlg_OpenPrintDoc* pDlg = new CDlg_OpenPrintDoc(pParent);
		pDlg->m_pSample = this;
		pDlg->Create(CDlg_OpenPrintDoc::IDD, pParent);
		m_pWindow = pDlg;
	}
	return m_pWindow;
}

HRESULT CSDKSample_PrintDoc::Perform()
{
	HRESULT hr = S_OK;
	CComPtr<PXC::IPXC_Document> pDoc;
	do
	{
		hr = OpenDocFromFile(m_sFileName, pDoc);
		if (pDoc != nullptr)
		{
			//static_cast<CMainFrame*>(theApp.GetMainWnd())->ShowPreview(pDoc);
			PrintDoc(pDoc, m_pWindow->m_hWnd);
		}
	} while (false);
	return hr;
}

void CSDKSample_PrintDoc::PrintDoc(PXC::IPXC_Document* pDoc, HWND hWnd)
{
	if (pDoc == nullptr)
		return;

	CComPtr<PXC::IPXC_Pages> pPages;
	HRESULT hr = pDoc->get_Pages(&pPages);
	if (FAILED(hr))
		return;

	DWORD pagescount = 0;
	hr = pPages->get_Count(&pagescount);
	if (FAILED(hr))
		return;
	//
	PRINTDLG pdlg = { sizeof(PRINTDLG) };
	//
	pdlg.hwndOwner = hWnd;
	pdlg.Flags = PD_NOSELECTION | PD_RETURNDC;
	pdlg.nMinPage = 1;
	pdlg.nMaxPage = (WORD)pagescount;
	pdlg.nCopies = 1;

	if (!PrintDlg(&pdlg))
		return;
	DWORD nStartPage = 1;
	DWORD nEndPage = pagescount;
	if (pdlg.Flags & PD_PAGENUMS)
	{
		nStartPage = pdlg.nFromPage;
		nEndPage = pdlg.nToPage;
	}
	nStartPage--;
	nEndPage--;
	size_t i;

	DOCINFO di = { sizeof(DOCINFO) };
	di.lpszDocName = _T("PXC-View PDF Print");
	// Now Getting Information from DC
	SIZE physicalSize;
	SIZE margin;
	SIZE printigSize;
	physicalSize.cx = GetDeviceCaps(pdlg.hDC, PHYSICALWIDTH);
	physicalSize.cy = GetDeviceCaps(pdlg.hDC, PHYSICALHEIGHT);
	margin.cx = GetDeviceCaps(pdlg.hDC, PHYSICALOFFSETX);
	margin.cy = GetDeviceCaps(pdlg.hDC, PHYSICALOFFSETY);
	printigSize.cx = GetDeviceCaps(pdlg.hDC, HORZRES);
	printigSize.cy = GetDeviceCaps(pdlg.hDC, VERTRES);
	//
	CRect or_prect;
	or_prect.left = margin.cx;
	or_prect.right = or_prect.left + printigSize.cx;
	or_prect.top = margin.cy;
	or_prect.bottom = or_prect.top + printigSize.cy;
	//
	if (::StartDoc(pdlg.hDC, &di) > 0)
	{

		for (i = nStartPage; i <= nEndPage; i++)
		{
			::StartPage(pdlg.hDC);
			//
			CComPtr<PXC::IPXC_Page> pPage;
			hr = pPages->get_Item((PXC::ULONG_T)i, &pPage);
			if (FAILED(hr))
				return;

			double doc_page_w, doc_page_h;
			hr = pPage->GetDimension(&doc_page_w, &doc_page_h);
			if (FAILED(hr))
				return;

			PXC::PXC_Matrix dm = {0};
			pPage->GetMatrix(PXC::PBox_MediaBox, &dm);
			dm.e -= or_prect.left;
			dm.f -= or_prect.top;
			CRect DrawRect(0, 0, or_prect.Width(), or_prect.Height());
			CComPtr<PXC::IPXC_OCContext> pROpt;
			g_Inst->CreateStdOCCtx(&pROpt);
			pROpt->put_PrintContentFlags(PXC::RenderType_ModePrint);
			pPage->DrawToDevice((PXC::HANDLE_T)pdlg.hDC, DrawRect, &dm, PXC::DDF_AsVector, nullptr, pROpt, nullptr);
			::EndPage(pdlg.hDC);
		}
		::EndDoc(pdlg.hDC);
	}
	DeleteDC(pdlg.hDC);
}
