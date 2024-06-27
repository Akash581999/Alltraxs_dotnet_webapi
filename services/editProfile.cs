using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using Org.BouncyCastle.Ocsp;
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
            resData.rData["rCode"] = 0;
            try
            {
                // string base64Image = req.addInfo["ProfilePic"].ToString();
                // byte[] ProfilePic = Convert.FromBase64String(base64Image);

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@FirstName", req.addInfo["FirstName"].ToString()),
                    new MySqlParameter("@LastName", req.addInfo["LastName"].ToString()),
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Mobile", req.addInfo["Mobile"].ToString()),
                    new MySqlParameter("@ProfilePic", req.addInfo["ProfilePic"].ToString())
                };

                var updateSql = @"UPDATE pc_student.Alltraxs_users 
                                SET FirstName = @FirstName, LastName = @LastName, UserName = @UserName, Email = @Email, Mobile = @Mobile," + @" ProfilePic = @ProfilePic 
                                WHERE UserId = @UserId";
                var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                if (rowsAffected != 0)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
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
                            resData.eventID = req.eventID;
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
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}