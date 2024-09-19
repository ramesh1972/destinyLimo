using DestinyLimoServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper; // Add this line to import the IMapper interface
using DestinyLimoServer.DTOs.RequestDTOs;
using DestinyLimoServer.Models;
using DestinyLimoServer.DTOs.ResponseDTOs;
using DestinyLimoServer.Common.Repository;
using DestinyLimoServer.Repositories.impl;
using DestinyLimoServer.Common.DB;
using DestinyLimoServer.Common.Uploader;

namespace DestinyLimoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController(DapperContext dapper, IMapper mapper, ILogger logger, IConfiguration configuration) : ControllerBase
    {
        private readonly DapperContext _dapper = dapper;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;
        private readonly IConfiguration _configuration = configuration;


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

            var materials = await _repository.GetAllMaterials(isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<MaterialFileDTO>>(materials);

            FileUploader fileUploader = new FileUploader(_logger, _configuration);

            // copy the material_id to id field
            foreach (var materialDTO in materialDTOs)
            {
                materialDTO.id = materialDTO.material_id;
                materialDTO.file_name = fileUploader.GetFilePathName(UploadFilePaths.TrainingMaterial, materialDTO.file_name!);
            }

            return Ok(materialDTOs);
        }

        [HttpGet("text/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetTextMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialText> _repository = new MaterialRepository<MaterialText>(_dapper);

            var materials = await _repository.GetAllMaterials(isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<DestinyLimoServer.DTOs.ResponseDTOs.MaterialTextDTO>>(materials);

            // copy the material_id to id field
            foreach (var materialDTO in materialDTOs)
            {
                materialDTO.id = materialDTO.material_id;
            }

            return Ok(materialDTOs);
        }

        [HttpGet("video/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetVideoMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialVideo> _repository = new MaterialRepository<MaterialVideo>(_dapper);

            var materials = await _repository.GetAllMaterials(isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<MaterialVideoDTO>>(materials);

            FileUploader fileUploader = new FileUploader(_logger, _configuration);

            // copy the material_id to id field
            foreach (var materialDTO in materialDTOs)
            {
                materialDTO.id = materialDTO.material_id;

                if (materialDTO.url != null && materialDTO.url != "" && materialDTO.url.Contains("http") == false)
                    materialDTO.url = fileUploader.GetFilePathName(UploadFilePaths.TrainingMaterial, materialDTO.url!);
            }

            return Ok(materialDTOs);
        }

        [HttpGet("mcq/{includeInActive?}/{includeDeleted?}")]
        public async Task<IActionResult> GetMCQMaterialsAsync(bool? includeInActive = true, bool? includeDeleted = false, bool? isPublic = false)
        {
            IMaterialRepository<MaterialMCQ> _repository = new MaterialRepository<MaterialMCQ>(_dapper);

            var materials = await _repository.GetAllMaterials(isPublic ?? false, includeInActive ?? true, includeDeleted ?? false);

            System.Console.WriteLine("materials: " + materials);

            var materialDTOs = _mapper.Map<IEnumerable<DestinyLimoServer.DTOs.ResponseDTOs.MaterialMCQDTO>>(materials);

            // copy the material_id to id field
            foreach (var materialDTO in materialDTOs)
            {
                materialDTO.id = materialDTO.material_id;
            }

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

        // CUD operations on MaterialText
        [HttpPost("text")]
        public async Task<IActionResult> AddMaterialTextAsync([FromBody] DestinyLimoServer.DTOs.RequestDTOs.MaterialTextDTO materialTextDTO)
        {
            var materialText = _mapper.Map<MaterialText>(materialTextDTO);
            materialText.material_type_id = 5;

            // add to material table first
            IBaseRepository<Material> repoMat = new MaterialRepository<Material>(_dapper);
            int newMaterialId = await repoMat.AddAsync(materialText as Material);

            if (newMaterialId == -1)
            {
                return BadRequest("Failed to add material");
            }

            materialText.material_id = newMaterialId;
            IBaseRepository<MaterialText> repoMatText = new BaseRepository<MaterialText>(_dapper, "training_material_text", "text_id");
            var result = await repoMatText.AddAsync(materialText, ["material_id", "text"]);

            if (result == -1)
            {
                return BadRequest("Failed to add material text");
            }

            // get the whole material
            IMaterialRepository<MaterialText> repoMat2 = new MaterialRepository<MaterialText>(_dapper);
            var newMaterial = await repoMat2.GetMaterialById(newMaterialId);

            var materialDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.MaterialTextDTO>(newMaterial);

            return Ok(materialDTO);
        }

        // update material text
        [HttpPut("text")]
        public async Task<IActionResult> UpdateMaterialTextAsync([FromBody] DestinyLimoServer.DTOs.RequestDTOs.MaterialTextDTO materialTextDTO)
        {
            var materialText = _mapper.Map<MaterialText>(materialTextDTO);

            // update material table first
            IBaseRepository<Material> repoMat = new MaterialRepository<Material>(_dapper);
            var result = await repoMat.UpdateAsync(materialText as Material, materialText.material_id);

            if (result == false)
            {
                return BadRequest("Failed to update material");
            }

            IBaseRepository<MaterialText> repoMatText = new BaseRepository<MaterialText>(_dapper, "training_material_text", "text_id");
            result = await repoMatText.UpdateAsync(materialText, materialText.text_id, ["material_id", "text"]);

            if (result == false)
            {
                return BadRequest("Failed to update material text");
            }

            // get the whole material
            IMaterialRepository<MaterialText> repoMat2 = new MaterialRepository<MaterialText>(_dapper);
            var newMaterial = await repoMat2.GetMaterialById(materialText.material_id);

            var materialDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.MaterialTextDTO>(newMaterial);

            return Ok(materialDTO);
        }

        // detele material text
        [HttpDelete("text/{materialId}")]
        public async Task<IActionResult> DeleteMaterialTextAsync(int materialId)
        {
            IBaseRepository<Material> repoMat = new MaterialRepository<Material>(_dapper);
            var result = await repoMat.DeleteSoftAsync(materialId);

            if (result == false)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpPost("mcq")]
        public async Task<IActionResult> AddMaterialMCQAsync([FromBody] DestinyLimoServer.DTOs.RequestDTOs.MaterialMCQDTO materialMCQDTO)
        {
            var materialMCQ = _mapper.Map<MaterialMCQ>(materialMCQDTO);
            materialMCQ.material_type_id = 4;

            // add to material table first
            IBaseRepository<Material> repoMat = new MaterialRepository<Material>(_dapper);
            int newMaterialId = await repoMat.AddAsync(materialMCQ as Material);

            if (newMaterialId == -1)
            {
                return BadRequest("Failed to add material");
            }

            materialMCQ.material_id = newMaterialId;
            IBaseRepository<MaterialMCQ> repoMatMCQ = new BaseRepository<MaterialMCQ>(_dapper, "training_material_mcqs", "question_id");
            var fieldNames = new string[] { "material_id", "question_text", "answer_1", "answer_2", "answer_3", "answer_4", "correct_1", "correct_2", "correct_3", "correct_4" };
            var result = await repoMatMCQ.AddAsync(materialMCQ, fieldNames);

            if (result == -1)
            {
                return BadRequest("Failed to add material mcq");
            }

            // get the whole material
            IMaterialRepository<MaterialMCQ> repoMat2 = new MaterialRepository<MaterialMCQ>(_dapper);
            var newMaterial = await repoMat2.GetMaterialById(newMaterialId);

            var materialDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.MaterialMCQDTO>(newMaterial);

            return Ok(materialDTO);
        }

        // update material mcq
        [HttpPut("mcq")]
        public async Task<IActionResult> UpdateMaterialMCQAsync([FromBody] DestinyLimoServer.DTOs.RequestDTOs.MaterialMCQDTO materialMCQDTO)
        {
            var materialMCQ = _mapper.Map<MaterialMCQ>(materialMCQDTO);

            // update material table first
            IBaseRepository<Material> repoMat = new MaterialRepository<Material>(_dapper);
            var result = await repoMat.UpdateAsync(materialMCQ as Material, materialMCQ.material_id);

            if (result == false)
            {
                return BadRequest("Failed to update material");
            }

            IBaseRepository<MaterialMCQ> repoMatMCQ = new BaseRepository<MaterialMCQ>(_dapper, "training_material_mcqs", "question_id");
            var fieldNames = new string[] { "material_id", "question_id", "question_text", "answer_1", "answer_2", "answer_3", "answer_4", "correct_1", "correct_2", "correct_3", "correct_4" };
            result = await repoMatMCQ.UpdateAsync(materialMCQ, materialMCQ.question_id, fieldNames);

            if (result == false)
            {
                return BadRequest("Failed to update material mcq");
            }

            // get the whole material
            IMaterialRepository<MaterialMCQ> repoMat2 = new MaterialRepository<MaterialMCQ>(_dapper);
            var newMaterial = await repoMat2.GetMaterialById(materialMCQ.material_id);

            var materialDTO = _mapper.Map<DestinyLimoServer.DTOs.ResponseDTOs.MaterialMCQDTO>(newMaterial);

            return Ok(materialDTO);
        }

        // detele material mcq
        [HttpDelete("mcq/{materialId}")]
        public async Task<IActionResult> DeleteMaterialMCQAsync(int materialId)
        {
            IBaseRepository<Material> repoMat = new MaterialRepository<Material>(_dapper);
            var result = await repoMat.DeleteSoftAsync(materialId);

            if (result == false)
            {
                return Ok(false);
            }

            return Ok(true);
        }

    }
}


