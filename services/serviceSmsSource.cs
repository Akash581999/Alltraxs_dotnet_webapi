using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class serviceSmsSource
    {
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _twilioPhoneNumber;

        public serviceSmsSource(IConfiguration configuration)
        {
            _twilioAccountSid = configuration["Twilio:AccountSid"];
            _twilioAuthToken = configuration["Twilio:AuthToken"];
            _twilioPhoneNumber = configuration["Twilio:PhoneNumber"];

            if (string.IsNullOrEmpty(_twilioAccountSid) || string.IsNullOrEmpty(_twilioAuthToken) || string.IsNullOrEmpty(_twilioPhoneNumber))
            {
                throw new InvalidOperationException("Twilio settings are missing or incomplete in appsettings.json.");
            }

            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
        }

        public async Task ServiceSmsSource(string phoneNumber, string message)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                Console.WriteLine($"SMS sent successfully to {result.To}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send SMS: {ex.Message}");
            }
        }
    }
}
