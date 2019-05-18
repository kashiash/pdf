// This MFC Samples source code demonstrates using MFC Microsoft Office Fluent User Interface 
// (the "Fluent UI") and is provided only as referential material to supplement the 
// Microsoft Foundation Classes Reference and related electronic documentation 
// included with the MFC C++ library software.  
// License terms to copy, use or distribute the Fluent UI are available separately.  
// To learn more about our Fluent UI licensing program, please visit 
// http://go.microsoft.com/fwlink/?LinkId=238214.
//
// Copyright (C) Microsoft Corporation
// All rights reserved.

// MFCSample.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "afxwinappex.h"
#include "afxdialogex.h"
#include "MFCSample.h"
#include "MainFrm.h"

//////////////////////////////////////////////////////////////////////////
#include "Sample_Stroke.h"
#include "Sample_Fill.h"
#include "Sample_Primitives.h"
#include "Sample_Text.h"
#include "Sample_ViewDoc.h"
#include "Sample_DigiSig.h"
#include "Sample_ExportToImage.h"
#include "Sample_PrintDoc.h"

//////////////////////////////////////////////////////////////////////////

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCSampleApp

BEGIN_MESSAGE_MAP(CMFCSampleApp, CWinAppEx)
	ON_COMMAND(ID_APP_ABOUT, &CMFCSampleApp::OnAppAbout)
END_MESSAGE_MAP()


// CMFCSampleApp construction

CMFCSampleApp::CMFCSampleApp()
{
	m_bHiColorIcons = TRUE;

	// support Restart Manager
	m_dwRestartManagerSupportFlags = AFX_RESTART_MANAGER_SUPPORT_RESTART;
#ifdef _MANAGED
	// If the application is built using Common Language Runtime support (/clr):
	//     1) This additional setting is needed for Restart Manager support to work properly.
	//     2) In your project, you must add a reference to System.Windows.Forms in order to build.
	System::Windows::Forms::Application::SetUnhandledExceptionMode(System::Windows::Forms::UnhandledExceptionMode::ThrowException);
#endif

	// TODO: replace application ID string below with unique ID string; recommended
	// format for string is CompanyName.ProductName.SubProduct.VersionInformation
	SetAppID(_T("TrackerSoftware.PDFXChange.SDKSamples.NoVersion"));

	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}

// The one and only CMFCSampleApp object

CMFCSampleApp theApp;

// CMFCSampleApp initialization

BOOL CMFCSampleApp::InitInstance()
{
	// InitCommonControlsEx() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// Set this to include all the common control classes you want to use
	// in your application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinAppEx::InitInstance();


	// Initialize OLE libraries
	if (!AfxOleInit())
	{
		AfxMessageBox(IDP_OLE_INIT_FAILED);
		return FALSE;
	}

	AfxEnableControlContainer();

	EnableTaskbarInteraction(FALSE);

	// AfxInitRichEdit2() is required to use RichEdit control	
	AfxInitRichEdit2();

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization
	SetRegistryKey(_T("Tracker Software\\PDF-XChange SDK"));


	InitContextMenuManager();
	InitShellManager();

	InitKeyboardManager();

	InitTooltipManager();

	InitializeSamples();

	CMFCToolTipInfo ttParams;
	ttParams.m_bVislManagerTheme = TRUE;
	theApp.GetTooltipManager()->SetTooltipParams(AFX_TOOLTIP_TYPE_ALL, RUNTIME_CLASS(CMFCToolTipCtrl), &ttParams);

	// To create the main window, this code creates a new frame window
	// object and then sets it as the application's main window object
	CMainFrame* pFrame = new CMainFrame;
	if (!pFrame)
		return FALSE;
	m_pMainWnd = pFrame;
	// create and load the frame with its resources
	pFrame->LoadFrame(IDR_MAINFRAME,
					  WS_OVERLAPPEDWINDOW | FWS_ADDTOTITLE,
					  NULL, NULL);

	// The one and only window has been initialized, so show and update it
	pFrame->ShowWindow(SW_SHOW);
	pFrame->UpdateWindow();
	return TRUE;
}

int CMFCSampleApp::ExitInstance()
{
	//TODO: handle additional resources you may have added
	AfxOleTerm(FALSE);

	FinalizeSamples();

	return CWinAppEx::ExitInstance();
}

bool CMFCSampleApp::AddSample(CSDKSample* pSample)
{
	if (!pSample)
		return false;
	m_Samples.push_back(pSample);
	return true;
}

void CMFCSampleApp::InitializeSamples()
{
	AddSample(new CSDKSample_Stroke);
	AddSample(new CSDKSample_Fill);
	AddSample(new CSDKSample_Primitives);
	AddSample(new CSDKSample_Text);
	AddSample(new CSDKSample_ViewDoc);
	AddSample(new CSDKSample_DigiSig);
	AddSample(new CSDKSample_ExportToImage);
	AddSample(new CSDKSample_PrintDoc);
}

void CMFCSampleApp::FinalizeSamples()
{
	for (auto* pS : m_Samples)
		delete pS;
	m_Samples.clear();
}

// CMFCSampleApp message handlers

// CAboutDlg dialog used for App About

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnNMClickSyslink1(NMHDR *pNMHDR, LRESULT *pResult);
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
	ON_NOTIFY(NM_CLICK, IDC_SYSLINK1, &CAboutDlg::OnNMClickSyslink1)
END_MESSAGE_MAP()

// App command to run the dialog
void CMFCSampleApp::OnAppAbout()
{
	CAboutDlg aboutDlg;
	aboutDlg.DoModal();
}

// CMFCSampleApp customization load/save methods

void CMFCSampleApp::PreLoadState()
{
	BOOL bNameValid;
	CString strName;
	bNameValid = strName.LoadString(IDS_EDIT_MENU);
	ASSERT(bNameValid);
	GetContextMenuManager()->AddMenu(strName, IDR_POPUP_EDIT);
}

void CMFCSampleApp::LoadCustomState()
{
}

void CMFCSampleApp::SaveCustomState()
{
}

// CMFCSampleApp message handlers

void CAboutDlg::OnNMClickSyslink1(NMHDR *pNMHDR, LRESULT *pResult)
{
	PNMLINK pNMLink = (PNMLINK)pNMHDR;
	ShellExecuteW(NULL, L"open", pNMLink->item.szUrl, NULL, NULL, SW_SHOWNORMAL);
	

	*pResult = 0;
}
