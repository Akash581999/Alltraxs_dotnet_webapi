using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class resetPassword
    {
        dbServices ds = new dbServices();

        public async Task<responseData> ResetPassword(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                string userId = req.addInfo["UserId"].ToString();
                string oldPassword = req.addInfo["UserPassword"].ToString();
                string newPassword = req.addInfo["NewPassword"].ToString();

                // Check if the old password matches the one stored in the database
                var sq = "SELECT * FROM pc_student.Alltraxs_users WHERE UserId = @UserId AND UserPassword = @UserPassword";
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", userId),
                    new MySqlParameter("@UserPassword", oldPassword)
                };
                var data = ds.ExecuteSQLName(sq, para);

                if (data == null || data[0].Count() == 0)
                {
                    // No matching user found with provided credentials
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Invalid credentials";
                }
                else
                {
                    // Update the password in the database
                    var resetSql = "UPDATE pc_student.Alltraxs_users SET UserPassword = @NewPassword WHERE UserId = @UserId";
                    para = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", userId),
                        new MySqlParameter("@NewPassword", newPassword)
                    };
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(resetSql, para);

                    if (rowsAffected > 0)
                    {
                        // Password reset successful
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Password reset successfully";
                    }
                    else
                    {
                        // No rows affected, probably due to incorrect password
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to reset password";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                resData.rStatus = 404;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}