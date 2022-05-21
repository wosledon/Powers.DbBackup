using Microsoft.SqlServer.Management.Smo;
using Powers.DbBackup.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbBackup(opts =>
{
    opts.ServerInstance = "127.0.0.1";
    opts.Username = "sa";
    opts.Password = "123456";
    opts.DatabaseName = "TestDb";
    opts.ScriptingOptions = new ScriptingOptions
    {
        DriAll = true,
        ScriptSchema = true,
        ScriptData = true,
        ScriptDrops = false
    };
    /**
     * Include 3 rules:
     * 1. Full name: UserTable
     * 2. Start with: Sys*
     * 3. End with: *Table
     */
    opts.FormatTables = new string[] { "Sys*", "Log*", "UserTable", "*Table" };
});

// Or this way
//builder.Services.AddDbBackup(opts => new DbBackupOptions
//{
//    ServerInstance = "127.0.0.1",
//    Username = "sa",
//    // .....
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();