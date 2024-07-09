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
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@title", rData.addInfo["title"]),
                };

                var checkQuery = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE title = @title;";
                var dbCheckData = ds.ExecuteSQLName(checkQuery, checkParams);
                if (dbCheckData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song with this title already exists!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@title", rData.addInfo["title"]),
                        new MySqlParameter("@artist", rData.addInfo["artist"]),
                        new MySqlParameter("@album", rData.addInfo["album"]),
                        new MySqlParameter("@genre", rData.addInfo["genre"]),
                        new MySqlParameter("@duration", rData.addInfo["duration"]),
                        new MySqlParameter("@popularity", rData.addInfo["popularity"]),
                        new MySqlParameter("@songUrl", rData.addInfo["songUrl"]),
                        new MySqlParameter("@songPic", rData.addInfo["songPic"]),
                    };
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Songs (title, artist, album, genre, duration, popularity, songUrl, songPic) 
                                        VALUES (@title, @artist, @album, @genre, @duration, @popularity, @songUrl, @songPic);";
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
                    // new MySqlParameter("@SongId", rData.addInfo["SongId"].ToString()),
                    new MySqlParameter("@title", rData.addInfo["title"].ToString())
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE title=@title;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No song found!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.Alltraxs_Songs WHERE title = @title";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Song deleted successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Song couldn't deleted!";
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
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", rData.addInfo["SongId"]),
                };

                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE SongId=@SongId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No Song found with the provided Id!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@SongId", rData.addInfo["SongId"]),
                        new MySqlParameter("@title", rData.addInfo["title"]),
                        new MySqlParameter("@artist", rData.addInfo["artist"]),
                        new MySqlParameter("@album", rData.addInfo["album"]),
                        new MySqlParameter("@genre", rData.addInfo["genre"]),
                        new MySqlParameter("@duration", rData.addInfo["duration"]),
                        new MySqlParameter("@popularity", rData.addInfo["popularity"]),
                        new MySqlParameter("@songUrl", rData.addInfo["songUrl"]),
                        new MySqlParameter("@songPic", rData.addInfo["songPic"]),
                   };
                    var updatequery = @"UPDATE pc_student.Alltraxs_Songs
                                        SET title = @title, artist = @artist, album = @album, genre = @genre, duration = @duration, popularity=@popularity, songUrl = @songUrl, songPic = @songPic
                                        WHERE SongId = @SongId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
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
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Song found successfully!";
            try
            {
                string SongId = req.addInfo["SongId"].ToString();
                string Title = req.addInfo["Title"].ToString();
                string Artist = req.addInfo["Artist"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@SongId", req.addInfo["SongId"]),
                    new MySqlParameter("@Title", req.addInfo["Title"]),
                    new MySqlParameter("@Artist", req.addInfo["Artist"])
                };

                string getsql = $"SELECT * FROM pc_student.Alltraxs_Songs " +
                             "WHERE SongId = @SongId OR Title = @Title OR Artist = @Artist;";
                var songdata = ds.ExecuteSQLName(getsql, myParams);
                if (songdata == null || songdata.Count == 0 || songdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Song not found!";
                }
                else
                {
                    var songData = songdata[0][0];
                    resData.rData["SongId"] = songData["SongId"];
                    resData.rData["Title"] = songData["Title"];
                    resData.rData["Artist"] = songData["Artist"];
                    resData.rData["Album"] = songData["Album"];
                    resData.rData["Genre"] = songData["Genre"];
                    resData.rData["Duration"] = songData["Duration"];
                    resData.rData["Popularity"] = songData["Popularity"];
                    resData.rData["SongUrl"] = songData["SongUrl"];
                    resData.rData["SongPic"] = songData["SongPic"];
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

        public async Task<responseData> GetAllSongs(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.Alltraxs_Songs ORDER BY SongId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all songs!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> songslist = new List<object>();
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
                                var songs = new
                                {
                                    SongId = rowData.ElementAtOrDefault(0),
                                    Title = rowData.ElementAtOrDefault(1),
                                    Artist = rowData.ElementAtOrDefault(2),
                                    Album = rowData.ElementAtOrDefault(3),
                                    Genre = rowData.ElementAtOrDefault(4),
                                    Duration = rowData.ElementAtOrDefault(5),
                                    SongUrl = rowData.ElementAtOrDefault(6),
                                    SongPic = rowData.ElementAtOrDefault(7),
                                    Popularity = rowData.ElementAtOrDefault(8),
                                    UploadOn = rowData.ElementAtOrDefault(9)
                                };
                                songslist.Add(songs);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All Songs found successfully";
                resData.rData["songs"] = songslist;
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
