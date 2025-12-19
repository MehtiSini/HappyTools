using HappyTools.Contract.Dtos;
using HappyTools.Shared;

namespace HappyTools.Contract.Interfaces
{

    public interface ICrudService<TEntityListDto, TEntitySingleDto, TKey, TPageAndSortRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto>
         where TEntityListDto : EntityDto<TKey>, new()
         where TEntitySingleDto : EntityDto<TKey>, new()
         where TFilterModel : BaseFilterModel
         where TPageAndSortRequestDto : PageAndSortRequestDto
         where TReturnDto : CrudResponseDto<TKey>

    {
        Task<TReturnDto> CreateAsync(TCreateDto create);
        Task<TReturnDto> UpdateAsync(TKey id, TUpdateDto create);
        Task<List<TEntityListDto>> GetFilteredListAsync(TFilterModel filterModel);
        Task<PagedResultDto<TEntityListDto>> GetFilteredPagedListAsync(TPageAndSortRequestDto input, TFilterModel filterModel);
        Task<TEntitySingleDto> GetAsync(TKey id);
    }
}
