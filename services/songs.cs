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
    public class songs
    {
        dbServices ds = new dbServices();

        public async Task<responseData> PostSong(requestData rData)
        {
            responseData resData = new responseData();
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE title=@title";
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@title", rData.addInfo["title"])
                };

                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song with this name already exists.";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Songs(title, artist, album, genre, duration, songUrl, songPic) 
                                       VALUES (@title, @artist, @album, @genre, @duration, @songUrl, @songPic)";
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
                        resData.rData["rMessage"] = "Song uploaded successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to upload song.";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }

        public async Task<responseData> DeleteSong(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", req.addInfo["SongId"].ToString()),
                };

                var deleteSql = $"DELETE FROM pc_student.Alltraxs_Songs WHERE SongId = @SongId";
                var rowsAffected = ds.ExecuteSQLName(deleteSql, para);
                if (rowsAffected[0].Count() != 0)
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Song deleted successfully.";
                }
                else
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No song found with the provided ID.";
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

        public async Task<responseData> UpdateSong(requestData rData)
        {
            responseData resData = new responseData();
            try
            {
                var query = @"UPDATE pc_student.Alltraxs_Songs
                              SET title = @title, artist = @artist, album = @album, genre = @genre, duration = @duration, songUrl = @songUrl, songPic = @songPic
                              WHERE SongId = @SongId";
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", rData.addInfo["SongId"]),
                    new MySqlParameter("@title", rData.addInfo["title"]),
                    new MySqlParameter("@artist", rData.addInfo["artist"]),
                    new MySqlParameter("@album", rData.addInfo["album"]),
                    new MySqlParameter("@genre", rData.addInfo["genre"]),
                    new MySqlParameter("@duration", rData.addInfo["duration"]),
                    new MySqlParameter("@songUrl", rData.addInfo["songUrl"]),
                    new MySqlParameter("@songPic", rData.addInfo["songPic"]),
                };

                int rowsAffected = ds.ExecuteInsertAndGetLastId(query, myParam);
                if (rowsAffected > 0)
                {
                    resData.rData["rMessage"] = "Song updated successfully.";
                }
                else
                {
                    resData.rData["rMessage"] = "No Song found with the provided ID.";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rMessage"] = "Exception occurred: " + ex.Message;
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
                string input = req.addInfo["SongId"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", input)
                };

                var sql = $"SELECT * FROM pc_student.Alltraxs_Songs WHERE SongId=@SongId;";
                var data = ds.ExecuteSQLName(sql, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song not found!";
                }
                else
                {
                    resData.rData["Title"] = data[0][0]["Title"];
                    resData.rData["Artist"] = data[0][0]["Artist"];
                    resData.rData["Album"] = data[0][0]["Album"];
                    resData.rData["Genre"] = data[0][0]["Genre"];
                    resData.rData["Duration"] = data[0][0]["Duration"];
                    resData.rData["SongUrl"] = data[0][0]["SongUrl"];
                    resData.rData["SongPic"] = data[0][0]["SongPic"];
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}
