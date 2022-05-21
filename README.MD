# Powers.DbBackup

About:

This lib can backup a database to a sql script. And it can match the table names.

Support:

Now it only supports the SqlServer.

## How to Use

### Configure Posers.DbBackup

#### 1. In `Startup.cs`(Net5):

```csharp
services.AddDbBackup();
```

`appsetting.json`:
```jsonc
"DbBackupOptions": {
    // remote server
    "ServerInstance": "192.168.31.36",
    // database username
    "Username": "sa",
    // password
    "Password": "sa123.",
    // ddatabase name
    "DatabaseName": "PumInfoShop",
    // output options
    "ScriptingOptions": {
      "DriAll": false,
      "ScriptSchema": true,
      "ScriptData": true,
      "ScriptDrops": false
    },
    // match rules
    /**
     * Include 3 rules:
     * 1. Full name: UserTable
     * 2. Start with: Sys*
     * 3. End with: *Table
     */
    "FormatTables": []
  }
```
**OR**
```csharp
services.AddDbBackup(opts =>
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
//services.AddDbBackup(opts => new DbBackupOptions
//{
//    ServerInstance = "127.0.0.1",
//    Username = "sa",
//    // .....
//});
```

#### 2. In `Program.cs`(Net6):

```csharp
builder.Services.AddDbBackup();
```

`appsetting.json`:
```json
"DbBackupOptions": {
    "ServerInstance": "192.168.31.36",
    "Username": "sa",
    "Password": "sa123.",
    "DatabaseName": "PumInfoShop",
    "ScriptingOptions": {
      "DriAll": false,
      "ScriptSchema": true,
      "ScriptData": true,
      "ScriptDrops": false
    },
    "FormatTables": []
  }
```
**OR**
```csharp
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
```

### Use Powers.DbBackup

```csharp
[HttpGet]
public async Task<ActionResult> StartDbBackup()
{
    var rootPath = "D:/";
    var fileName = DateTime.Now.ToString("yyyyMMddhhmmss"); // No ".sql" suffix is required.
    var (path, size) = await DbBackupExtensions.StartBackupAsync(rootPath, fileName);// path is full path

    return Ok(new
    {
        Path = path,
        Size = size
    });
}

[HttpGet]
public async Task<ActionResult> DeleteDbBackup(string filePath)
{
    var (res, msg) = await DbBackupExtensions.DeleteBackup(filePath);

    if (res)
    {
        return Ok(msg);
    }
    else
    {
        return NotFound(msg);
    }
}
```
