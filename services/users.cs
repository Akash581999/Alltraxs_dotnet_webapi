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
    public class users
    {
        dbServices ds = new dbServices();
        public async Task<responseData> GetAllUsers(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_users WHERE Role = 'User' ORDER BY UserId ASC;";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Users not found!!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> usersList = new List<object>();
                foreach (var rowSet in dbData)
                {
                    if (rowSet != null)
                    {
                        foreach (var row in rowSet)
                        {
                            if (row != null)
                            {
                                List<string> rowData = new List<string>();

                                foreach (var column in row)
                                {
                                    if (column != null)
                                    {
                                        rowData.Add(column.ToString());
                                    }
                                }
                                var user = new
                                {
                                    UserId = rowData.ElementAtOrDefault(0),
                                    FirstName = rowData.ElementAtOrDefault(1),
                                    LastName = rowData.ElementAtOrDefault(2),
                                    UserName = rowData.ElementAtOrDefault(3),
                                    UserPassword = rowData.ElementAtOrDefault(4),
                                    Email = rowData.ElementAtOrDefault(5),
                                    Mobile = rowData.ElementAtOrDefault(6),
                                    ProfilePic = rowData.ElementAtOrDefault(7),
                                    CreatedOn = rowData.ElementAtOrDefault(8),
                                };
                                usersList.Add(user);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "Users found successfully";
                resData.rData["users"] = usersList;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> GetUserById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "User details found successfully";
            try
            {
                string input = req.addInfo["Email"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", input)
                };

                var getusersql = $"SELECT * FROM pc_student.Alltraxs_users WHERE Email=@Email;";
                var data = ds.ExecuteSQLName(getusersql, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed to get user details!!";
                }
                else
                {
                    resData.rData["UserId"] = data[0][0]["UserId"];
                    resData.rData["FirstName"] = data[0][0]["FirstName"];
                    resData.rData["LastName"] = data[0][0]["LastName"];
                    resData.rData["UserName"] = data[0][0]["UserName"];
                    resData.rData["Email"] = data[0][0]["Email"];
                    resData.rData["Mobile"] = data[0][0]["Mobile"];
                    resData.rData["ProfilePic"] = data[0][0]["ProfilePic"];
                    resData.rData["CreatedOn"] = data[0][0]["CreatedOn"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }
        public async Task<responseData> DeleteUserById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    // new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString())
                };

                var checkSql = $"SELECT * FROM pc_student.Alltraxs_users WHERE Email = @Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "User not found, No records deleted!";
                }
                else
                {
                    var deleteSql = @"DELETE FROM pc_student.Alltraxs_users WHERE Email = @Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occurred, User not deleted!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "User deleted successfully";
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