#pragma once

class CDocAuthCallback : public CCmdTarget
{
public:
	CDocAuthCallback();
	~CDocAuthCallback();
public:
	DECLARE_INTERFACE_MAP()
	BEGIN_INTERFACE_PART(LocalClass, PXC::IPXC_DocAuthCallback)
		STDMETHOD(GetTypeInfoCount)(UINT FAR* pctinfo);
		STDMETHOD(GetTypeInfo)(
			UINT itinfo,
			LCID lcid,
			ITypeInfo FAR* FAR* pptinfo);
		STDMETHOD(GetIDsOfNames)(
			REFIID riid,
			OLECHAR FAR* FAR* rgszNames,
			UINT cNames,
			LCID lcid,
			DISPID FAR* rgdispid);
		STDMETHOD(Invoke)(
			DISPID dispidMember,
			REFIID riid,
			LCID lcid,
			WORD wFlags,
			DISPPARAMS FAR* pdispparams,
			VARIANT FAR* pvarResult,
			EXCEPINFO FAR* pexcepinfo,
			UINT FAR* puArgErr);
		STDMETHOD(AuthDoc)(PXC::IPXC_Document* pDoc, ULONG nFlags);
	END_INTERFACE_PART(LocalClass)
public:
};
