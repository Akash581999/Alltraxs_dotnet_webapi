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
            resData.rData["rCode"] = 0;
            try
            {
                string UserId = req.addInfo["UserId"].ToString();
                string NewPassword = req.addInfo["NewPassword"].ToString();
                string ConfirmPassword = req.addInfo["ConfirmPassword"].ToString();

                if (NewPassword != ConfirmPassword)
                {
                    resData.rData["rCode"] = 3;
                    resData.rData["rMessage"] = "New password and confirm password must match!";
                }
                else
                {
                    var para = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@NewPassword", NewPassword)
                    };

                    var selectSql = $"SELECT * FROM pc_student.Alltraxs_users WHERE UserId = @UserId;";
                    var data = ds.ExecuteSQLName(selectSql, para);
                    if (data == null || data[0].Count() == 0)
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "UserId not found, Please enter a valid UserId";
                    }
                    else
                    {
                        var updateSql = $"UPDATE pc_student.Alltraxs_users SET UserPassword = @NewPassword WHERE UserId = @UserId;";
                        var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                        if (rowsAffected > 0)
                        {
                            resData.rData["rCode"] = 3;
                            resData.rData["rMessage"] = "Some error occured, could'nt reset password!";
                        }
                        else
                        {
                            selectSql = $"SELECT * FROM pc_student.Alltraxs_users WHERE UserId = @UserId AND UserPassword=@NewPassword;";
                            data = ds.ExecuteSQLName(selectSql, para);
                            if (data.Count() == 0)
                            {
                                resData.rData["rCode"] = 4;
                                resData.rData["rMessage"] = "Password already exist, new password must be different!";
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