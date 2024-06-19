using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class playlists
    {
        dbServices ds = new dbServices();
        public async Task<responseData> CreatePlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.eventID = rData.eventID;
            resData.rData["rCode"] = 0;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_Playlists WHERE Title=@Title;";
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@Title", rData.addInfo["Title"])
                };

                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Playlist with this name already exists.";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Playlists(UserId, Title, Description, PlaylistImageUrl, IsPublic) 
                                       VALUES (@UserId, @Title, @Description, @PlaylistImageUrl, @IsPublic);";
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@Title", rData.addInfo["Title"]),
                        new MySqlParameter("@Description", rData.addInfo["Description"]),
                        new MySqlParameter("@PlaylistImageUrl", rData.addInfo["PlaylistImageUrl"]),
                        new MySqlParameter("@IsPublic", rData.addInfo["IsPublic"]),
                    };

                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);
                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Playlist added successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add playlist.";
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

        public async Task<responseData> DeletePlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.eventID = rData.eventID;
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@Playlist_Id", rData.addInfo["Playlist_Id"].ToString())
                };

                var query = $"DELETE FROM pc_student.Alltraxs_Playlists WHERE Playlist_Id = @Playlist_Id;";
                int rowsAffected = ds.ExecuteInsertAndGetLastId(query, myParam);
                if (rowsAffected > 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No playlist found with the provided ID.";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Playlist deleted successfully.";
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

        public async Task<responseData> UpdatePlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.eventID = rData.eventID;
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@Playlist_Id", rData.addInfo["Playlist_Id"]),
                    new MySqlParameter("@Title", rData.addInfo["Title"]),
                    new MySqlParameter("@Description", rData.addInfo["Description"]),
                    new MySqlParameter("@PlaylistImageUrl", rData.addInfo["PlaylistImageUrl"]),
                    new MySqlParameter("@IsPublic", rData.addInfo["IsPublic"]),
                };

                var query = @"UPDATE pc_student.Alltraxs_Playlists
                              SET Title = @Title, Description = @Description, PlaylistImageUrl = @PlaylistImageUrl, IsPublic = @IsPublic
                              WHERE Playlist_Id = @Playlist_Id;";
                int rowsAffected = ds.ExecuteInsertAndGetLastId(query, myParam);
                if (rowsAffected > 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No playlist found with the provided ID.";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Playlist updated successfully.";
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

        public async Task<responseData> GetPlaylist(requestData req)
        {
            responseData resData = new responseData();
            resData.eventID = req.eventID;
            resData.rData["rCode"] = 0;
            resData.rData["rMessage"] = "Playlist found successfully!";

            try
            {
                var list = new ArrayList();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Playlist_Id", req.addInfo["Playlist_Id"]),
                };

                var sq = $"SELECT * FROM pc_student.Alltraxs_Playlists WHERE Playlist_Id=@Playlist_Id;";
                var data = ds.ExecuteSQLName(sq, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No playlist found!";
                }
                else
                {
                    resData.rData["Playlist_Id"] = data[0][0]["Playlist_Id"];
                    resData.rData["UserId"] = data[0][0]["UserId"];
                    resData.rData["Title"] = data[0][0]["Title"];
                    resData.rData["Description"] = data[0][0]["Description"];
                    resData.rData["CreatedOn"] = data[0][0]["CreatedOn"];
                    resData.rData["PlaylistImageUrl"] = data[0][0]["PlaylistImageUrl"];
                    resData.rData["IsPublic"] = data[0][0]["IsPublic"];
                    resData.rData["NumSongs"] = data[0][0]["NumSongs"];
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
