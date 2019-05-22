#pragma once

#include "Sample.h"

class CSDKSample_Stroke : public CSDKSample
{
public:
	CSDKSample_Stroke();
	virtual ~CSDKSample_Stroke();
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
};

