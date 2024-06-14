using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class editProfile
    {
        dbServices ds = new dbServices();
        public async Task<responseData> EditProfile(requestData req)
        {
            responseData resData = new responseData();

            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@ROLE_ID", req.addInfo["ROLE_ID"].ToString()),
                    new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString()),
                    new MySqlParameter("@FirstName", req.addInfo["FirstName"].ToString()),
                    new MySqlParameter("@LastName", req.addInfo["LastName"].ToString()),
                    new MySqlParameter("@EMAIL_ID", req.addInfo["EMAIL_ID"].ToString()),
                    new MySqlParameter("@MOBILE_NO", req.addInfo["MOBILE_NO"].ToString())
                };

                var updateSql = @"
                    UPDATE pc_student.Alltraxs_users 
                    SET ROLE_ID = @ROLE_ID, UserPassword = @UserPassword, 
                    FirstName = @FirstName, LastName = @LastName, 
                    EMAIL_ID = @EMAIL_ID, MOBILE_NO = @MOBILE_NO 
                    WHERE UserId = @UserId
                ";

                var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);

                if (rowsAffected > 0)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Profile updated successfully";
                }
                else
                {
                    var selectSql = @"SELECT * FROM pc_student.Alltraxs_users WHERE UserId = @UserId";

                    var existingDataList = ds.ExecuteSQLName(selectSql, para);

                    if (existingDataList != null && existingDataList.Count > 0)
                    {
                        var currentData = existingDataList[0];
                        bool changesDetected = false;

                        if (changesDetected = true)
                        {
                            resData.rData["rCode"] = 0;
                            resData.rData["rMessage"] = "Profile updated successfully";
                        }
                        else
                        {
                            resData.rData["rCode"] = 2;
                            resData.rData["rMessage"] = "No changes were made";
                        }
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
