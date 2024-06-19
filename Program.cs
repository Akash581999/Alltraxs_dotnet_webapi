using System.IO;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using COMMON_PROJECT_STRUCTURE_API.services;

WebHost.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        services.AddSingleton<login>();
        services.AddSingleton<register>();
        services.AddSingleton<editProfile>();
        services.AddSingleton<changePassword>();
        services.AddSingleton<resetPassword>();
        services.AddSingleton<deleteProfile>();
        services.AddSingleton<contactUs>();
        services.AddSingleton<songs>();
        services.AddSingleton<playlists>();

        services.AddAuthorization();
        services.AddControllers();
        services.AddCors(options => { options.AddPolicy("AllowAnyOrigin", builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); }); });
        services.AddAuthentication("SourceJWT").AddScheme<SourceJwtAuthenticationSchemeOptions, SourceJwtAuthenticationHandler>("SourceJWT", options =>
           {
               options.SecretKey = appsettings["jwt_config:Key"].ToString();
               options.ValidIssuer = appsettings["jwt_config:Issuer"].ToString();
               options.ValidAudience = appsettings["jwt_config:Audience"].ToString();
               options.Subject = appsettings["jwt_config:Subject"].ToString();
           });
    })
    .Configure(app =>
    {
        // app.UseCors(options =>
        //     options.WithOrigins("https://localhost:5002", "http://localhost:5001")
        //     .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseCors("AllowAnyOrigin");
        app.UseRouting();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            var login = endpoints.ServiceProvider.GetRequiredService<login>();
            var register = endpoints.ServiceProvider.GetRequiredService<register>();
            var editProfile = endpoints.ServiceProvider.GetRequiredService<editProfile>();
            var changePassword = endpoints.ServiceProvider.GetRequiredService<changePassword>();
            var resetPassword = endpoints.ServiceProvider.GetService<resetPassword>();
            var deleteProfile = endpoints.ServiceProvider.GetRequiredService<deleteProfile>();
            var contactUs = endpoints.ServiceProvider.GetRequiredService<contactUs>();
            var songs = endpoints.ServiceProvider.GetRequiredService<songs>();
            var playlists = endpoints.ServiceProvider.GetRequiredService<playlists>();

            endpoints.MapPost("/login",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1001") // Read
                    await http.Response.WriteAsJsonAsync(await login.Login(rData));
            }).RequireAuthorization();

            endpoints.MapPost("/register",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1002") // Write
                    await http.Response.WriteAsJsonAsync(await register.Register(rData));
            }).RequireAuthorization();

            endpoints.MapPut("/editProfile",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1003") // Update
                    await http.Response.WriteAsJsonAsync(await editProfile.EditProfile(rData));
            });

            endpoints.MapPut("/changePassword",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1004") // Update
                    await http.Response.WriteAsJsonAsync(await changePassword.ChangePassword(rData));
            });

            endpoints.MapPut("/resetPassword",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1005") // Update
                    await http.Response.WriteAsJsonAsync(await resetPassword.ResetPassword(rData));
            });

            endpoints.MapDelete("/deleteProfile",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1006") // Delete
                    await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfile(rData));
            });

            endpoints.MapPost("contactUs",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1007") // Contact us
                    await http.Response.WriteAsJsonAsync(await contactUs.ContactUs(rData));
            });

            //Endponits for songs
            endpoints.MapPost("/songs",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1008") // Post song
                    await http.Response.WriteAsJsonAsync(await songs.PostSong(rData));
            });
            endpoints.MapDelete("/songs/id",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1009") // Delete song
                    await http.Response.WriteAsJsonAsync(await songs.DeleteSong(rData));
            });
            endpoints.MapPut("/songs/id",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1010") // Update song
                    await http.Response.WriteAsJsonAsync(await songs.UpdateSong(rData));
            });
            endpoints.MapGet("/songs/id",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1011") // Get song
                    await http.Response.WriteAsJsonAsync(await songs.GetSong(rData));
            });


            //Endponits for playlists
            endpoints.MapPost("/playlists",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1012") // Create playlist
                    await http.Response.WriteAsJsonAsync(await playlists.CreatePlaylist(rData));
            });
            endpoints.MapDelete("/playlists/id",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1013") // Delete playlist
                    await http.Response.WriteAsJsonAsync(await playlists.DeletePlaylist(rData));
            });
            endpoints.MapPut("/playlists/id",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1014") // Update playlist
                    await http.Response.WriteAsJsonAsync(await playlists.UpdatePlaylist(rData));
            });
            endpoints.MapGet("/playlists/id",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1015") // Get playlist
                    await http.Response.WriteAsJsonAsync(await playlists.GetPlaylist(rData));
            });

            endpoints.MapGet("/bing",
                 async c => await c.Response.WriteAsJsonAsync("{'Name':'Akash','Age':'24','Project':'AllTraxs_Music_Webapp'}"));
        });
    }).Build().Run();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();

public record requestData
{
    [Required]
    public string eventID { get; set; }
    [Required]
    public IDictionary<string, object> addInfo { get; set; }
}

public record responseData
{
    public responseData()
    {
        eventID = "";
        rStatus = 0;
        rData = new Dictionary<string, object>();
    }
    [Required]
    public int rStatus { get; set; } = 0;
    public string eventID { get; set; }
    public IDictionary<string, object> addInfo { get; set; }
    public IDictionary<string, object> rData { get; set; }
}
