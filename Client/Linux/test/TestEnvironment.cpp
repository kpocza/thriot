#include "TestEnvironment.h"
#include "ClientSettings.h"

void TestEnvironment::SetUp()
{
	Thriot::ClientSettings::Instance().SetupTlsValidation(false, false);
}

::testing::Environment* const test_env = ::testing::AddGlobalTestEnvironment(new TestEnvironment());
