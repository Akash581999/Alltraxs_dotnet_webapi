// using System;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Options;
// using MySql.Data.MySqlClient;
// using Twilio;
// using Twilio.Types;
// using Twilio.Rest.Api.V2010.Account;

// namespace COMMON_PROJECT_STRUCTURE_API.services
// {
//     public class resetPassword
//     {
//         private readonly dbServices ds = new dbServices();
//         private readonly serviceSmsSource ts;

//         public resetPassword(IOptions<serviceSmsSource> twilioOptions, dbServices dbServices)
//         {
//             ds = dbServices;
//             ts = twilioOptions.Value;
//         }
//         public async Task<responseData> ResetPassword(requestData req)
//         {
//             var resData = new responseData();
//             resData.eventID = req.eventID;
//             resData.rData["rCode"] = 1;
//             try
//             {
//                 string userId = req.addInfo["UserId"].ToString();
//                 string newPassword = req.addInfo["NewPassword"].ToString();

//                 string otp = GenerateOTP();

//                 bool otpSent = await SendOTPViaTwilio(userId, otp);
//                 if (!otpSent)
//                 {
//                     resData.rData["rMessage"] = "Failed to send OTP via SMS";
//                     return resData;
//                 }

//                 resData.rData["rCode"] = 0;
//                 resData.rData["rMessage"] = "OTP sent successfully";
//                 resData.rData["otp"] = otp;
//             }
//             catch (Exception ex)
//             {
//                 resData.rData["rMessage"] = $"Error: {ex.Message}";
//             }
//             return resData;
//         }

//         public async Task<responseData> VerifyOTPAsync(requestData req)
//         {
//             var resData = new responseData();
//             resData.rData["rCode"] = 1;
//             try
//             {
//                 string otp = req.addInfo["otp"].ToString();

//                 if (VerifyOTPinDatabase(otp))
//                 {
//                     string userId = req.addInfo["UserId"].ToString();
//                     string newPassword = req.addInfo["NewPassword"].ToString();

//                     string updateSql = "UPDATE pc_student.Alltraxs_users SET UserPassword = @NewPassword WHERE UserId = @UserId";
//                     MySqlParameter[] parameters = new MySqlParameter[]
//                     {
//                         new MySqlParameter("@UserId", userId),
//                         new MySqlParameter("@NewPassword", newPassword)
//                     };
//                     var rowsAffected = ds.executeSQLpcmdb(updateSql, parameters);

//                     if (rowsAffected[0].Count() == 0)
//                     {
//                         resData.rData["rCode"] = 1;
//                         resData.rData["rMessage"] = "Failed to update password";
//                     }
//                     else
//                     {
//                         resData.rData["rCode"] = 0;
//                         resData.rData["rMessage"] = "Password updated successfully";
//                     }
//                 }
//                 else
//                 {
//                     resData.rData["rCode"] = 1;
//                     resData.rData["rMessage"] = "OTP verification failed";
//                 }
//             }
//             catch (Exception ex)
//             {
//                 resData.rData["rMessage"] = $"Error: {ex.Message}";
//             }
//             return resData;
//         }

//         private string GenerateOTP()
//         {
//             Random random = new Random();
//             return random.Next(100000, 999999).ToString();
//         }

//         private async Task<bool> SendOTPViaTwilio(string phoneNumber, string otp)
//         {
//             try
//             {
//                 TwilioClient.Init(ts.AccountSid, ts.AuthToken);
//                 var message = await MessageResource.CreateAsync(
//                     body: $"Your OTP for password reset is {otp}",
//                     from: new PhoneNumber(ts.PhoneNumber),
//                     to: new PhoneNumber(phoneNumber)
//                 );
//                 return true;
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error sending OTP via Twilio: {ex.Message}");
//                 return false;
//             }
//         }

//         private bool VerifyOTPinDatabase(string otp)
//         {
//             return true;
//         }
//     }
// }
