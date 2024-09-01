using Dapper;
using DestinyLimoServer.Common.DB;
using DestinyLimoServer.Common.Repository;
using DestinyLimoServer.Models;
using Org.BouncyCastle.Security;

namespace DestinyLimoServer.Repositories.impl
{
    public class MaterialRepository<T>(DapperContext dapperContext) : BaseRepository<MaterialCategory>(dapperContext, "training_material", "material_id"), IMaterialRepository<T> where T : class
    {
        private DapperContext? _dapperContext = dapperContext;

        private string _getTableName()
        {
            Type type = typeof(T);

            if (type == typeof(MaterialText))
            {
                return "training_material_text";
            }
            else if (type == typeof(MaterialVideo))
            {
                return "training_material_videos";
            }
            else if (type == typeof(MaterialFile))
            {
                return "training_material_files";
            }
            else if (type == typeof(MaterialMCQ))
            {
                return "training_material_mcqs";
            }
            else if (type == typeof(Material))
            {
                return "training_material";
            }
            else
            {
                throw new Exception("Invalid Material Type");
            }
        }

        public async Task<IEnumerable<T>> GetAllMaterials(bool is_public, bool inactive = true, bool deleted = false)
        {

            System.Console.WriteLine("is public " + is_public);

            using var connection = CreateConnection();
            var sql = $"SELECT * FROM {_getTableName()} " +
                      $"INNER JOIN training_material ON training_material.material_id = {_getTableName()}.material_id ";

            sql += (is_public == true) ? "WHERE is_public = 1" : "WHERE is_public in (1,0)";

            if (!inactive)
            {
                sql += " AND is_active = 1";
            }

            if (!deleted)
            {
                sql += " AND is_deleted = 0";
            }

            System.Console.WriteLine("MATERIAL sql: " + sql);

            return await connection.QueryAsync<T>(sql);
        }

        public Task<T> GetMaterialById(int MaterialId)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetMaterialByMaterialName(string Materialname)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetRandomQuestions(int numQuestions)
        {
            var sql = $"SELECT * FROM {_getTableName()} " +
                      $"INNER JOIN training_material ON training_material.material_id = {_getTableName()}.material_id " +
                      $"WHERE is_active = 1 AND is_deleted = 0 " + 
                      "ORDER BY RAND() LIMIT @numQuestions";
            
            var connection = CreateConnection();
            
            return await connection.QueryAsync<T>(sql, new { numQuestions = numQuestions });
        }
    }
}