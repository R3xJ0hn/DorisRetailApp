using DorisApp.Data.Library.DTO;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using static Dapper.SqlMapper;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerManager _log;

        public FileController(IWebHostEnvironment env, ILoggerManager log)
        {
            _env = env;
            _log = log;
        }
        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;

        [HttpPost("upload-file")]
        public async Task<ActionResult<UploadResultDTO>> UploadFile(IFormFile file)
        {
            var uploadResult = new UploadResultDTO();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            long maxFileSize = 1024 * 1024; // 1 MB in bytes
            var userName = GetUserIdentity().Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "anonymous";
            var firstName = AppHelper.GetFirstWord(userName);


            if (file.Length == 0)
            {
                var msg = $"{untrustedFileName} length is 0";
                _log.LogError($"{firstName} sent" + msg);
                return BadRequest(msg);
            }

            else if (file.Length > maxFileSize)
            {
                var msg = $"{untrustedFileName} of {file.Length} bytes is " +
                    $"larger than the limit of {maxFileSize} bytes";
                _log.LogError($"{firstName} sent" + msg);
                return BadRequest(msg);
            }

            try
            {
                trustedFileNameForFileStorage = Path.GetRandomFileName();

                var tempfile = Path.Combine(_env.ContentRootPath, "uploads/temp");

                if (!Directory.Exists(tempfile))
                {
                    Directory.CreateDirectory(tempfile);
                }

                var path = Path.Combine(tempfile, trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.FileName = untrustedFileName;
                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResult.Uploaded = true;
                return Ok(uploadResult);

            }
            catch (IOException ex)
            {
                _log.LogError($"{uploadResult.FileName} error on upload: {ex.Message}");
                return BadRequest($"{uploadResult.FileName} error on upload.");
            }

        }

        [HttpGet("get/{type}/{filename}")]
        public Stream GetFileStream(string type, string? filename)
        {
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
            var noImg = System.IO.File.OpenRead(
                Path.Combine(_env.ContentRootPath, "assets", "no_image.jpg"));

            if (!string.IsNullOrWhiteSpace(filename))
            {
                var imgPath = Path.Combine(uploadsFolder, type, filename);

                if (System.IO.File.Exists(imgPath)) 
                {
                    return System.IO.File.OpenRead(imgPath);
                }
                else
                {
                    return noImg;
                }
            }

            return noImg;
        }

        [HttpGet("get/{type}/")]
        public Stream NoImage(string type)
        {
            return GetFileStream(type, null);
        }

    }

}
