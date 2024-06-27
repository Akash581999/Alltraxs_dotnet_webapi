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
                var query = @"SELECT * FROM pc_student.Alltraxs_users ORDER BY UserId ASC";
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
                                    Email = rowData.ElementAtOrDefault(4),
                                    Mobile = rowData.ElementAtOrDefault(5),
                                    ProfilePic = rowData.ElementAtOrDefault(6),
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

        public bool CheckPhoneNumberExists(string phoneNumber)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=210.210.210.50;user=test_user;password=test*123;port=2020;database=pc_student;"))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Kapil_signup WHERE phone = @phone";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@phone", phoneNumber);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while executing query: " + ex.Message);
                return false;
            }
        }
    }
}