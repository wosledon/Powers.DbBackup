using Microsoft.AspNetCore.Mvc;

namespace Powers.DbBackup.SqlServer.Sample.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class DbBackupController : ControllerBase
    {
        public DbBackupController()
        {
        }

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
    }
}