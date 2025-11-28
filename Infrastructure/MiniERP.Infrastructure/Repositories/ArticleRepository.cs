using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniERP.ApplicationLayer.Interfaces;
using MiniERP.Domain;
using MiniERP.Infrastructure.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MiniERP.Infrastructure.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Article>> GetAllAsync() =>
            await ExecuteArticleQueryAsync("SELECT * FROM Articles");

        public override async Task<Article?> GetByIdAsync(int id)
        {
            var result = await ExecuteArticleQueryAsync(
                "SELECT * FROM Articles WHERE Id = @id LIMIT 1",
                new SqliteParameter("@id", id));

            return result.FirstOrDefault();
        }

        public async Task<Article?> GetByCodeAsync(string code)
        {
            var normalizedCode = code?.Trim();
            if (string.IsNullOrEmpty(normalizedCode))
            {
                return null;
            }

            var result = await ExecuteArticleQueryAsync(
                @"SELECT * FROM Articles 
                  WHERE (Name = @code OR Code = @code) 
                  LIMIT 1",
                new SqliteParameter("@code", normalizedCode));

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Article>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllAsync();
            }

            var normalizedKeyword = $"%{keyword.Trim()}%";

            return await ExecuteArticleQueryAsync(
                @"SELECT * FROM Articles 
                  WHERE (Name LIKE @kw 
                        OR Description LIKE @kw 
                        OR Specification LIKE @kw 
                        OR Code LIKE @kw)",
                new SqliteParameter("@kw", normalizedKeyword));
        }

        private async Task<List<Article>> ExecuteArticleQueryAsync(string sql, params SqliteParameter[] parameters)
        {
            var articles = new List<Article>();
            var connectionString = _context.Database.GetConnectionString()
                ?? _context.Database.GetDbConnection().ConnectionString;

            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            if (parameters?.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            await using var reader = await command.ExecuteReaderAsync();
            var columns = GetColumnOrdinals(reader);

            while (await reader.ReadAsync())
            {
                articles.Add(MapArticle(reader, columns));
            }

            return articles;
        }

        private static IReadOnlyDictionary<string, int> GetColumnOrdinals(DbDataReader reader)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (!dict.ContainsKey(name))
                {
                    dict[name] = i;
                }
            }
            return dict;
        }

        private static Article MapArticle(DbDataReader reader, IReadOnlyDictionary<string, int> columns)
        {
            return new Article
            {
                Id = (int)(GetLong(reader, columns, "Id") ?? 0),
                Name = GetString(reader, columns, "Name", "Code", "Title"),
                Price = GetDecimal(reader, columns, "Price", "UnitPrice", "SalesPrice"),
                MinimumPrice = GetDecimal(reader, columns, "MinimumPrice", "CostPrice", "MinPrice"),
                Description = GetString(reader, columns, "Description"),
                Specification = GetString(reader, columns, "Specification", "Specs"),
                Discount = GetString(reader, columns, "Discount"),
                Note = GetString(reader, columns, "Note", "Notes", "Remark"),
                Specs_EN = GetString(reader, columns, "Specs_EN", "Specification_EN"),
                Category = GetString(reader, columns, "Category"),
                Name_EN = GetString(reader, columns, "Name_EN"),
                Description_EN = GetString(reader, columns, "Description_EN", "DescriptionEn")
            };
        }

        private static bool TryGetOrdinal(IReadOnlyDictionary<string, int> columns, out int ordinal, params string[] names)
        {
            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(name) && columns.TryGetValue(name, out ordinal))
                {
                    return true;
                }
            }

            ordinal = -1;
            return false;
        }

        private static string? GetString(DbDataReader reader, IReadOnlyDictionary<string, int> columns, params string[] names)
        {
            if (!TryGetOrdinal(columns, out var ordinal, names) || reader.IsDBNull(ordinal))
            {
                return null;
            }

            var value = reader.GetValue(ordinal);
            return value?.ToString();
        }

        private static long? GetLong(DbDataReader reader, IReadOnlyDictionary<string, int> columns, params string[] names)
        {
            if (!TryGetOrdinal(columns, out var ordinal, names) || reader.IsDBNull(ordinal))
            {
                return null;
            }

            var value = reader.GetValue(ordinal);
            return value switch
            {
                long l => l,
                int i => i,
                short s => s,
                byte b => b,
                string str when long.TryParse(str, out var parsed) => parsed,
                _ => null
            };
        }

        private static decimal? GetDecimal(DbDataReader reader, IReadOnlyDictionary<string, int> columns, params string[] names)
        {
            if (!TryGetOrdinal(columns, out var ordinal, names) || reader.IsDBNull(ordinal))
            {
                return null;
            }

            var value = reader.GetValue(ordinal);
            return value switch
            {
                decimal d => d,
                double dbl => (decimal)dbl,
                float f => (decimal)f,
                long l => l,
                int i => i,
                string str when decimal.TryParse(str, out var parsed) => parsed,
                _ => null
            };
        }
    }
}

