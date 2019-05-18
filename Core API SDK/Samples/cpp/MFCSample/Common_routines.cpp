#include "stdafx.h"
#include "Common_routines.h"
#include "InitializeSDK.h"
#include "AuthCallback.h"

const PXC::PXC_Rect	letterSize = {0.0, 0.0, 8.5 * 72.0, 11.0 * 72.0};

LPCWSTR GetErrorDesc(HRESULT hres, CString& str)
{
	str.Empty();
	if (g_AUX)
	{
		CComBSTR bstr;
		g_AUX->FormatHRESULT(hres, 0, &bstr);
		str = (LPCWSTR)bstr;
	}
	else
	{
		DWORD dwFlags = FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER;
		LPWSTR pBuffer = nullptr;
		DWORD nRes = FormatMessageW(dwFlags, nullptr, hres, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
									pBuffer, 0, nullptr);
		if (pBuffer != nullptr)
		{
			str = pBuffer;
			LocalFree(pBuffer);
		}
	}
	return (LPCWSTR)str;
}

HRESULT AddNewPage(PXC::IPXC_Document* pDoc, CComPtr<PXC::IPXC_Page>& pPage, const PXC::PXC_Rect& pageRect)
{
	HRESULT hr = S_OK;
	do
	{
		pPage = nullptr;
		//
		CComPtr<PXC::IPXC_Pages> pPages;
		pDoc->get_Pages(&pPages);
		// adding page to the document
		PXC::PXC_Rect pr = pageRect;
		hr = pPages->InsertPage((ULONG)-1, &pr, nullptr, &pPage);
		BreakOnFailure(hr, L"Error adding page to the document");
	} while (false);
	return hr;
}

HRESULT CreateNewDocWithPage(CComPtr<PXC::IPXC_Document>& pDoc, CComPtr<PXC::IPXC_Page>& pPage, const PXC::PXC_Rect& pageRect)
{
	HRESULT hr = S_OK;
	pPage = nullptr;
	do 
	{
		// new document creation
		hr = g_Inst->NewDocument(&pDoc);
		BreakOnFailure(hr, L"Error creating new document");
		CComPtr<PXC::IPXC_DocumentProps> pProps;
		pDoc->get_Props(&pProps);
		pProps->put_SpecVersion(0x10007);
		hr = AddNewPage(pDoc, pPage, pageRect);
	} while (false);
	return hr;
}

HRESULT DrawTitle(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, double cx, double baseLineY, LPCWSTR sText, double fontSize)
{
	HRESULT hr = S_OK;
	do
	{
		CComPtr<PXC::IPXC_Font> pFont;
		hr = pDoc->CreateNewFont(L"Arial", 0, FW_NORMAL, &pFont);
		if (FAILED(hr))
			break;
		pCC->SaveState();
		pCC->SetFillColorRGB(clrBlack);
		pCC->SetFont(pFont);
		double twidth = 0;
		double theight = 0;
		pCC->CalcTextSize(fontSize, (LPWSTR)sText, &twidth, &theight, -1);
		pCC->SetFontSize(fontSize);
		pCC->ShowTextLine(cx - twidth / 2.0, baseLineY, (LPWSTR)sText, -1, PXC::STLF_Top | PXC::STLF_AllowSubstitution);
		pCC->RestoreState();
	} while (false);
	return hr;
}

HRESULT SaveDocument(PXC::IPXC_Document* pDoc, CStringW& sFilename, BOOL bOpen)
{
	HRESULT hr = S_OK;
	do 
	{
		if (sFilename.IsEmpty())
		{
			CFileDialog dlg(FALSE, L"pdf", L"sample.pdf", OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
							L"PDF Document|*.pdf||", AfxGetMainWnd());

			if (dlg.DoModal() != IDOK)
			{
				LOG(hr, L"User break");
				break;
			}
			sFilename = dlg.GetPathName();
		}
		hr = pDoc->WriteToFile((LPWSTR)(LPCWSTR)sFilename, nullptr, 0);
		BreakOnFailure(hr, L"Error saving document: %.8lx", hr);
		LOG(hr, L"Document has been saved to '%s'", (LPCWSTR)sFilename);
		if (bOpen)
			ShellExecute(NULL, L"open", sFilename, nullptr, nullptr, SW_NORMAL);
	} while (false);
	return hr;
}

HRESULT CreateImagePattern(PXC::IPXC_Document* pDoc, CBitmap& bmp, CComPtr<PXC::IPXC_Pattern>& pPattern)
{
	pPattern = nullptr;
	HRESULT hr = S_OK;
	do 
	{
		PXC::PXC_Rect bbox;
		BITMAP bm = {0};
		bmp.GetBitmap(&bm);
		bbox.left	= 0;
		bbox.bottom	= 0;
		bbox.right	= (double)bm.bmWidth * 72.0 / 96.0;
		bbox.top	= (double)bm.bmHeight * 72.0 / 96.0;
		CComPtr<PXC::IIXC_Page> page;
		hr = g_ImgCore->Page_CreateFromHBITMAP((PXC::HANDLE_T)(HBITMAP)bmp, 0, &page);
		if (FAILED(hr))
			break;
		CComPtr<PXC::IPXC_Image> pImg;
		hr = pDoc->AddImageFromIXCPage(page, 0, &pImg);
		if (FAILED(hr))
			break;
		CComPtr<PXC::IPXC_ContentCreator> pCC;
		hr = pDoc->CreateContentCreator(&pCC);
		if (FAILED(hr))
			break;
		PXC::PXC_Matrix im = {0};
		im.a = bbox.right;
		im.d = bbox.top;
		pCC->SaveState();
		pCC->ConcatCS(&im);
		pCC->PlaceImage(pImg);
		pCC->RestoreState();
		hr = pDoc->CreateTilePattern(&bbox, &pPattern);
		if (FAILED(hr))
			break;
		CComPtr<PXC::IPXC_Content> pC;
		pCC->Detach(&pC);
		pC->put_BBox(&bbox);
		pPattern->SetContent(pC, PXC::PlaceContent_Replace);
	} while (false);
	if (FAILED(hr))
		pPattern = nullptr;
	return hr;
}

HRESULT OpenDocFromFile(LPCWSTR sFileName, CComPtr<PXC::IPXC_Document>& pDoc)
{
	HRESULT hr = S_OK;
	pDoc = nullptr;
	do 
	{
		BreakOnNull(g_Inst, hr, E_FAIL, L"SDK not initialized");
		CDocAuthCallback authCallback;
		PXC::IPXC_DocAuthCallback* pAuthCallback = (PXC::IPXC_DocAuthCallback*)authCallback.GetInterface(&__uuidof(PXC::IPXC_DocAuthCallback));
		hr = g_Inst->OpenDocumentFromFile((LPWSTR)sFileName, pAuthCallback, nullptr, 0, 0, &pDoc);
		if (FAILED(hr))
		{
			CString str;
			LOG(hr, L"Error opening document '<i>%s</i>': <r><b>0x%.8lx</b> - %s</r>", sFileName, hr, GetErrorDesc(hr, str));
		}
		else
		{
			LOG(hr, L"Document '<i>%s</i>' opened successfully.", sFileName);
		}
	} while (false);
	return hr;
}
