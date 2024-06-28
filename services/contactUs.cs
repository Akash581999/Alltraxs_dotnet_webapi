using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class contactUs
    {
        dbServices ds = new dbServices();
        public async Task<responseData> ContactUs(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Country", req.addInfo["Country"].ToString()),
                    new MySqlParameter("@Comments", req.addInfo["Comments"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.Alltraxs_users WHERE Email=@Email AND UserName=@UserName;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult == null || checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Email or Username not found, Please enter valid details!";
                }
                else
                {
                    var insertSql = $"INSERT INTO pc_student.Alltraxs_ContactUs (UserName, Email, Country, Comments) VALUES(@UserName, @Email, @Country, @Comments);";
                    var insertId = ds.ExecuteInsertAndGetLastId(insertSql, para);

                    if (insertId != 0)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Thank you for your response";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to submit feedback";
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

        public async Task<responseData> GetAllFeedbacks(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_ContactUs ORDER BY Feedback_Id DESC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all feedbacks!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> feedbackslist = new List<object>();
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
                                var feedback = new
                                {
                                    Feedback_Id = rowData.ElementAtOrDefault(0),
                                    UserName = rowData.ElementAtOrDefault(1),
                                    Email = rowData.ElementAtOrDefault(2),
                                    Country = rowData.ElementAtOrDefault(3),
                                    Comments = rowData.ElementAtOrDefault(4),
                                    CreatedAt = rowData.ElementAtOrDefault(5),
                                };
                                feedbackslist.Add(feedback);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All feedbacks retrieved successfully";
                resData.rData["feedback"] = feedbackslist;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }
    }
}
