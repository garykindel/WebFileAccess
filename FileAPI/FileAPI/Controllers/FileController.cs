namespace FileAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using System.Net.Mime;
    using Microsoft.AspNetCore.StaticFiles;

    namespace YourNamespace.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class FileController : ControllerBase
        {
            [HttpGet]
            public IActionResult GetFile([FromQuery] string filePath)
            {  try
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
                }
                catch (FileNotFoundException)
                {
                    return NotFound("File not found");
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid("Access to the file is forbidden");
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
                }
            }

            private string GetContentType(string path)
            {
                var types = new FileExtensionContentTypeProvider();
                var ext = Path.GetExtension(path).ToLowerInvariant();
                string contentType;
                if (!types.TryGetContentType(path, out contentType))
                {
                    contentType = MediaTypeNames.Application.Octet;
                }
                return contentType;
            }
        }
    }
}
