#include "stdafx.h"
#include "Sample_Fill.h"
#include "InitializeSDK.h"
#include "MainFrm.h"
#include "MFCSample.h"
#include "resource.h"

#define _USE_MATH_DEFINES
#include <math.h>


CSDKSample_Fill::CSDKSample_Fill()
{
}

CSDKSample_Fill::~CSDKSample_Fill()
{
}

LPCWSTR CSDKSample_Fill::GetGroup()
{
	return L"Creation";
}

LPCWSTR CSDKSample_Fill::GetTitle()
{
	return L"Filling";
}

LPCWSTR CSDKSample_Fill::GetDescription()
{
	return L"";
}

HRESULT Fill_AddStarPath(PXC::IPXC_ContentCreator* pCC, double x, double y, double r)
{
	static const int num = 5;
	double points[num * 2];

	double a = -90;
	for (int i = 0; i < num; i++)
	{
		points[i * 2 + 0] = x + r * cos(a * M_PI / 180.0);
		points[i * 2 + 1] = y - r * sin(a * M_PI / 180.0);
		a += 2.0 * (360.0 / num);
	}
	return pCC->Polygon(points, num * 2, VARIANT_TRUE);
}

HRESULT Fill_Ex1(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	double x = I2P(2.0);
	double y = pr.top - I2P(2.0);
	double r = I2P(1.0);
	double rr;

	static LPCWSTR titles[] = {L"NONZERO WINDING NUMBER RULE", L"EVEN-ODD RULE"};
	static const PXC::PXC_FillRule rules[] = {PXC::FillRule_Winding, PXC::FillRule_EvenOdd};

	for (int i = 0; i < 2; i++)
	{
		x = I2P(2.0);
		PXC::PXC_FillRule rule = rules[i];
		DrawTitle(pDoc, pCC, (pr.right + pr.left) / 2, y - r - 15, titles[i], 15);
		//
		Fill_AddStarPath(pCC, x, y, r);
		pCC->SetStrokeColorRGB(clrBlack);
		pCC->SetFillColorRGB(clrKhaki);
		pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, rule);

		x = (pr.right + pr.left) / 2;
		rr = r;
		pCC->Arc(&PXC::PXC_Rect({x - rr, y - rr, x + rr, y + rr}), 0.0, 360.0, VARIANT_TRUE);
		rr = r / 2;
		pCC->Arc(&PXC::PXC_Rect({x - rr, y - rr, x + rr, y + rr}), 0.0, 360.0, VARIANT_TRUE);
		pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, rule);

		x = pr.right - I2P(2);
		rr = r;
		pCC->Arc(&PXC::PXC_Rect({x - rr, y - rr, x + rr, y + rr}), 0.0, 360.0, VARIANT_TRUE);
		rr = r / 2;
		pCC->Arc(&PXC::PXC_Rect({x - rr, y - rr, x + rr, y + rr}), 360.0, 0.0, VARIANT_TRUE);
		pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, rule);
		//
		y -= I2P(3);
	}

	return S_OK;
}

HRESULT Fill_Ex2(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	HRESULT hr = S_OK;

	double w = (pr.right - pr.left - I2P(3)) / 2.0;
	double h = I2P(1);
	double y = pr.top - I2P(1.0) - h;
	double dy = I2P(2);
	double x[2] = {pr.left + I2P(1.0), pr.left + I2P(1.0 + 4.0)};

	pCC->SetLineWidth(1.0);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->SetFillColorRGB(clrLtGray);

	pCC->Rect(x[0], y, x[0] + w, y + h);
	pCC->StrokePath(VARIANT_FALSE);
	DrawTitle(pDoc, pCC, x[0] + w / 2, y - I2P(0.1), L"STROKE, NO FILL", 15);

	pCC->Rect(x[1], y, x[1] + w, y + h);
	pCC->FillPath(VARIANT_FALSE, VARIANT_FALSE, PXC::FillRule_Winding);
	DrawTitle(pDoc, pCC, x[1] + w / 2, y - I2P(0.1), L"FILL, NO STROKE", 15);

	y -= dy;

	pCC->Rect(x[0], y, x[0] + w, y + h);
	pCC->FillPath(VARIANT_FALSE, VARIANT_TRUE, PXC::FillRule_Winding);
	DrawTitle(pDoc, pCC, x[0] + w / 2, y - I2P(0.1), L"STROKE & FILL", 15);

	static LPCWSTR titles[] =
	{
		L"PATTER FILL: CrossHatch",
		L"PATTER FILL: CrossDiagonal",
		L"PATTER FILL: DiagonalLeft",
		L"PATTER FILL: DiagonalRight",
		L"PATTER FILL: Horizontal",
		L"PATTER FILL: Vertical"
	};

	int k = 1;
	CComPtr<PXC::IPXC_Pattern> pPat;
	for (int i = PXC::StdPattern_CrossHatch; i <= PXC::StdPattern_Vertical; i++)
	{
		pDoc->GetStdTilePattern((PXC::PXC_StdPatternType)i, &pPat);
		pCC->SetPatternRGB(pPat, VARIANT_TRUE, clrKhaki);
		pPat = nullptr;
		pCC->Rect(x[k], y, x[k] + w, y + h);
		pCC->FillPath(VARIANT_FALSE, VARIANT_TRUE, PXC::FillRule_Winding);
		DrawTitle(pDoc, pCC, x[k] + w / 2, y - I2P(0.1), titles[i - PXC::StdPattern_CrossHatch], 15);
		k ^= 1;
		if (k == 0)
			y -= dy;
	}

	CBitmap bmp;
	bmp.LoadBitmap(IDB_MAIN);
	hr = CreateImagePattern(pDoc, bmp, pPat);
	pCC->SetPatternRGB(pPat, VARIANT_TRUE, clrKhaki);
	pCC->Rect(x[k], y, x[k] + w, y + h);
	pCC->FillPath(VARIANT_FALSE, VARIANT_TRUE, PXC::FillRule_Winding);
	DrawTitle(pDoc, pCC, x[k] + w / 2, y - I2P(0.1), L"PATTERN FILL: Image", 15);

	return S_OK;
}

HRESULT Fill_Ex3(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	HRESULT hr = S_OK;

	double w = (pr.right - pr.left - I2P(3)) / 2.0;
	double h = I2P(1);
	double y = pr.top - I2P(1.0) - h;
	double dy = I2P(2);
	double x = pr.left + I2P(1.0);

	CComPtr<PXC::IPXC_GradientStops> pStops;
	pDoc->CreateShadeStops(&pStops);

	pStops->AddStopRGB(0.0, RGB(75, 181, 253));
	pStops->AddStopRGB(0.5, RGB(102, 185, 255));
	pStops->AddStopRGB(0.5, RGB(255, 217, 0));
	pStops->AddStopRGB(1.0, RGB(255, 245, 0));

	CComPtr<PXC::IPXC_Shading> pShade;
	PXC::PXC_Point p0, p1;
	p0.x = x; p0.y = y + h;
	p1.x = x; p1.y = y;

	pDoc->CreateLinearShade(&p0, &p1, pStops, 3, &pShade);

	pCC->SaveState();
	pCC->Rect(x, y, x + w, y + h);
	pCC->ClipPath(PXC::FillRule_Winding, VARIANT_TRUE);
	pCC->Shade(pShade);
	pCC->RestoreState();
	DrawTitle(pDoc, pCC, x + w / 2, y - I2P(0.1), L"Linear Gradient", 15);

	x = pr.left + I2P(5) + w / 2;
	double r = h / 2;
	pShade = nullptr;
	pStops->Reset();
	p0.x = x; p0.y = y + r;
	p1.x = p0.x - 0.5 * r; p1.y = p0.y + 0.5 * r;
	pStops->AddStopRGB(0.0, RGB(92, 92, 92));
	pStops->AddStopRGB(1.0, RGB(242, 242, 242));
	pDoc->CreateRadialShade(&p0, &p1, h / 2, 0.0, pStops, 0, &pShade);
	pCC->Shade(pShade);
	DrawTitle(pDoc, pCC, x, y - I2P(0.1), L"Radial Gradient", 15);

	return S_OK;
}

HRESULT CSDKSample_Fill::Perform()
{
	HRESULT hr = S_OK;
	CComPtr<PXC::IPXC_Document> pDoc;
	do 
	{
		// creating document with one page
		CComPtr<PXC::IPXC_Page> pPage;
		CComPtr<PXC::IPXC_Content> pContent;
		CComPtr<PXC::IPXC_ContentCreator> pCC;

		PXC::PXC_Rect pr = letterSize;
		hr = CreateNewDocWithPage(pDoc, pPage, pr);
		if (FAILED(hr))
			break;
		// creating some conent
		hr = pDoc->CreateContentCreator(&pCC);
		BreakOnFailure(hr, L"Error creating conent creator");
		// page 1
		Fill_Ex1(pDoc, pCC, pr);
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		BreakOnFailure(hr, L"Error replacing page content");
		pContent = nullptr;
		// page 2
		hr = AddNewPage(pDoc, pPage, pr);
		if (FAILED(hr))
			break;
		Fill_Ex2(pDoc, pCC, pr);
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		BreakOnFailure(hr, L"Error replacing page content");
		pContent = nullptr;
		// page 3
		hr = AddNewPage(pDoc, pPage, pr);
		if (FAILED(hr))
			break;
		Fill_Ex3(pDoc, pCC, pr);
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		BreakOnFailure(hr, L"Error replacing page content");
		pContent = nullptr;

		pCC = nullptr;
		BreakOnFailure(hr, L"Error replacing page content");
		pPage = nullptr;
		//
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
