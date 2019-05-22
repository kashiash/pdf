#include "stdafx.h"
#include "Sample_Stroke.h"
#include "InitializeSDK.h"
#include "resource.h"
#include "MainFrm.h"
#include "MFCSample.h"

#define _USE_MATH_DEFINES
#include <math.h>

CSDKSample_Stroke::CSDKSample_Stroke()
{
}

CSDKSample_Stroke::~CSDKSample_Stroke()
{
}

LPCWSTR CSDKSample_Stroke::GetGroup()
{
	return L"Creation";
}

LPCWSTR CSDKSample_Stroke::GetTitle()
{
	return L"Strokes";
}

LPCWSTR CSDKSample_Stroke::GetDescription()
{
	return L"";
}

//

HRESULT Stroke_Ex1(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = (pr.right + pr.left) / 2.0;
	double y = pr.top - I2P(0.8);

	pCC->SetLineWidth(5);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->NoDash();
	pCC->MoveTo(x - I2P(3.5), y);
	pCC->LineTo(x + I2P(3.5), y);
	pCC->StrokePath(VARIANT_FALSE);
	DrawTitle(pDoc, pCC, x, y - 8, L"SOLID LINE", 15);
	return S_OK;
}

HRESULT Stroke_Ex2(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = (pr.right + pr.left) / 2.0;
	double y = pr.top - I2P(1.5);

	pCC->SetLineWidth(5);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->SetDash(20.0, 10.0, 5.0);
	pCC->MoveTo(x - I2P(3.5), y);
	pCC->LineTo(x + I2P(3.5), y);
	pCC->StrokePath(VARIANT_FALSE);
	pCC->NoDash();
	DrawTitle(pDoc, pCC, x, y - 8, L"DASHED", 15);
	return S_OK;
}

HRESULT Stroke_Ex3(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = (pr.right + pr.left) / 2.0;
	double y = pr.top - I2P(2.2);

	double dashArr[] = {70, 10, 10, 5, 3, 5, 10, 10};
	pCC->SetLineWidth(5);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->SetDashEx(dashArr, _countof(dashArr), 1.0);
	pCC->MoveTo(x - I2P(3.5), y);
	pCC->LineTo(x + I2P(3.5), y);
	pCC->StrokePath(VARIANT_FALSE);
	pCC->NoDash();
	DrawTitle(pDoc, pCC, x, y - 8, L"POLY DASHED", 15);
	return S_OK;
}

HRESULT Stroke_Ex4(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = (pr.right + pr.left) / 2.0;
	double y = pr.top - I2P(3);

	pCC->SetLineWidth(20);
	pCC->SetStrokeColorRGB(clrBlack);

	PXC::PXC_LineCap caps[3] = {PXC::LineCap_Butt, PXC::LineCap_Round, PXC::LineCap_Square};
	LPCWSTR titles[3] = {L"LineCap_Butt", L"LineCap_Round", L"LineCap_Square"};
	for (int i = 0; i < _countof(titles); i++)
	{
		pCC->SetLineCap(caps[i]);
		pCC->MoveTo(x - I2P(1), y);
		pCC->LineTo(x + I2P(1), y);
		pCC->StrokePath(VARIANT_FALSE);
		DrawTitle(pDoc, pCC, x, y - 15, titles[i], 15);
		y -= I2P(0.8);
	}
	return S_OK;
}

HRESULT Stroke_Ex5(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = (pr.right + pr.left) / 2.0 - I2P(2);
	double y = pr.top - I2P(5.9);
	double r = I2P(0.5);

	pCC->SetLineWidth(15);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->SaveState();

	PXC::PXC_LineJoin joins[3] = {PXC::LineJoin_Miter, PXC::LineJoin_Round, PXC::LineJoin_Bevel};
	LPCWSTR titles[3] = {L"LineJoin_Miter", L"LineJoin_Round", L"LineJoin_Bevel"};
	double pts[3 * 2];
	for (int i = 0; i < _countof(titles); i++)
	{
		double a = 30;
		for (int j = 0; j < 3; j++)
		{
			double xx = r * cos(a * M_PI / 180.0);
			double yy = r * sin(a * M_PI / 180.0);
			pts[j * 2 + 0] = x + xx;
			pts[j * 2 + 1] = y - yy;
			a += 120;
		}
		pCC->SetLineJoin(joins[i]);
		pCC->Polygon(pts, 6, VARIANT_TRUE);
		pCC->StrokePath(VARIANT_TRUE);
		DrawTitle(pDoc, pCC, x, y - r, titles[i], 15);
		x += I2P(2);
	}
	pCC->RestoreState();
	return S_OK;
}

HRESULT Stroke_Ex6(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = (pr.right + pr.left) / 2.0 - I2P(2);
	double y = pr.top - I2P(7.7);

	pCC->SetLineWidth(15);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->SaveState();

	pCC->SetLineJoin(PXC::LineJoin_Miter);

	LPCWSTR titles[2] = {L"NO MITER LIMIT", L"MITER LIMIT"};
	for (int i = 0; i < _countof(titles); i++)
	{
		pCC->MoveTo(x - I2P(1), y + I2P(0.5));
		pCC->LineTo(x + I2P(1), y);
		pCC->LineTo(x - I2P(1), y);
		pCC->StrokePath(VARIANT_FALSE);
		DrawTitle(pDoc, pCC, x, y - 14, titles[i], 15);
		x += I2P(4);
		pCC->SetMiterLimit(3.0);
	}
	pCC->RestoreState();
	return S_OK;
}

HRESULT Stroke_Ex7(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = pr.left + I2P(2.5);
	double y = pr.top - I2P(9.4);

	CComPtr<PXC::IColor> color;
	HRESULT hr = g_AUX->CreateColor(PXC::ColorType_Gray, &color);
	if (FAILED(hr))
		return hr;

	int numSteps = 256;
	float minV = 0.0;
	float maxV = 1.0;

	double width = I2P(1.5);
	double height = I2P(1.5);

	double dx = (width / (numSteps - 1)) / 2.0;
	double dy = (height / (numSteps - 1)) / 2.0;

	PXC::PXC_Rect r;
	r.left = x - width / 2.0;
	r.right = r.left + width;
	r.bottom = y - height / 2.0;
	r.top = r.bottom + height;

	pCC->SetLineWidth(1.0);
	pCC->SetLineJoin(PXC::LineJoin_Bevel);

	for (int i = 0; i < numSteps; i++)
	{
		color->SetGray(minV + ((maxV - minV) * (float)i / (float)(numSteps - 1)));
		pCC->SetColor(nullptr, color);
		pCC->Rect(r.left, r.bottom, r.right, r.top);
		pCC->StrokePath(VARIANT_TRUE);
		r.left += dx; r.right -= dx;
		r.bottom += dy; r.top -= dy;
	}
	DrawTitle(pDoc, pCC, x, y - 65, L"GRADIENT FILL EMULATION", 15);
	return S_OK;
}

HRESULT Stroke_Ex8(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = pr.right - I2P(2.5);
	double y = pr.top - I2P(9.4);

	double width = I2P(3.0);
	double height = I2P(1.5);

	PXC::PXC_Rect rect;
	rect.left = x - (width / 2);
	rect.right = rect.left + width;
	rect.top = y + height / 2;
	rect.bottom = rect.top - height;

	pCC->SetLineWidth(15);
	CComPtr<PXC::IPXC_Pattern> pPat;
	CBitmap bmp;
	bmp.LoadBitmap(IDB_MAIN);
	CreateImagePattern(pDoc, bmp, pPat);

	pCC->SetPattern(pPat, VARIANT_FALSE, nullptr);
	pCC->Ellipse(rect.left, rect.bottom, rect.right, rect.top);
	pCC->StrokePath(VARIANT_TRUE);

	DrawTitle(pDoc, pCC, x, y - 65, L"Stroke With Image Pattern", 15);
	return S_OK;
}

HRESULT CSDKSample_Stroke::Perform()
{
	HRESULT hr = S_OK;
	CComPtr<PXC::IPXC_Document> pDoc;
	do
	{
		// creating document with one page
		CComPtr<PXC::IPXC_Page> pPage;
		PXC::PXC_Rect pr = letterSize;
		hr = CreateNewDocWithPage(pDoc, pPage, pr);
		if (FAILED(hr))
			break;
		// creating some conent
		CComPtr<PXC::IPXC_ContentCreator> pCC;
		hr = pDoc->CreateContentCreator(&pCC);
		BreakOnFailure(hr, L"Error creating conent creator");
		//
		Stroke_Ex1(pDoc, pCC, pr);
		Stroke_Ex2(pDoc, pCC, pr);
		Stroke_Ex3(pDoc, pCC, pr);
		Stroke_Ex4(pDoc, pCC, pr);
		Stroke_Ex5(pDoc, pCC, pr);
		Stroke_Ex6(pDoc, pCC, pr);
		Stroke_Ex7(pDoc, pCC, pr);
		Stroke_Ex8(pDoc, pCC, pr);

		// replace page content with newly created one
		CComPtr<PXC::IPXC_Content> pContent;
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		pContent = nullptr;
		pCC = nullptr;
		BreakOnFailure(hr, L"Error replacing page content");
		pPage = nullptr;
		if (m_bSavePDF)
		{
			CStringW sFilename;
			hr = SaveDocument(pDoc, sFilename, m_bOpenPDF);
		}
		if (m_bPreviewPDF)
		{
			static_cast<CMainFrame*>(theApp.GetMainWnd())->ShowPreview(pDoc);
		}
	} while (false);
	if (pDoc && !m_bPreviewPDF)
		pDoc->Close(0);
	return hr;
}
