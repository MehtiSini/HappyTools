using HappyTools.Contract.Dtos;
using HappyTools.Contract.Interfaces;
using HappyTools.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Api
{
    [ApiController]
    [Route("api/app")]
    public abstract class CrudController<TAppService, TEntityListDto, TEntitySingleDto, TKey, TPageAndSortRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto> : ControllerBase
         where TAppService : ICrudService<TEntityListDto, TEntitySingleDto, TKey, TPageAndSortRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto>
         where TEntityListDto : EntityDto<TKey>, new()
         where TEntitySingleDto : EntityDto<TKey>, new()
         where TFilterModel : BaseFilterModel
         where TPageAndSortRequestDto : PageAndSortRequestDto
         where TReturnDto : CrudResponseDto<TKey>
    {
        private readonly TAppService _appService;

        public CrudController(TAppService appService)
        {
            _appService = appService;
        }

        [HttpPost]
        [Route("admin/[controller]")]
        [Authorize(Roles = "RegulatorAdmin,SuperAdmin,Admin")]
        public async virtual Task<TReturnDto> Create([FromBody] TCreateDto objectDto)
        {

            var res = await _appService.CreateAsync(objectDto);
            return res;
        }


        [HttpPut]
        [Route("admin/[controller]/{id}")]
        [Authorize(Roles = "RegulatorAdmin,SuperAdmin,Admin")]
        public async virtual Task<TReturnDto> Update(TKey id, [FromBody] TUpdateDto objDto)
        {
            var res = await _appService.UpdateAsync(id, objDto);
            return res;
        }


        [HttpGet]
        [Route("admin/[controller]/{id}")]
        [Authorize(Roles = "RegulatorAdmin,SuperAdmin,Admin")]
        public async virtual Task<TEntitySingleDto> GetAsync(TKey id)
        {
            var res = await _appService.GetAsync(id);
            return res;

        }

        [HttpPost]
        [Route("admin/[controller]/filter")]
        [Authorize(Roles = "RegulatorAdmin,SuperAdmin,Admin")]
        public async virtual Task<PagedResultDto<TEntityListDto>> GetFilteredList([FromQuery] TPageAndSortRequestDto input, [FromBody] TFilterModel filterModel)
        {

            var filteredPagedResult = await _appService.GetFilteredPagedListAsync(input, filterModel);
            return filteredPagedResult;
        }


    }
}
