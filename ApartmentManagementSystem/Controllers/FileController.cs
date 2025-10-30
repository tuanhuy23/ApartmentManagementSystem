using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("{appartmentBuilding}/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        public FileController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ResponseData<ImageDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(file);
            return Ok(new ResponseData<ImageDto>(System.Net.HttpStatusCode.OK, new ImageDto(){ Url = imageUrl}, null, null));
        }
    }
}