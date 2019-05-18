#pragma once

#include "Sample.h"

class CSDKSample_Text : public CSDKSample
{
public:
	CSDKSample_Text();
	virtual ~CSDKSample_Text();
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
};

