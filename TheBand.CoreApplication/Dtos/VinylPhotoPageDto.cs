namespace TheBand.CoreApplication.Dtos;

public record VinylPhotoPageDto(
    IReadOnlyCollection<VinylPhotoDto> Items,
    int Page,
    int PageSize,
    long TotalItems)
{
    public long TotalPages => TotalItems / PageSize + (TotalItems % PageSize == 0 ? 0 : 1);

    public bool HasNextPage => Page < TotalPages;
}
