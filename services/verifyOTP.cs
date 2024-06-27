using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace dotnetApi.services
{
    public class verifyOTP
    {
        private readonly dbServices db = new dbServices();
        private readonly Dictionary<string, string> jwt_config = new Dictionary<string, string>();
        IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public async Task<responseData> VerifyOTP(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rcode"] = 0;
            try
            {
                string otp = req.addInfo["otp"].ToString();
                MySqlParameter[] verify = new MySqlParameter[]
                {
                    new MySqlParameter("@otp", otp)
                };

                var sq = @"SELECT * FROM OTP_reg WHERE otp = @otp AND status = 1";
                var data = db.executeSQL(sq, verify);
                if (data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Invalid OTP";
                }
                else
                {
                    MySqlParameter[] myPar = new MySqlParameter[]
                    {
                        new MySqlParameter("@otp", otp),
                        new MySqlParameter("@status", 0)
                    };
                    var query = @"UPDATE OTP_reg SET status = @status WHERE otp = @otp";
                    var dbData = db.executeSQL(query, myPar);
                    resData.rData["rMessage"] = "OTP is valid for 10 minutes";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rcode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;
        }
    }
}