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
    public class songs
    {
        dbServices ds = new dbServices();
        public async Task<responseData> PostSong(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@title", rData.addInfo["title"])
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE title=@title;";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song with this name already exists!";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Songs(title, artist, album, genre, duration, songUrl, songPic) 
                                       VALUES (@title, @artist, @album, @genre, @duration, @songUrl, @songPic);";
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@title", rData.addInfo["title"]),
                        new MySqlParameter("@artist", rData.addInfo["artist"]),
                        new MySqlParameter("@album", rData.addInfo["album"]),
                        new MySqlParameter("@genre", rData.addInfo["genre"]),
                        new MySqlParameter("@duration", rData.addInfo["duration"]),
                        new MySqlParameter("@songUrl", rData.addInfo["songUrl"]),
                        new MySqlParameter("@songPic", rData.addInfo["songPic"]),
                    };

                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);
                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Song added successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add song!";
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

        public async Task<responseData> DeleteSong(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", rData.addInfo["SongId"].ToString()),
                    new MySqlParameter("@title", rData.addInfo["title"].ToString())
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE SongId=@SongId AND title=@title;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No song found!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.Alltraxs_Songs WHERE SongId = @SongId";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Song couldn't deleted!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Song deleted successfully.";
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

        public async Task<responseData> UpdateSong(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", rData.addInfo["SongId"]),
                    new MySqlParameter("@title", rData.addInfo["title"]),
                    new MySqlParameter("@artist", rData.addInfo["artist"]),
                    new MySqlParameter("@album", rData.addInfo["album"]),
                    new MySqlParameter("@genre", rData.addInfo["genre"]),
                    new MySqlParameter("@duration", rData.addInfo["duration"]),
                    new MySqlParameter("@songUrl", rData.addInfo["songUrl"]),
                    new MySqlParameter("@songPic", rData.addInfo["songPic"]),
                    new MySqlParameter("@popularity", rData.addInfo["popularity"]),
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE SongId=@SongId";
                var dbData = ds.ExecuteSQLName(query, myParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No Song found with the provided Id!";
                }
                else
                {
                    var updatequery = @"UPDATE pc_student.Alltraxs_Songs
                              SET title = @title, artist = @artist, album = @album, genre = @genre, duration = @duration, songUrl = @songUrl, songPic = @songPic, popularity=@popularity
                              WHERE SongId = @SongId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, myParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, couldn't update details!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Song details updated successfully.";
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

        public async Task<responseData> GetSong(requestData req)
        {
            responseData resData = new responseData();
            resData.eventID = req.eventID;
            resData.rData["rCode"] = 0;
            resData.rData["rMessage"] = "Song found successfully!";
            try
            {
                string SongId = req.addInfo["SongId"].ToString();
                string Title = req.addInfo["Title"].ToString();
                string Artist = req.addInfo["Artist"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new("@SongId", MySqlDbType.VarChar) { Value = SongId },
                    new("@Title", MySqlDbType.VarChar) { Value = Title },
                    new("@Artist", MySqlDbType.VarChar) { Value = Artist }
                };

                string sql = $"SELECT * FROM pc_student.Alltraxs_Songs " +
                             "WHERE SongId = @SongId OR Title = @Title OR Artist = @Artist;";
                var data = ds.ExecuteSQLName(sql, myParams);
                if (data == null || data.Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song not found!";
                }
                else
                {
                    var songData = data[0][0];
                    resData.rData["SongId"] = songData["SongId"];
                    resData.rData["Title"] = songData["Title"];
                    resData.rData["Artist"] = songData["Artist"];
                    resData.rData["Album"] = songData["Album"];
                    resData.rData["Genre"] = songData["Genre"];
                    resData.rData["Duration"] = songData["Duration"];
                    resData.rData["SongUrl"] = songData["SongUrl"];
                    resData.rData["SongPic"] = songData["SongPic"];
                    resData.rData["Popularity"] = songData["Popularity"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct song or artist name!";
            }
            return resData;
        }
    }
}
