using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class ServiceSmsSource
{
    private readonly string _twilioAccountSid;
    private readonly string _twilioAuthToken;
    private readonly string _twilioPhoneNumber;

    public ServiceSmsSource()
    {
    }

    public ServiceSmsSource(IConfiguration configuration)
    {
        _twilioAccountSid = configuration["Twilio:AccountSid"];
        _twilioAuthToken = configuration["Twilio:AuthToken"];
        _twilioPhoneNumber = configuration["Twilio:PhoneNumber"];
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            if (string.IsNullOrEmpty(_twilioAccountSid))
            {
                Console.WriteLine("Twilio account SID is not configured.");
                return;
            }
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

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
