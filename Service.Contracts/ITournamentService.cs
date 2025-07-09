using Tournament.Core.DTOs;

namespace Services.Contracts
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentDto>> GetAllAsync(bool includeGames);
        Task<TournamentDto> GetAsyncWithGames(int id, bool includeGames);
        Task<bool> TournamentExistAsync(int id);
        Task<TournamentDto> CreateAsync(TournamentCreateDto dto);
        Task<bool> UpdateAsync(int id, TournamentUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<TournamentDto>, PaginationMetadata)> GetFilteredAsync(
    string? title, string? searchQuery, int pageNumber, int pageSize);

    }
}