using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Interfaces;

public interface IConcertService
{
    Task<ConcertDto> CreateAsync(string userId, CreateConcertDto request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ConcertDto>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ConcertPriceByYearDto>> GetPriceByYearAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ConcertDto>> GetUpcomingAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ConcertDto>> GetPastAsync(string userId, CancellationToken cancellationToken);

    Task<ConcertDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<ConcertDto?> UpdateAsync(Guid guid, string userId, UpdateConcertDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
