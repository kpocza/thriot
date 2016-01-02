#pragma once

namespace Thriot
{
/** Setup class for Client library settings */
class ClientSettings
{
	private:
		ClientSettings();

		bool _validateCertificate;
		bool _validateHostname;

	public:
		static ClientSettings& Instance();

		void SetupTlsValidation(bool validateCertificate, bool validateHostname);

		bool IsValidateCertificate();
		bool IsValidateHostname();
};
}
