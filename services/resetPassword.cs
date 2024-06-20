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
                string UserId = req.addInfo["UserId"].ToString();
                string UserPassword = req.addInfo["UserPassword"].ToString();
                string NewPassword = req.addInfo["NewPassword"].ToString();
                string ConfirmPassword = req.addInfo["ConfirmPassword"].ToString();

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@UserPassword", UserPassword)
                };

                var sq = "SELECT * FROM pc_student.Alltraxs_users WHERE UserId = @UserId AND UserPassword = @UserPassword;";
                var data = ds.ExecuteSQLName(sq, para);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Invalid credentials";
                }
                else
                {
                    para = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@NewPassword", NewPassword),
                        new MySqlParameter("@ConfirmPassword", ConfirmPassword)
                    };

                    if (NewPassword == ConfirmPassword)
                    {
                        var resetSql = "UPDATE pc_student.Alltraxs_users SET UserPassword = @NewPassword WHERE UserId = @UserId;";
                        var rowsAffected = ds.ExecuteInsertAndGetLastId(resetSql, para);
                        if (rowsAffected > 0)
                        {
                            resData.rData["rCode"] = 3;
                            resData.rData["rMessage"] = "Failed to reset password";
                        }
                        else
                        {
                            resData.eventID = req.eventID;
                            resData.rData["rCode"] = 0;
                            resData.rData["rMessage"] = "Password reset successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}