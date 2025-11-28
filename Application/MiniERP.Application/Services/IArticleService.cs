using MiniERP.Domain;

namespace MiniERP.ApplicationLayer.Services
{
    public interface IArticleService
    {
        Task<Article?> GetArticleByIdAsync(int id);
        Task<Article?> GetArticleByCodeAsync(string code);
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<IEnumerable<Article>> SearchArticlesAsync(string keyword);
        Task<Article> CreateArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(int id);
    }
}

