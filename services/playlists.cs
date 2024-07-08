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
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@Title", rData.addInfo["Title"]),
                    new MySqlParameter("@Description", rData.addInfo["Description"]),
                    new MySqlParameter("@PlaylistImageUrl", rData.addInfo["PlaylistImageUrl"]),
                    new MySqlParameter("@Type", rData.addInfo["Type"]),
                    new MySqlParameter("@NumSongs", rData.addInfo["NumSongs"]),
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Playlists WHERE Title=@Title;";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Playlist with this name already exists!";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Playlists(UserId, Title, Description, PlaylistImageUrl, Type, NumSongs) 
                                       VALUES (@UserId, @Title, @Description, @PlaylistImageUrl, @Type, @NumSongs);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, myParam);
                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Playlist created successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to created playlist!";
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

        public async Task<responseData> DeletePlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    // new MySqlParameter("@Playlist_Id", rData.addInfo["Playlist_Id"].ToString()),
                    new MySqlParameter("@Title", rData.addInfo["Title"].ToString())
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Playlists WHERE Title=@Title;";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Playlist not found!!.";
                }
                else
                {
                    var delquery = $"DELETE FROM pc_student.Alltraxs_Playlists WHERE Title = @Title;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(delquery, myParam);
                    if (rowsAffected != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occurred, could'nt delete playlist!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Playlist deleted successfully.";
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

        public async Task<responseData> UpdatePlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@Playlist_Id", rData.addInfo["Playlist_Id"]),
                    new MySqlParameter("@Title", rData.addInfo["Title"]),
                    new MySqlParameter("@Description", rData.addInfo["Description"]),
                    new MySqlParameter("@PlaylistImageUrl", rData.addInfo["PlaylistImageUrl"]),
                    new MySqlParameter("@Type", rData.addInfo["Type"]),
                    new MySqlParameter("@NumSongs", rData.addInfo["NumSongs"])
                };
                var query = @"SELECT * FROM pc_student.Alltraxs_Playlists WHERE Playlist_Id=@Playlist_Id;";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Playlist not found!!.";
                }
                else
                {
                    var updatequery = @"UPDATE pc_student.Alltraxs_Playlists
                                        SET Title = @Title, Description = @Description, PlaylistImageUrl = @PlaylistImageUrl, Type = @Type, NumSongs=@NumSongs
                                        WHERE Playlist_Id = @Playlist_Id;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(updatequery, myParam);
                    if (rowsAffected != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, could'nt update playlist!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Playlist updated successfully.";
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

        public async Task<responseData> GetPlaylist(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Playlist found successfully!";
            try
            {
                string Playlist_Id = req.addInfo["Playlist_Id"].ToString();
                string Title = req.addInfo["Title"].ToString();
                string Description = req.addInfo["Description"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Playlist_Id", req.addInfo["Playlist_Id"]),
                    new MySqlParameter("@Title", req.addInfo["Title"]),
                    new MySqlParameter("@Description", req.addInfo["Description"])
                };

                string getsql = $"SELECT * FROM pc_student.Alltraxs_Playlists " +
                             "WHERE Playlist_Id = @Playlist_Id OR Title = @Title OR Description = @Description;";
                var playlistdata = ds.ExecuteSQLName(getsql, myParams);
                if (playlistdata == null || playlistdata.Count == 0 || playlistdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Playlist not found!";
                }
                else
                {
                    resData.rData["Playlist_Id"] = playlistdata[0][0]["Playlist_Id"];
                    resData.rData["UserId"] = playlistdata[0][0]["UserId"];
                    resData.rData["Title"] = playlistdata[0][0]["Title"];
                    resData.rData["Description"] = playlistdata[0][0]["Description"];
                    resData.rData["CreatedOn"] = playlistdata[0][0]["CreatedOn"];
                    resData.rData["PlaylistImageUrl"] = playlistdata[0][0]["PlaylistImageUrl"];
                    resData.rData["Type"] = playlistdata[0][0]["Type"];
                    resData.rData["NumSongs"] = playlistdata[0][0]["NumSongs"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct playlist name or description!";
            }
            return resData;
        }

        public async Task<responseData> GetAllPlaylists(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_Playlists ORDER BY Playlist_Id ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all playlists!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> playlists = new List<object>();
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
                                var playlistsdata = new
                                {
                                    Playlist_Id = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    Title = rowData.ElementAtOrDefault(2),
                                    Description = rowData.ElementAtOrDefault(3),
                                    CreatedOn = rowData.ElementAtOrDefault(4),
                                    PlaylistImageUrl = rowData.ElementAtOrDefault(5),
                                    Type = rowData.ElementAtOrDefault(6),
                                    NumSongs = rowData.ElementAtOrDefault(7),
                                };
                                playlists.Add(playlistsdata);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All Playlists found successfully";
                resData.rData["playlistsdata"] = playlists;
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
