#pragma once
#include "Sample.h"

class CSDKSample_Fill : public CSDKSample
{
public:
	CSDKSample_Fill();
	virtual ~CSDKSample_Fill();
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
};
