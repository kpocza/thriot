#include "ClientSettings.h"

namespace Thriot
{
ClientSettings::ClientSettings()
{
	_validateCertificate = true;
	_validateHostname = true;
}

/**
Returns a singleton instance of the Thriot library client settings setup class
*/
ClientSettings &ClientSettings::Instance()
{
	static ClientSettings instance;

	return instance;
}

/**
Setup Tls Validation settings

@param validateCertificate Validate certificate trust chain - setting this to false enables self-signed certs
@param validateHostname Validate certificate host (HTTPS only) - settings this to false enabled host name mismatches
*/
void ClientSettings::SetupTlsValidation(bool validateCertificate, bool validateHostname)
{
	_validateCertificate = validateCertificate;
	_validateHostname = validateHostname;
}

/**
Returns whether certificate trust chain validation is enabled or not

@return setting true/false
*/
bool ClientSettings::IsValidateCertificate()
{
	return _validateCertificate;
}

/**
Returns whether certificate host name is matched to the target host name

@return setting true/false
*/
bool ClientSettings::IsValidateHostname()
{
	return _validateHostname;
}
}
