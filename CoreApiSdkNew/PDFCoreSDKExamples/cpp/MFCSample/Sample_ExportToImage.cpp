#include "stdafx.h"
#include "Sample_ExportToImage.h"
#include "InitializeSDK.h"
#include "MainFrm.h"
#include "MFCSample.h"
#include "resource.h"

//////////////////////////////////////////////////////////////////////////
class CDlg_ExportToImage : public CDialogEx
{
public:
	enum { IDD = IDD_DLG_EXPORT2IMAGE };
	CDlg_ExportToImage(CWnd* pParent);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	// Implementation
	void UpdatePagesControls();
	void UpdageImageFileName();
	void PopulateControls();
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCmdRun();
	afx_msg void OnPDFBrowse();
	afx_msg void OnPageNumChange();
	afx_msg void OnFormatChanged();
public:
	bool							m_bNoUpdate;
	CSDKSample_ExportToImage*		m_pSample;
	DWORD							m_nNumPages;
	bool							m_bCustomImageFileName;
private:
	CComboBox						m_cbFormats;
	CComboBox						m_cbTextMode;
	CComboBox						m_cbBpp;
	CComboBox						m_cbRender;
};

CDlg_ExportToImage::CDlg_ExportToImage(CWnd* pParent) : CDialogEx(IDD, pParent)
{
	m_pSample = nullptr;
	m_nNumPages = 0;
	m_bCustomImageFileName = false;
	m_bNoUpdate = true;
}

BOOL CDlg_ExportToImage::OnInitDialog()
{
	__super::OnInitDialog();

	SendDlgItemMessage(IDC_DPI_SPIN, UDM_SETRANGE32, 50, 1200);
	SendDlgItemMessage(IDC_DPI_SPIN, UDM_SETPOS32, 0, m_pSample->m_nDPI);

	UpdatePagesControls();
	PopulateControls();
	m_bNoUpdate = false;
	return TRUE;
}

void CDlg_ExportToImage::PopulateControls()
{
	// available image formats
	PXC::ULONG_T nNumFormats = 0;
	g_ImgCore->get_FormatsCount(&nNumFormats);
	int nCur = -1;
	for (PXC::ULONG_T i = 0; i < nNumFormats; i++)
	{
		CComPtr<PXC::IIXC_FormatInfo> fmtInfo;
		g_ImgCore->GetFormatInfo(i, &fmtInfo);
		CComBSTR sName;
		PXC::ULONG_T nID;
		PXC::ULONG_T nFlags;
		fmtInfo->get_Flags(&nFlags);
		if (((nFlags & PXC::FMTF_Vector) != 0) || ((nFlags & PXC::FMTF_CAN_Encode) == 0))
			continue;
		fmtInfo->get_Name(&sName);
		fmtInfo->get_ID(&nID);
		//
		int idx = m_cbFormats.AddString((LPCWSTR)sName);
		m_cbFormats.SetItemData(idx, (DWORD_PTR)nID);
		if ((DWORD)nID == m_pSample->m_nImageFormat)
			nCur = idx;
	}
	if (nCur != -1)
		m_cbFormats.SetCurSel(nCur);
	// text modes
	m_cbTextMode.AddString(L"No Smooth");
	m_cbTextMode.AddString(L"Antialias");
	m_cbTextMode.AddString(L"Clear Type");
	m_cbTextMode.SetCurSel(m_pSample->m_nTextSmoothMode);
	// bpp
	m_cbBpp.AddString(L"32 bit");
	m_cbBpp.AddString(L"24 bit");
	m_cbBpp.AddString(L"16 bit");
	m_cbBpp.AddString(L"8 bit");

	//
// 	m_cbRender.AddString(L"1");
// 	m_cbRender.AddString(L"2");
// 	m_cbRender.AddString(L"3");
// 	m_cbRender.AddString(L"4");
}

void CDlg_ExportToImage::DoDataExchange(CDataExchange* pDX)
{
	__super::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_FORMATS, m_cbFormats);
	DDX_Control(pDX, IDC_TEXTMODE, m_cbTextMode);
	DDX_Control(pDX, IDC_BPP, m_cbBpp);
	DDX_Control(pDX, IDC_RFORMAT, m_cbRender);
	
	if (m_pSample)
	{
		DDX_Text(pDX, IDC_FILE_NAME, m_pSample->m_sPDFFileName);
		DDX_Text(pDX, IDC_IMAGE_FILE_NAME, m_pSample->m_sImageFileName);
		DDX_Text(pDX, IDC_DPI, m_pSample->m_nDPI);
		DDV_MinMaxUInt(pDX, m_pSample->m_nDPI, 50, 1200);
	}
}

void CDlg_ExportToImage::OnCmdRun()
{
	UpdateData(TRUE);
	if (m_pSample)
		m_pSample->Perform();
}

void CDlg_ExportToImage::OnPDFBrowse()
{
	if (m_pSample == nullptr)
		return;
	UpdateData(TRUE);
	CFileDialog dlg(TRUE, L"pdf", m_pSample->m_sPDFFileName, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
					L"PDF Document|*.pdf|All Files|*.*||", this);
	if (dlg.DoModal() != IDOK)
		return;
	PXC::ULONG_T numPages = 0;
	CString sFileName = dlg.GetPathName();
	do
	{
		CComPtr<PXC::IPXC_Document> pDoc;
		HRESULT hr = OpenDocFromFile(sFileName, pDoc);
		if (pDoc == nullptr)
			break;
		CComPtr<PXC::IPXC_Pages> pPages;
		pDoc->get_Pages(&pPages);
		pPages->get_Count(&numPages);
	} while (false);
	if (numPages == 0)
	{
		::MessageBeep(-1);
		return;
	}
	m_pSample->m_sPDFFileName = sFileName;
	m_nNumPages = (DWORD)numPages;
	if (m_pSample->m_nPageNumber >= m_nNumPages)
		m_pSample->m_nPageNumber = m_nNumPages - 1;
	UpdateData(FALSE);
	UpdatePagesControls();
	UpdageImageFileName();
}

void CDlg_ExportToImage::UpdatePagesControls()
{
	SendDlgItemMessage(IDC_NUMBER_SPIN, UDM_SETRANGE32, 1, m_nNumPages);
	SendDlgItemMessage(IDC_NUMBER_SPIN, UDM_SETPOS32, 0, m_pSample->m_nPageNumber + 1);
	CString fmt;
	fmt.Format(L"of %d page(s)", m_nNumPages);
	SetDlgItemText(IDC_NUM_PAGES_LABEL, fmt);
}

static
LONG ImgFormatID2FormatIdx(DWORD nFmtID, CComPtr<PXC::IIXC_FormatInfo>& info)
{
	LONG nIdx = 0;
	PXC::ULONG_T nNumFormats = 0;
	g_ImgCore->get_FormatsCount(&nNumFormats);
	info = nullptr;
	for (PXC::ULONG_T i = 0; i < nNumFormats; i++)
	{
		CComPtr<PXC::IIXC_FormatInfo> fmtInfo;
		g_ImgCore->GetFormatInfo(i, &fmtInfo);
		if (fmtInfo == nullptr)
			continue;
		PXC::ULONG_T nID;
		fmtInfo->get_ID(&nID);
		if ((DWORD)nID == nFmtID)
		{
			info = fmtInfo;
			return (LONG)i;
		}
	}
	return -1;
}

void CDlg_ExportToImage::UpdageImageFileName()
{
	if (m_bCustomImageFileName || !m_pSample)
		return;
	CString t;
	CString s = m_pSample->m_sPDFFileName;
	if (s.GetLength() == 0)
		return;

	int nPos = s.ReverseFind('.');
	if (nPos >= 0)
		s = s.Left(nPos);
	t.Format(L"_%d", m_pSample->m_nPageNumber + 1);
	s.Append(t);
	CComPtr<PXC::IIXC_FormatInfo> info;
	ImgFormatID2FormatIdx(m_pSample->m_nImageFormat, info);
	if (info != nullptr)
	{
		CComBSTR sExt;
		info->get_Extentions(&sExt);
		t = (LPCWSTR)sExt;
		nPos = t.Find('.');
		if (nPos >= 0)
			t = t.Mid(nPos + 1);
		nPos = t.Find(';');
		if (nPos >= 0)
			t = t.Left(nPos);
		s.AppendChar('.');
		s.Append(t);
	}
	m_pSample->m_sImageFileName = s;
	UpdateData(FALSE);
}

void CDlg_ExportToImage::OnPageNumChange()
{
	if (m_bNoUpdate || (m_pSample == nullptr) || (m_nNumPages == 0))
		return;
	LONG nPN = (LONG)SendDlgItemMessage(IDC_NUMBER_SPIN, UDM_GETPOS32);
	if (nPN < 1)
		nPN = 1;
	if ((DWORD)nPN > m_nNumPages)
		nPN = m_nNumPages;
	m_pSample->m_nPageNumber = nPN - 1;
	UpdageImageFileName();
}

void CDlg_ExportToImage::OnFormatChanged()
{
	if (m_bNoUpdate || !m_pSample)
		return;
	int idx = m_cbFormats.GetCurSel();
	if (idx < 0)
		return;
	DWORD nFmtID = (DWORD)m_cbFormats.GetItemData(idx);
	m_pSample->m_nImageFormat = nFmtID;
	UpdageImageFileName();
}

BEGIN_MESSAGE_MAP(CDlg_ExportToImage, CDialogEx)
	ON_BN_CLICKED(IDC_CMD_RUN, &OnCmdRun)
	ON_BN_CLICKED(IDC_BROWSE, &OnPDFBrowse)
	ON_EN_CHANGE(IDC_NUMBER, &OnPageNumChange)
	ON_CBN_SELCHANGE(IDC_FORMATS, &OnFormatChanged)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////
CSDKSample_ExportToImage::CSDKSample_ExportToImage()
{
	m_nPageNumber		= 0;
	m_nImageFormat		= PXC::FMT_PNG_ID;
	m_nDPI				= 300;
	m_nPageFormat		= PXC::PageFormat_8ARGB;
}

CSDKSample_ExportToImage::~CSDKSample_ExportToImage()
{
}

LPCWSTR CSDKSample_ExportToImage::GetGroup()
{
	return L"Rendering";
}

LPCWSTR CSDKSample_ExportToImage::GetTitle()
{
	return L"Export to Image";
}

LPCWSTR CSDKSample_ExportToImage::GetDescription()
{
	return L"Export specified page of PDF document to image of selected format.";
}

CWnd* CSDKSample_ExportToImage::GetDialog(CWnd* pParent)
{
	if (m_pWindow == nullptr)
	{
		CDlg_ExportToImage* pDlg = new CDlg_ExportToImage(pParent);
		pDlg->m_pSample = this;
		pDlg->Create(CDlg_ExportToImage::IDD, pParent);
		m_pWindow = pDlg;
	}
	return m_pWindow;
}

HRESULT CSDKSample_ExportToImage::Perform()
{
	HRESULT hr = S_OK;
	CComPtr<PXC::IPXC_Document> pDoc;
	do
	{
		hr = OpenDocFromFile(m_sPDFFileName, pDoc);
		if (pDoc == nullptr)
			break;

		CComPtr<PXC::IPXC_Pages> pages;
		pDoc->get_Pages(&pages);
		CComPtr<PXC::IPXC_Page> page;
		pages->get_Item(m_nPageNumber, &page);
		
		double fWidth = 0.0, fHeidth = 0.0;
		page->GetDimension(&fWidth, &fHeidth);


		const ULONG cx = (ULONG)(fWidth * m_nDPI / 72.0);
		const ULONG cy = (ULONG)(fHeidth * m_nDPI / 72.0);

		CComPtr<PXC::IIXC_Page> iPage;
		hr = g_ImgCore->Page_CreateEmpty(cx, cy, m_nPageFormat, 0, &iPage);
		if (FAILED(hr))
			break;

		CRect rc(0, 0, cx, cy);
		//PXC::PXC_Matrix mt;
		//page->DrawToIXCPage(iPage, rc, &mt, param, nullptr, nullptr); 

		CComPtr<PXC::IPXC_PageRenderParams> param;
		g_Inst->CreateRenderParams(&param);
		if (param != nullptr)
		{
			param->put_RenderFlags(PXC::RF_OverrideBackgroundColor | PXC::RF_SmoothImages | PXC::RF_SmoothLineArts);

			param->put_TextSmoothMode(m_nTextSmoothMode);
		}

		hr = page->DrawToIXCPage(iPage, rc, nullptr, param, nullptr, nullptr);
		if (FAILED(hr))
			break;

		iPage->put_FmtInt(PXC::FP_ID_XDPI, m_nDPI);
		iPage->put_FmtInt(PXC::FP_ID_YDPI, m_nDPI);
		iPage->put_FmtInt(PXC::FP_ID_FILTER, 0);
		iPage->put_FmtInt(PXC::FP_ID_FORMAT, m_nImageFormat);
		iPage->put_FmtInt(PXC::FP_ID_ITYPE, 16);
		iPage->put_FmtInt(PXC::FP_ID_COMP_LEVEL, 2 /*pCommonCmdLine->complevel*/);
		//iPage->put_FmtInt(PXC::FP_ID_IMG_SUBTYPE, PXC::ImageFormat_Auto);
		iPage->put_FmtInt(PXC::FP_ID_COMP_TYPE, 32773);

		CComPtr<PXC::IIXC_Image> img;
		hr = g_ImgCore->CreateEmptyImage(&img);
		if (FAILED(hr))
			break;
		hr = img->InsertPage(iPage, 0);
		if (FAILED(hr))
			break;
		hr = img->Save(m_sImageFileName.GetBuffer(), PXC::CreationDisposition_Overwrite);
		if (FAILED(hr))
			break;

		// open image file
		ShellExecute(m_pWindow->m_hWnd, L"open", m_sImageFileName, nullptr, nullptr, 0);
	} while (false);
	return hr;
}
