using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    public class UploadController : Controller
    {
        private UploadServices _uploadServices;

        public UploadController(UploadServices uploadServices)
        {
            _uploadServices = uploadServices;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadToBlobStorage()
        {
            Guid fileGuid;
            var files = Request.Form.Files;

            if (files == null || files.Count < 0)
            {
                return BadRequest(new ErrorMessageModel("No file is uploaded"));
            }
            using (var fileStream = files[0].OpenReadStream())
            {
                fileGuid = await _uploadServices.UploadBlobAsync(fileStream, files[0].ContentType);
                return Ok(new
                {
                    FileGuid = fileGuid
                });
            }
        }
    }
}
