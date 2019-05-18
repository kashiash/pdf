#include "stdafx.h"
#include "AuthCallback.h"
#include "InitializeSDK.h"


CDocAuthCallback::CDocAuthCallback()
{
}

CDocAuthCallback::~CDocAuthCallback()
{
}

BEGIN_INTERFACE_MAP(CDocAuthCallback, CCmdTarget)
	INTERFACE_PART(CDocAuthCallback, __uuidof(PXC::IPXC_DocAuthCallback), LocalClass)
END_INTERFACE_MAP()

STDMETHODIMP_(ULONG) CDocAuthCallback::XLocalClass::AddRef()
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		return pThis->ExternalAddRef();
}

STDMETHODIMP_(ULONG) CDocAuthCallback::XLocalClass::Release()
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		return pThis->ExternalRelease();
}

STDMETHODIMP CDocAuthCallback::XLocalClass::QueryInterface(REFIID iid, LPVOID* ppvObj)
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		return pThis->ExternalQueryInterface(&iid, ppvObj);
}

STDMETHODIMP CDocAuthCallback::XLocalClass::GetTypeInfoCount(UINT FAR* pctinfo)
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		LPDISPATCH lpDispatch = pThis->GetIDispatch(FALSE);
	ASSERT(lpDispatch != NULL);
	return lpDispatch->GetTypeInfoCount(pctinfo);
}

STDMETHODIMP CDocAuthCallback::XLocalClass::GetTypeInfo(UINT itinfo, LCID lcid, ITypeInfo FAR* FAR* pptinfo)
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		LPDISPATCH lpDispatch = pThis->GetIDispatch(FALSE);
	ASSERT(lpDispatch != NULL);
	return lpDispatch->GetTypeInfo(itinfo, lcid, pptinfo);
}

STDMETHODIMP CDocAuthCallback::XLocalClass::GetIDsOfNames(REFIID riid, OLECHAR FAR* FAR* rgszNames, UINT cNames, LCID lcid, DISPID FAR* rgdispid)
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		LPDISPATCH lpDispatch = pThis->GetIDispatch(FALSE);
	ASSERT(lpDispatch != NULL);
	return lpDispatch->GetIDsOfNames(riid, rgszNames, cNames, 
		lcid, rgdispid);
}

STDMETHODIMP CDocAuthCallback::XLocalClass::Invoke(DISPID dispidMember, REFIID riid, LCID lcid, WORD wFlags,
													  DISPPARAMS FAR* pdispparams, VARIANT FAR* pvarResult,
													  EXCEPINFO FAR* pexcepinfo, UINT FAR* puArgErr)
{
	METHOD_PROLOGUE(CDocAuthCallback, LocalClass)
		LPDISPATCH lpDispatch = pThis->GetIDispatch(FALSE);
	ASSERT(lpDispatch != NULL);
	return lpDispatch->Invoke(dispidMember, riid, lcid,
		wFlags, pdispparams, pvarResult,
		pexcepinfo, puArgErr);
}

STDMETHODIMP CDocAuthCallback::XLocalClass::AuthDoc(PXC::IPXC_Document* pDoc, ULONG nFlags)
{
	PXC::PXC_PermStatus status = PXC::Perm_ReqDenied;
	HRESULT hr = E_NOTIMPL; // default behaviour
	PXC::IPXC_SecurityHandlerPtr pHandler;
	pDoc->GetSecurityHandler(VARIANT_FALSE, &pHandler);
	if (pHandler == nullptr)
		return E_FAIL;
	ULONG atmStandard;
	ULONG nName;
	g_COS->StrToAtom(L"Standard", &atmStandard);
	pHandler->get_Name(&nName);
	if (nName == atmStandard)
	{
		CComBSTR sPass = L"password";
		hr = pDoc->AuthorizeWithPassword(sPass, &status);
	}
	return hr;
}
