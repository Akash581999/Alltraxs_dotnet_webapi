using System.Text;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class playlistSongs
    {
        dbServices ds = new dbServices();
        public async Task<responseData> AddToPlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                // string UserId = rData.addInfo["UserId"].ToString();
                // string SongId = rData.addInfo["SongId"].ToString();
                // string Playlist_Id = rData.addInfo["Playlist_Id"].ToString();

                MySqlParameter[] insertParams = new MySqlParameter[]
                {
                    // new MySqlParameter("@UserId", UserId.ToString()),
                    // new MySqlParameter("@SongId", SongId.ToString()),
                    // new MySqlParameter("@Playlist_Id", Playlist_Id.ToString()),
                    new MySqlParameter("@Title", rData.addInfo["Title"].ToString()),
                    new MySqlParameter("@Artist", rData.addInfo["Artist"].ToString()),
                    new MySqlParameter("@Album", rData.addInfo["Album"].ToString()),
                    new MySqlParameter("@Genre", rData.addInfo["Genre"].ToString()),
                    new MySqlParameter("@Duration", rData.addInfo["Duration"].ToString()),
                    new MySqlParameter("@SongUrl", rData.addInfo["SongUrl"].ToString()),
                    new MySqlParameter("@SongPic", rData.addInfo["SongPic"].ToString()),
                };
                var query = @"SELECT * FROM pc_student.Alltraxs_PlaylistSongs WHERE Title=@Title;";
                var dbData = ds.ExecuteSQLName(query, insertParams);
                if (dbData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song already added in playlist!";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_PlaylistSongs(Title, Artist, Album, Genre, Duration, SongUrl, SongPic)
                                    VALUES (@Title, @Artist, @Album, @Genre, @Duration, @SongUrl, @SongPic);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);
                    if (rowsAffected == 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add song in playlist!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Song added to playlist.";
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


        public async Task<responseData> RemoveFromPlaylist(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Id", rData.addInfo["Id"].ToString()),
                    new MySqlParameter("@Title", rData.addInfo["Title"].ToString())
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_PlaylistSongs WHERE Id=@Id AND Title=@Title;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song not found!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.Alltraxs_PlaylistSongs WHERE Id=@Id AND Title = @Title";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Failed to remove song from playlist!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Song removed from playlist.";
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


        public async Task<responseData> GetSongFromPlaylist(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Song found successfully!";
            try
            {
                string Id = req.addInfo["Id"].ToString();
                string Title = req.addInfo["Title"].ToString();
                string Artist = req.addInfo["Artist"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Id", req.addInfo["Id"]),
                    new MySqlParameter("@Title", req.addInfo["Title"]),
                    new MySqlParameter("@Artist", req.addInfo["Artist"])
                };

                string getsql = $"SELECT * FROM pc_student.Alltraxs_PlaylistSongs " +
                             "WHERE Id=@Id OR Title = @Title OR Artist = @Artist;";
                var songdata = ds.ExecuteSQLName(getsql, myParams);
                if (songdata == null || songdata.Count == 0 || songdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song not found!";
                }
                else
                {
                    var songData = songdata[0][0];
                    resData.rData["Id"] = songData["Id"];
                    resData.rData["Title"] = songData["Title"];
                    resData.rData["Artist"] = songData["Artist"];
                    resData.rData["Album"] = songData["Album"];
                    resData.rData["Genre"] = songData["Genre"];
                    resData.rData["Duration"] = songData["Duration"];
                    resData.rData["SongUrl"] = songData["SongUrl"];
                    resData.rData["SongPic"] = songData["SongPic"];
                    resData.rData["UserId"] = songData["UserId"];
                    resData.rData["SongId"] = songData["SongId"];
                    resData.rData["Playlist_Id"] = songData["Playlist_Id"];
                    // resData.rData["Popularity"] = songData["Popularity"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Some exception occurred, cant get song!";
            }
            return resData;
        }

        public async Task<responseData> GetAllPlaylistSongs(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_PlaylistSongs ORDER BY Id ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all playlist songs!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> playlistsongs = new List<object>();
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
                                var playlistsongsdata = new
                                {
                                    Id = rowData.ElementAtOrDefault(0),
                                    Title = rowData.ElementAtOrDefault(1),
                                    Artist = rowData.ElementAtOrDefault(2),
                                    Album = rowData.ElementAtOrDefault(3),
                                    Genre = rowData.ElementAtOrDefault(4),
                                    Duration = rowData.ElementAtOrDefault(5),
                                    SongUrl = rowData.ElementAtOrDefault(6),
                                    SongPic = rowData.ElementAtOrDefault(7),
                                    UserId = rowData.ElementAtOrDefault(8),
                                    SongId = rowData.ElementAtOrDefault(9),
                                    Playlist_Id = rowData.ElementAtOrDefault(10),
                                };
                                playlistsongs.Add(playlistsongsdata);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All Playlist songs found successfully";
                resData.rData["playlistsongsdata"] = playlistsongs;
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