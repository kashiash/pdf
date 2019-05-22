#include "stdafx.h"
#include "Sample_Primitives.h"
#include "InitializeSDK.h"
#include "MainFrm.h"
#include "MFCSample.h"
#include "resource.h"

#define _USE_MATH_DEFINES
#include <math.h>

CSDKSample_Primitives::CSDKSample_Primitives()
{
}

CSDKSample_Primitives::~CSDKSample_Primitives()
{
}

LPCWSTR CSDKSample_Primitives::GetGroup()
{
	return L"Creation";
}

LPCWSTR CSDKSample_Primitives::GetTitle()
{
	return L"Primitives";
}

LPCWSTR CSDKSample_Primitives::GetDescription()
{
	return L"";
}

static
HRESULT Ex1(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	HRESULT hr = S_OK;

	PXC::PXC_Point center;
	double r0 = I2P(1.5);
	double r = r0;
	double dr = 6.0;
	LONG i;
	center.x = I2P(2.3);
	center.y = pr.top - I2P(2.5);

	DrawTitle(pDoc, pCC, center.x, center.y - r - 18.0, L"FILLED, STROKE", 15);
	pCC->SetLineWidth(0.5);
	pCC->SetStrokeColorRGB(clrGold);

	for (i = 360; i > 0; i -= 30)
	{
		pCC->MoveTo(center.x, center.y);
		pCC->CircleArc(center.x, center.y, r, 0.0, (double)i, VARIANT_FALSE);
		DWORD c = MulDiv(i, 240, 360);
		pCC->SetFillColorRGB(RGB(c, c, 255));
		pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);
		r -= dr;
	}
	//
	r = r0;
	center.x += I2P(4);
	DrawTitle(pDoc, pCC, center.x, center.y - r - 18.0, L"FILLED, NO STROKE", 15);
	for (i = 360; i > 0; i -= 30)
	{
		pCC->MoveTo(center.x, center.y);
		pCC->CircleArc(center.x, center.y, r, 0.0, (double)i, VARIANT_FALSE);
		DWORD c = MulDiv(i, 240, 360);
		pCC->SetFillColorRGB(RGB(c, c, 255));
		pCC->FillPath(VARIANT_TRUE, VARIANT_FALSE, PXC::FillRule_Winding);
		r -= dr;
	}
	//
	r = r0;
	center.y -= I2P(4.5);
	center.x -= I2P(4);
	DrawTitle(pDoc, pCC, center.x, center.y - r - 18.0, L"STROKE, NOT CLOSED PATH", 15);
	pCC->SetStrokeColorRGB(clrBlack);
	for (i = 360; i > 0; i -= 30)
	{
		pCC->CircleArc(center.x, center.y, r, 0, i, VARIANT_FALSE);
		pCC->StrokePath(VARIANT_FALSE);
		r -= dr;
	}
	//
	r = r0;
	center.x += I2P(4);
	DrawTitle(pDoc, pCC, center.x, center.y - r - 18.0, L"STROKE, CLOSED PATH", 15);
	for (i = 360; i > 0; i -= 30)
	{
		pCC->MoveTo(center.x, center.y);
		pCC->CircleArc(center.x, center.y, r, 0, i, VARIANT_FALSE);
		pCC->StrokePath(VARIANT_TRUE);
		r -= dr;
	}
	return hr;
}

static
HRESULT Ex2(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	HRESULT hr = S_OK;

	double rx	= I2P(1.5);
	double ry	= I2P(1);
	double X	= I2P(2.3);
	double Y	= pr.top - I2P(2.5);

	pCC->SetLineWidth(0.5);
	pCC->SetStrokeColorRGB(clrBlack);
	CComPtr<PXC::IColor> clr;
	g_AUX->CreateColor(PXC::ColorType_Gray, &clr);
	clr->SetGray(0.94f);
	pCC->SetColor(clr, nullptr);

	PXC::PXC_Rect rect = {X - rx, Y - ry, X + ry, Y + ry};

	pCC->Ellipse(rect.left, rect.bottom, rect.right, rect.top);
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);
	DrawTitle(pDoc, pCC, (rect.left + rect.right) / 2.0, rect.bottom - I2P(0.2), L"ELLIPSE", 15);
	//
	rect.left += I2P(4); rect.right += I2P(4);
	DrawTitle(pDoc, pCC, (rect.left + rect.right) / 2.0, rect.bottom - I2P(0.2), L"ARC", 15);
	pCC->Arc(&rect, 0, 270.0, VARIANT_TRUE);
	pCC->StrokePath(VARIANT_FALSE);
	//
	rect.left -= I2P(4); rect.right -= I2P(4);
	rect.top -= I2P(3); rect.bottom -= I2P(3);
	DrawTitle(pDoc, pCC, (rect.left + rect.right) / 2.0, rect.bottom - I2P(0.2), L"CLOSED ARC", 15);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->Arc(&rect, 0, 270.0, VARIANT_TRUE);
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);
	//
	rect.left += I2P(4); rect.right += I2P(4);
	DrawTitle(pDoc, pCC, (rect.left + rect.right) / 2.0, rect.bottom - I2P(0.2), L"PIE", 15);
	pCC->Pie(&rect, 0, 270.0);
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);
	//
	rect.left -= I2P(4); rect.right -= I2P(4);
	rect.top -= I2P(3); rect.bottom -= I2P(3);
	DrawTitle(pDoc, pCC, (rect.left + rect.right) / 2.0, rect.bottom - I2P(0.2), L"CHORD", 15);
	pCC->SetStrokeColorRGB(RGB(200, 200, 200));
	pCC->SetDash(3, 3, 0);
	pCC->Arc(&rect, 0, 270.0, VARIANT_TRUE);
	pCC->StrokePath(VARIANT_FALSE);
	pCC->NoDash();
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->Chord(&rect, 0, 270.0);
	pCC->StrokePath(VARIANT_FALSE);
	//
	rect.left += I2P(4); rect.right += I2P(4);
	PXC::PXC_Point pnt;
	pnt.x = (rect.left + rect.right) / 2;
	pnt.y = (rect.top + rect.bottom) / 2;
	DrawTitle(pDoc, pCC, (rect.left + rect.right) / 2.0, rect.bottom - I2P(0.2), L"CIRCLE", 15);
	pCC->Circle(pnt.x, pnt.y, ry);
	pCC->FillPath(VARIANT_FALSE, VARIANT_TRUE, PXC::FillRule_Winding);

	return hr;
}

static
HRESULT Ex3(PXC::IPXC_Document* pDoc, PXC::IPXC_ContentCreator* pCC, const PXC::PXC_Rect& pr)
{
	HRESULT hr = S_OK;

	// FAST-BEZIER CIRCLE
	double x = I2P(2.2);
	double y = pr.top - I2P(2.5);
	double r = I2P(1.2);

	double b = 1.3333333;
	double br = b * r;
	double p[6 * 2];
	p[0] = p[8]  = p[10] = x - r;
	p[1] = p[3]  = y + br;
	p[2] = p[4]  = p[6] = x + r;
	p[5] = p[11] = y;
	p[7] = p[9]  = y - br;

	DrawTitle(pDoc, pCC, x, y - r - I2P(.1), L"FAST-BEZIER \"CIRCLE\"", 15);

	pCC->SetLineWidth(1.0);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->MoveTo(x - r, y);
	pCC->PolyCurve(p, _countof(p), VARIANT_FALSE);
	pCC->StrokePath(VARIANT_FALSE);

	// CHINA MONAD

	x = I2P(6.2);
	y = pr.top - I2P(2.5);
	double rr = r / 2.0;
	double a = I2P(0.05);

	PXC::PXC_Point center = {x, y};
	PXC::PXC_Point p1 = center;
	PXC::PXC_Point p2 = center;

	p1.y -= rr;
	p2.y += rr;

	DrawTitle(pDoc, pCC, x, y - r - I2P(.1), L"CHINA MONAD", 15);
	//
	pCC->SetLineWidth(1.0);
	pCC->SetColorRGB(clrBlack);

	pCC->CircleArc(center.x, center.y, r, 90.0, -90.0, VARIANT_TRUE);
	pCC->CircleArc(p1.x, p1.y, rr, 270.0, 90.0, VARIANT_TRUE);
	pCC->CircleArc(p2.x, p2.y, rr, -90.0, 90.0, VARIANT_TRUE);
	pCC->Circle(p1.x, p1.y, I2P(0.1));
	pCC->Circle(p2.x, p2.y, I2P(0.1));
	pCC->FillPath(VARIANT_FALSE, VARIANT_TRUE, PXC::FillRule_EvenOdd);
	pCC->CircleArc(center.x, center.y, r, 90.0, 270.0, VARIANT_TRUE);
	pCC->StrokePath(VARIANT_FALSE);

	// POLYGON
	const int ncnt = 8;
	x = I2P(2.2);
	y = pr.top - I2P(6);
	r = I2P(1.3);
	double xy[ncnt * 2];

	DrawTitle(pDoc, pCC, x, y - r - I2P(.1), L"POLYGON", 15);

	a = -90;
	for (int i = 0; i < ncnt; i++)
	{
		xy[i * 2 + 0] = x + r * cos(a * M_PI / 180.0);
		xy[i * 2 + 1] = y - r * sin(a * M_PI / 180.0);
		a += 360.0 / ncnt;
	}
	pCC->Polygon(xy, ncnt * 2, VARIANT_TRUE);
	pCC->SetFillColorRGB(clrKhaki);
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);

	// LISSAJOUS FIGURE
	double xorig = I2P(6.2);
	double yorig = pr.top - I2P(6);
	r = I2P(1.2);
	
	DrawTitle(pDoc, pCC, xorig, yorig - r - I2P(.1), L"LISSAJOUS FIGURE", 15);
	//
	pCC->SetFillColorRGB(clrKhaki);
	pCC->SetStrokeColorRGB(clrBlack);
	for (int i = 0; i < 200; i++)
	{
		double ang = M_PI * i / 100.0;
		x = xorig + r * cos(3 * ang);
		y = yorig - r * sin(5 * ang);
		if (i > 0)
			pCC->LineTo(x, y);
		else
			pCC->MoveTo(x, y);
	}
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);

	// RECTANGLE

	x = I2P(2.2);
	y = pr.top - I2P(9);
	double w = I2P(2.5);
	double h = I2P(1.2);
	
	DrawTitle(pDoc, pCC, x, y - h / 2 - I2P(.1), L"RECTANGLE", 15);
	//
	pCC->SetFillColorRGB(clrKhaki);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->Rect(x - w / 2, y - h/2, x + w/2, y + h/2);
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);

	// ROUND RECTANGLE
	
	x = I2P(6.2);
	y = pr.top - I2P(9);
	w = I2P(2.5);
	h = I2P(1.2);
	double ew = w / 5.0;
	double eh = h / 5.0;
	
	PXC::PXC_Rect rect = {x - w / 2, y - h / 2, x + w / 2, y + h / 2};

	DrawTitle(pDoc, pCC, x, y - h / 2 - I2P(.1), L"ROUND RECTANGLE", 15);
	//
	pCC->SetFillColorRGB(clrKhaki);
	pCC->SetStrokeColorRGB(clrBlack);
	pCC->RoundRect(rect.left, rect.bottom, rect.right, rect.top, ew, eh);
	pCC->FillPath(VARIANT_TRUE, VARIANT_TRUE, PXC::FillRule_Winding);
	pCC->SetStrokeColorRGB(clrWhite);
	pCC->SetLineWidth(0.0);
	pCC->SetDash(1, 1, 0);
	rect.right = rect.left + ew;
	rect.bottom = rect.top - eh;
	pCC->Ellipse(rect.left, rect.bottom, rect.right, rect.top);
	pCC->StrokePath(VARIANT_FALSE);

	return hr;
}

HRESULT CSDKSample_Primitives::Perform()
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
		Ex1(pDoc, pCC, pr);
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		BreakOnFailure(hr, L"Error replacing page content");
		pContent = nullptr;
		// page 2
		hr = AddNewPage(pDoc, pPage, pr);
		if (FAILED(hr))
			break;
		Ex2(pDoc, pCC, pr);
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		BreakOnFailure(hr, L"Error replacing page content");
		pContent = nullptr;
		// page 3
		hr = AddNewPage(pDoc, pPage, pr);
		if (FAILED(hr))
			break;
		Ex3(pDoc, pCC, pr);
		pCC->Detach(&pContent);
		hr = pPage->PlaceContent(pContent, PXC::PlaceContent_Replace);
		BreakOnFailure(hr, L"Error replacing page content");
		pContent = nullptr;
		//
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
