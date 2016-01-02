#include <gtest/gtest.h>

class TestEnvironment : public ::testing::Environment
{
	public:
		virtual void SetUp() override;
};

