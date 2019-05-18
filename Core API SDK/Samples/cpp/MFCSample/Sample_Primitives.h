#pragma once

#include "Sample.h"

class CSDKSample_Primitives : public CSDKSample
{
public:
	CSDKSample_Primitives();
	virtual ~CSDKSample_Primitives();
public:
	LPCWSTR GetGroup() override;
	LPCWSTR GetTitle() override;
	LPCWSTR GetDescription() override;
	//
	HRESULT Perform() override;
};

