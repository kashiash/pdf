#include "stdafx.h"
#include "InitializeSDK.h"

// comment this define if you do not want to use registered CoClass
// #define REGISTERED_COM_SDK

// comment this define if you do not use dynamic load of the PDF-XChange Core SDK DLL
#define DYNAMIC_LOAD_SDK_DLL

#ifdef REGISTERED_COM_SDK
#undef DYNAMIC_LOAD_SDK_DLL
#endif

#ifdef DYNAMIC_LOAD_SDK_DLL
// uncomment this link if you use delay load of the SDK DLL; otherwise InitializeSDK
// will use GetProcAddress
// #define USE_DELAY_LOAD
#endif


HMODULE hSDKInst = NULL;

PXC::IPXC_Inst*				g_Inst			= nullptr;
PXC::IPXS_Inst*				g_COS			= nullptr;
PXC::IIXC_Inst*				g_ImgCore		= nullptr;
PXC::IAUX_Inst*				g_AUX			= nullptr;
CComPtr<PXC::IMathHelper>	g_MathHelper	= nullptr;

#if defined _M_X64

	#define SDK_DLL L"PDFXCoreAPI.x64.dll"

	#ifdef _DEBUG
		#define SDK_PATH L"%PXCC_BIN64D_PATH%"
	#else	// _DEBUG
		#define SDK_PATH L"%PXCC_BIN64R_PATH%"
	#endif	// _DEBUG

#else	// _M_X64

	#define SDK_DLL L"PDFXCoreAPI.x86.dll"

	#ifdef _DEBUG
		#define SDK_PATH L"%PXCC_BIN32D_PATH%"
	#else	// _DEBUG
		#define SDK_PATH L"%PXCC_BIN32R_PATH%"
	#endif	// _DEBUG

#endif

typedef HRESULT (WINAPI *fnPXC_GetInstance)(PXC::IPXC_Inst** ppInst);

static LPCWSTR sMySDKKey	= L"";

HRESULT InitializeSDK()
{
	HRESULT hr = S_OK;
	do 
	{
#if defined (REGISTERED_COM_SDK)
		hr = CoCreateInstance(__uuidof(PXC::PXC_Inst), nullptr, CLSCTX_INPROC_SERVER, __uuidof(PXC::IPXC_Inst), (void**)&g_Inst);
#elif defined (DYNAMIC_LOAD_SDK_DLL)
		if (g_Inst != nullptr)
			break;
		hSDKInst = LoadLibrary(SDK_DLL);
		if (hSDKInst == nullptr)
		{
			WCHAR path[MAX_PATH + 1] = {0};
			ExpandEnvironmentStrings(SDK_PATH, path, _countof(path));
			lstrcat(path, L"\\");
			lstrcat(path, SDK_DLL);
			hSDKInst = LoadLibrary(path);
		}
		BreakOnNullWithLastError(hSDKInst, hr, L"Error loading SDK DLL '%ws': %.8lx", SDK_DLL, GetLastError());
#ifdef USE_DELAY_LOAD
		hr = PXC_GetInstance(&g_Inst);
#else
		fnPXC_GetInstance pfn = (fnPXC_GetInstance)GetProcAddress(hSDKInst, "PXC_GetInstance");
		BreakOnNull(pfn, hr, E_FAIL, L"Invalid SDK DLL - function PXC_GetInstance not found");
		hr = pfn(&g_Inst);
#endif	// USE_DELAY_LOAD
#else
		hr = PXC_GetInstance(&g_Inst);
#endif	// DYNAMIC_LOAD_SDK_DLL
		BreakOnFailure(hr, L"Error in call PXC_GetInstance: %.8lx", hr);
		BreakOnNull(g_Inst, hr, E_FAIL, L"Invalid result of PXC_GetInstance");
		g_Inst->Init((LPWSTR)sMySDKKey);
		CComPtr<IUnknown> pUnk;
		pUnk = nullptr;
		g_Inst->GetExtension(L"PXS", &pUnk);
		if (pUnk != nullptr)
			pUnk->QueryInterface(&g_COS);
		pUnk = nullptr;
		g_Inst->GetExtension(L"IXC", &pUnk);
		if (pUnk != nullptr)
			pUnk->QueryInterface(&g_ImgCore);
		pUnk = nullptr;
		g_Inst->GetExtension(L"AUX", &pUnk);
		if (pUnk != nullptr)
			pUnk->QueryInterface(&g_AUX);

		BreakOnNull(g_COS, hr, E_FAIL, L"Cannot get COS instance object");
		BreakOnNull(g_ImgCore, hr, E_FAIL, L"Cannot get IXC instance object");
		BreakOnNull(g_AUX, hr, E_FAIL, L"Cannot get AUX instance object");

		g_AUX->get_MathHelper(&g_MathHelper);

		ULONG nVer = 0;
		g_Inst->get_APIVersion(&nVer);
		LOG(S_OK, L"SDK DLL successfully loaded.");
		LOG(S_OK, L"API version: <b>%d.%d.%d.%d</b> (0x%.8lx)", HIBYTE(HIWORD(nVer)), LOBYTE(HIWORD(nVer)), HIBYTE(LOWORD(nVer)), LOBYTE(LOWORD(nVer)), nVer);
	} while (false);
	return hr;
}

HRESULT FinalizeSDK()
{
	g_MathHelper = nullptr;
	if (g_Inst != nullptr)
	{
		g_Inst->Finalize();
		g_Inst->Release();
		g_Inst = nullptr;
	}
	if (hSDKInst != nullptr)
		FreeLibrary(hSDKInst);
	return S_OK;
}
