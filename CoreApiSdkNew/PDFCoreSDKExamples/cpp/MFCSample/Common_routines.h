#pragma once

extern const PXC::PXC_Rect	letterSize;

#define I2P(x)			((x) * 72.0)

#define clrBlack		RGB(0, 0, 0)
#define clrWhite		RGB(255, 255, 255)
#define clrGold			RGB(255, 215, 0)
#define clrKhaki		RGB(189, 183, 107)
#define clrLtKhaki		RGB(240, 230, 140)
#define clrGray			RGB(128, 128, 128)
#define clrLtGray		RGB(232, 232, 232)

HRESULT CreateNewDocWithPage(CComPtr<PXC::IPXC_Document>& pDoc, CComPtr<PXC::IPXC_Page>& pPage, const PXC::PXC_Rect& pageRect);
HRESULT AddNewPage(PXC::IPXC_Document* pDoc, CComPtr<PXC::IPXC_Page>& pPage, const PXC::PXC_Rect& pageRect);
HRESULT SaveDocument(PXC::IPXC_Document* pDoc, CStringW& sFilename, BOOL bOpen = TRUE);
HRESULT DrawTitle(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, double cx, double baseLineY, LPCWSTR sText, double fontSize);
HRESULT CreateImagePattern(PXC::IPXC_Document* pDoc, CBitmap& bmp, CComPtr<PXC::IPXC_Pattern>& pPattern);
HRESULT OpenDocFromFile(LPCWSTR sFileName, CComPtr<PXC::IPXC_Document>& pDoc);
