using DorisApp.Data.Library.DTO;
using DorisApp.WebAPI.DataAccess.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("upload-file")]
        public async Task<ActionResult<UploadResultDTO>> UploadFile(IFormFile file)
        {
            var uploadResult = new UploadResultDTO();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;

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


    }

}
