using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class login
    {
        dbServices ds = new dbServices();
        decryptService cm = new decryptService();
        private readonly Dictionary<string, string> jwt_config = new Dictionary<string, string>();
        private readonly Dictionary<string, string> service_config = new Dictionary<string, string>();
        IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public login()
        {
            jwt_config["Key"] = appsettings["jwt_config:Key"].ToString();
            jwt_config["Issuer"] = appsettings["jwt_config:Issuer"].ToString();
            jwt_config["Audience"] = appsettings["jwt_config:Audience"].ToString();
            jwt_config["Subject"] = appsettings["jwt_config:Subject"].ToString();
            jwt_config["ExpiryDuration_app"] = appsettings["jwt_config:ExpiryDuration_app"].ToString();
            jwt_config["ExpiryDuration_web"] = appsettings["jwt_config:ExpiryDuration_web"].ToString();
        }
        public async Task<responseData> Login(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string input = req.addInfo["UserId"].ToString();
                bool isEmail = IsValidEmail(input);
                bool isMobileNumber = IsValidMobileNumber(input);
                string columnName;
                if (isEmail)
                {
                    columnName = "Email";
                }
                else if (isMobileNumber)
                {
                    columnName = "Mobile";
                }
                else
                {
                    columnName = "";
                }

                MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@UserId", input),
                new MySqlParameter("@Role", req.addInfo["Role"].ToString()),
                new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString())
                };

                var sq = $"SELECT * FROM pc_student.Alltraxs_users WHERE Role=@Role AND {columnName} = @UserId AND UserPassword = @UserPassword;";
                var data = ds.ExecuteSQLName(sq, myParams);

                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Wrong details, use valid credentials!";
                }
                else
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_config["Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var Sectoken = new JwtSecurityToken(jwt_config["Issuer"],
                      jwt_config["Issuer"],
                      null,
                      expires: DateTime.Now.AddMinutes(120),
                      signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Login Successfully, Welcome!";
                    resData.rData["rCode"] = token;
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 404;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message;
            }
            return resData;
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsValidMobileNumber(string phoneNumber)
        {
            string pattern = @"^[0-9]{7,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}