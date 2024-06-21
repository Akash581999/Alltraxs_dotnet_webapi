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
                string UserId = rData.addInfo["UserId"].ToString();
                string SongId = rData.addInfo["SongId"].ToString();
                string Playlist_Id = rData.addInfo["Playlist_Id"].ToString();

                MySqlParameter[] insertParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", UserId.ToString()),
                    new MySqlParameter("@SongId", SongId.ToString()),
                    new MySqlParameter("@Playlist_Id", Playlist_Id.ToString()),
                    new MySqlParameter("@Title", rData.addInfo["Title"].ToString()),
                    new MySqlParameter("@Artist", rData.addInfo["Artist"].ToString()),
                    new MySqlParameter("@Album", rData.addInfo["Album"].ToString()),
                    new MySqlParameter("@Genre", rData.addInfo["Genre"].ToString()),
                    new MySqlParameter("@Duration", rData.addInfo["Duration"].ToString()),
                    new MySqlParameter("@SongUrl", rData.addInfo["SongUrl"].ToString()),
                    new MySqlParameter("@SongPic", rData.addInfo["SongPic"].ToString()),
                };

                var insertQuery = @"INSERT INTO pc_student.Alltraxs_PlaylistSongs(Title, Artist, Album, Genre, Duration, SongUrl, SongPic, UserId, SongId, Playlist_Id)
                                    VALUES (@Title, @Artist, @Album, @Genre, @Duration, @SongUrl, @SongPic, @UserId, @SongId, @Playlist_Id);";

                int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);
                if (rowsAffected > 0)
                {
                    resData.eventID = rData.eventID;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Song added to playlist.";
                }
                else
                {
                    resData.rData["rCode"] = 3;
                    resData.rData["rMessage"] = "Failed to add song in playlist!";
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
                        resData.rData["rMessage"] = "Song remove from playlist.";
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