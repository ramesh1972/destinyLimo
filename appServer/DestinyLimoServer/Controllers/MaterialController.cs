using DestinyLimoServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimoServer.DTOs.RequestDTOs;
using DestinyLimoServer.Models;
using DestinyLimoServer.DTOs.ResponseDTOs;
using DestinyLimoServer.Repositories.impl;
using DestinyLimoServer.Common.DB;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController(DapperContext dapper, IMapper mapper, ILogger logger) : ControllerBase
    {
        private readonly DapperContext _dapper = dapper;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;


        [HttpGet("material/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            System.Console.WriteLine("is public : controller", isPublic);
            IMaterialRepository<Material> _repository = new MaterialRepository<Material>(_dapper);

            var materials = await _repository.GetAllMaterials(isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<MaterialFileDTO>>(materials);

            return Ok(materialDTOs);
        }

        [HttpGet("file/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetFileMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialFile> _repository = new MaterialRepository<MaterialFile>(_dapper);

            var materials = await _repository.GetAllMaterials( isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<MaterialFileDTO>>(materials);

            return Ok(materialDTOs);
        }

        [HttpGet("text/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetTextMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialText> _repository = new MaterialRepository<MaterialText>(_dapper);

            var materials = await _repository.GetAllMaterials( isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<MaterialTextDTO>>(materials);

            return Ok(materialDTOs);
        }

        [HttpGet("video/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetVideoMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialVideo> _repository = new MaterialRepository<MaterialVideo>(_dapper);

            var materials = await _repository.GetAllMaterials( isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);


            var materialDTOs = _mapper.Map<IEnumerable<MaterialVideoDTO>>(materials);


            return Ok(materialDTOs);
        }

        [HttpGet("mcq/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetMCQMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialMCQ> _repository = new MaterialRepository<MaterialMCQ>(_dapper);

            var materials = await _repository.GetAllMaterials( isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<MaterialMCQDTO>>(materials);

            return Ok(materialDTOs);
        }

        private async Task<IEnumerable<MaterialCategoryDTO>> _getMaterialCategoriesAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            IMaterialCategoryRepository _repository = new MaterialCategoryRepository(_dapper);

            var categories = await _repository.GetAllMaterialCategories(includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + categories);

            var categoryDTOs = _mapper.Map<IEnumerable<MaterialCategoryDTO>>(categories);
            return categoryDTOs;
        }

        [HttpGet("category/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetMaterialCategoryAsync(bool? includeInActive = true, bool? includeDeleted = false)
        {
            var categoryDTOs = await _getMaterialCategoriesAsync(includeInActive, includeDeleted);
            
            return Ok(categoryDTOs);
        }

    }
}


