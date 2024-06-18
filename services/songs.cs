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
                var query = @"SELECT * FROM pc_student.Alltraxs_Songs WHERE name=@name";
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@name", rData.addInfo["name"])
                };
                var dbData = ds.executeSQL(query, myParam);
                if (dbData != null && dbData.Count > 0)
                {
                    resData.rData["rMessage"] = "Playlist with this name already exists.";
                }
                else
                {
                    var insertQuery = @"INSERT INTO pc_student.Alltraxs_Songs(image, name, price) 
                                       VALUES (@image, @name, @price)";
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@image", rData.addInfo["image"]),
                        new MySqlParameter("@name", rData.addInfo["name"]),
                        new MySqlParameter("@price", rData.addInfo["price"]),
                    };

                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);
                    if (rowsAffected > 0)
                    {
                        resData.rData["rMessage"] = "Playlist added successfully.";
                    }
                    else
                    {
                        resData.rData["rMessage"] = "Failed to add playlist.";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }

        public async Task<responseData> DeleteSong(requestData rData)
        {
            responseData resData = new responseData();
            try
            {
                var query = @"DELETE FROM pc_student.Alltraxs_Songs WHERE id = @Id";
                MySqlParameter[] myParam = new MySqlParameter[]
               {
                    new MySqlParameter("@Id", rData.addInfo["id"])
               };

                var song = ds.ExecuteInsertAndGetLastId(query, myParam);
                if (song > 0)
                {
                    resData.rData["rMessage"] = "Song deleted successfully.";
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

        public async Task<responseData> UpdateSong(requestData rData)
        {
            responseData resData = new responseData();
            try
            {
                var query = @"UPDATE pc_student.Alltraxs_Songs
                              SET image = @image, name = @name, price = @price
                              WHERE id = @id";
                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@id", rData.addInfo["id"]),
                    new MySqlParameter("@image", rData.addInfo["image"]),
                    new MySqlParameter("@name", rData.addInfo["name"]),
                    new MySqlParameter("@price", rData.addInfo["price"]),
                };

                int song = ds.ExecuteInsertAndGetLastId(query, myParam);
                if (song != 0)
                {
                    resData.rData["rMessage"] = "No Song found with the provided ID.";
                }
                else
                {
                    resData.rData["rMessage"] = "Song updated successfully.";
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
            resData.rData["rCode"] = 0;
            try
            {
                var list = new ArrayList();
                MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@Id", req.addInfo["Id"]),
                };

                var sq = $"SELECT * FROM pc_student.Alltraxs_Songs WHERE id=@id;";
                var song = ds.ExecuteSQLName(sq, myParams);
                if (song == null)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "No Card is present...";
                }
                else
                {
                    resData.rData["id"] = song[0][0]["id"];
                    resData.rData["image"] = song[0][0]["image"];
                    resData.rData["name"] = song[0][0]["name"];
                    resData.rData["price"] = song[0][0]["price"];
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "Exception occurred: " + ex.Message;
            }
            return resData;
        }
    }
}