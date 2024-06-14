using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class changePassword
    {
        dbServices ds = new dbServices();
        public async Task<responseData> ChangePassword(requestData req)
        {
            responseData resData = new responseData();

            try
            {
                // Update user's password in the database
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString()),
                };
                var updateSql = @"UPDATE pc_student.Alltraxs_users SET UserPassword = @UserPassword WHERE UserId = @UserId;";
                var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);

                if (rowsAffected > 0)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Password changed successfully";
                }
                else
                {
                    var selectSql = @"SELECT * FROM pc_student.Alltraxs_users WHERE UserId = @UserId";
                    var existingDataList = ds.ExecuteSQLName(selectSql, para);

                    if (existingDataList != null && existingDataList.Count > 0)
                    {
                        // User found, but failed to update password
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Password not changed";
                    }
                    else
                    {
                        // No user found with the provided UserId
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "No user found with the provided UserId";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 404;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}
