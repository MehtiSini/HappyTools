using HappyTools.Contract.Dtos;
using HappyTools.Shared;

namespace HappyTools.Contract.Interfaces
{
    public interface ICrudService<TEntityListDto, TEntitySingleDto, TKey, TPagedAndSortedResultRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto>
         where TEntityListDto : EntityDto<TKey>, new()
         where TEntitySingleDto : EntityDto<TKey>, new()
         where TFilterModel : BaseFilterModel
         where TPagedAndSortedResultRequestDto : PageAndSortRequestDto
         where TReturnDto : CrudResponseDto<TKey>

    {
        Task<TReturnDto> CreateAsync(TCreateDto create);
        Task<TReturnDto> UpdateAsync(TKey id, TUpdateDto create);
        Task<List<TEntityListDto>> GetFilteredListAsync(TFilterModel filterModel);
        Task<PagedResultDto<TEntityListDto>> GetFilteredPagedListAsync(TPagedAndSortedResultRequestDto input, TFilterModel filterModel);
        Task<TEntitySingleDto> GetAsync(TKey id);
    }
}
