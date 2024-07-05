using System;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class register
    {
        dbServices ds = new dbServices();
        public async Task<responseData> Register(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Role", req.addInfo["Role"].ToString()),
                    new MySqlParameter("@FirstName", req.addInfo["FirstName"].ToString()),
                    new MySqlParameter("@LastName", req.addInfo["LastName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Mobile", req.addInfo["Mobile"].ToString()),
                    new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString())
                };

                var checkSql = $"SELECT * FROM pc_student.Alltraxs_users WHERE Role=@Role AND Email=@Email AND Mobile=@Mobile;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Already registered, Try Login in!!";
                }
                else
                {
                    var insertSql = @"INSERT INTO pc_student.Alltraxs_users (Role, FirstName, LastName, Email, Mobile, UserPassword) 
                                      VALUES(@Role, @FirstName, @LastName, @Email, @Mobile, @UserPassword);";
                    var insertId = ds.ExecuteInsertAndGetLastId(insertSql, para);
                    if (insertId != 0)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Registered successfully!";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occurred while registration!";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}