#pragma once


extern PXC::IPXC_Inst*				g_Inst;
extern PXC::IPXS_Inst*				g_COS;
extern PXC::IIXC_Inst*				g_ImgCore;
extern PXC::IAUX_Inst*				g_AUX;
extern CComPtr<PXC::IMathHelper>	g_MathHelper;

HRESULT InitializeSDK();
HRESULT FinalizeSDK();
