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
    public class subscriptions
    {
        dbServices ds = new dbServices();
        public async Task<responseData> GetAllSubscriptions(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_Subscriptions ORDER BY SubscriptionId ASC;";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Subscriptions not found!!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> SubscriptionsList = new List<object>();
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
                                var Subscription = new
                                {
                                    SubscriptionId = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    UserName = rowData.ElementAtOrDefault(2),
                                    Email = rowData.ElementAtOrDefault(3),
                                    PlanType = rowData.ElementAtOrDefault(4),
                                    CouponCode = rowData.ElementAtOrDefault(5),
                                    PaymentDate = rowData.ElementAtOrDefault(6),
                                    StartDate = rowData.ElementAtOrDefault(7),
                                    EndDate = rowData.ElementAtOrDefault(8),
                                    LastUpdated = rowData.ElementAtOrDefault(9),
                                    Active = rowData.ElementAtOrDefault(10),
                                };
                                SubscriptionsList.Add(Subscription);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "Subscriptions found successfully";
                resData.rData["Subscriptions"] = SubscriptionsList;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> GetSubscriptionById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Subscription details found successfully";
            try
            {
                string input = req.addInfo["Email"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", input)
                };

                var getSubscriptionsql = $"SELECT * FROM pc_student.Alltraxs_Subscriptions WHERE Email=@Email;";
                var data = ds.ExecuteSQLName(getSubscriptionsql, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed to get Subscription details!!";
                }
                else
                {
                    resData.rData["SubscriptionId"] = data[0][0]["SubscriptionId"];
                    resData.rData["UserId"] = data[0][0]["UserId"];
                    resData.rData["UserName"] = data[0][0]["UserName"];
                    resData.rData["Email"] = data[0][0]["Email"];
                    resData.rData["PlanType"] = data[0][0]["PlanType"];
                    resData.rData["CouponCode"] = data[0][0]["CouponCode"];
                    resData.rData["PaymentDate"] = data[0][0]["PaymentDate"];
                    resData.rData["StartDate"] = data[0][0]["StartDate"];
                    resData.rData["EndDate"] = data[0][0]["EndDate"];
                    resData.rData["LastUpdated"] = data[0][0]["LastUpdated"];
                    resData.rData["Active"] = data[0][0]["Active"];
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
        public async Task<responseData> CancelSubscriptionById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    // new MySqlParameter("@SubscriptionId", req.addInfo["SubscriptionId"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString())
                };

                var checkSql = $"SELECT * FROM pc_student.Alltraxs_Subscriptions WHERE Email = @Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Subscription not found, No records cancelled!";
                }
                else
                {
                    var deleteSql = @"DELETE FROM pc_student.Alltraxs_Subscriptions WHERE Email = @Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occurred, Subscription not cancelled!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Subscription cancelled successfully";
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
        public async Task<responseData> CreateSubscription(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@UserName", rData.addInfo["UserName"]),
                    new MySqlParameter("@Email", rData.addInfo["Email"]),
                    new MySqlParameter("@PlanType", rData.addInfo["PlanType"]),
                    new MySqlParameter("@CouponCode", rData.addInfo["CouponCode"]),
                    new MySqlParameter("@PaymentDate", rData.addInfo["PaymentDate"]),
                    new MySqlParameter("@StartDate", rData.addInfo["StartDate"]),
                    new MySqlParameter("@EndDate", rData.addInfo["EndDate"]),
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_users WHERE Email=@Email;";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "User dont exists, please register first!";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Subscriptions(UserId, UserName, Email, PlanType, CouponCode, PaymentDate, StartDate, EndDate) 
                                       VALUES (@UserId, @UserName, @Email, @PlanType, @CouponCode, @PaymentDate, @StartDate, @EndDate);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, myParam);
                    if (rowsAffected != 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "User subscription added successfully.";
                    }
                    else if (rowsAffected == 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add subscription!";
                    }
                    else
                    {
                        resData.rData["rCode"] = 4;
                        resData.rData["rMessage"] = "Subscription is still active, Failed to renew!";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"User already have active subscription!: {ex.Message}, Failed to renew!";
            }
            return resData;
        }
        public async Task<responseData> EditSubscriptionById(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@UserName", rData.addInfo["UserName"]),
                    new MySqlParameter("@Email", rData.addInfo["Email"]),
                    new MySqlParameter("@PlanType", rData.addInfo["PlanType"]),
                    new MySqlParameter("@CouponCode", rData.addInfo["CouponCode"]),
                    new MySqlParameter("@PaymentDate", rData.addInfo["PaymentDate"]),
                    new MySqlParameter("@Active", rData.addInfo["Active"]),
                };
                var query = @"SELECT * FROM pc_student.Alltraxs_Subscriptions WHERE Email=@Email;";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Subscription not found!!.";
                }
                else
                {
                    var updatequery = @"UPDATE pc_student.Alltraxs_Subscriptions
                                        SET UserName = @UserName, Email = @Email, PlanType = @PlanType, CouponCode=@CouponCode, PaymentDate=@PaymentDate, Active=@Active
                                        WHERE Email = @Email;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(updatequery, myParam);
                    if (rowsAffected == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Subscription updated successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, could'nt update Subscription!";
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