using System.ComponentModel.DataAnnotations;
using HappyTools.Contract.Dtos;
using HappyTools.Contract.Interfaces;
using HappyTools.DependencyInjection.Contracts;
using HappyTools.Domain.Entities.Audit.Abstractions;
using HappyTools.Domain.Entities.Concurrency;
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.Shared;
using HappyTools.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace HappyTools.Application
{
    public class EfCrudService<TDbContext, TEntity, TEntityListDto, TEntitySingleDto, TKey, TPagedAndSortedResultRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto> :
          ICrudService<TEntityListDto, TEntitySingleDto, TKey, TPagedAndSortedResultRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto>
          where TEntity : class, IEntity<TKey>, IHasConcurrencyStamp, IHasCreationTime, ICreationAuditedObject
          where TDbContext : DbContext
          where TEntityListDto : EntityDto<TKey>, new()
          where TEntitySingleDto : EntityDto<TKey>, new()
          where TFilterModel : BaseFilterModel
          where TPagedAndSortedResultRequestDto : PageAndSortRequestDto
          where TReturnDto : CrudResponseDto<TKey>, new()

    {
        private readonly TDbContext _dbContext;

        public EfCrudService(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async  Task<TReturnDto> CreateAsync(TCreateDto input)
        {
            var entity = await MapCreateDtoToEntityAsync(input);

            await _dbContext.Set<TEntity>()
                .AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            var result = new TReturnDto
            {
                Id = entity.Id
            };
            return result;
        }

        public virtual async Task<TReturnDto> UpdateAsync(TKey id, TUpdateDto input)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var entity = await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id!.Equals(id));

            if (entity == null)
                throw new ValidationException($"No entity {typeof(TEntity).Name} with Id ==> {id}");

            var mappedEntity = await MapUpdateDtoToEntityAsync(entity, input);

            mappedEntity.ConcurrencyStamp = entity.ConcurrencyStamp;

            _dbContext.Set<TEntity>()
                .Update(mappedEntity);
            await _dbContext.SaveChangesAsync();

            return new TReturnDto
            {
                Id = entity.Id
            };
        }


        // Soft delete
        public virtual async Task<TReturnDto> SoftDeleteAsync(TKey id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var entity = await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id!.Equals(id));

            if (entity == null)
                throw new ValidationException($"No entity {typeof(TEntity).Name} with Id ==> {id}");

            if (entity is ISoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync();

                return new TReturnDto { Id = entity.Id };
            }

            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not support soft delete.");
        }

        // Hard delete
        public virtual async Task<TReturnDto> HardDeleteAsync(TKey id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var entity = await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id!.Equals(id));

            if (entity == null)
                throw new ValidationException($"No entity {typeof(TEntity).Name} with Id ==> {id}");

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();

            return new TReturnDto { Id = entity.Id };
        }



        public virtual async Task<List<TEntityListDto>> GetFilteredListAsync(TFilterModel filterModel)
        {
            var entities = await _dbContext.Set<TEntity>()
                   .AsNoTracking()
                   .ToListAsync();

            return await MapEntitiesToDtosAsync(entities);

        }


        public virtual async Task<PagedResultDto<TEntityListDto>> GetFilteredPagedListAsync(TPagedAndSortedResultRequestDto input, TFilterModel filterModel)
        {
            var query = _dbContext.Set<TEntity>()
                .AsNoTracking()
                .AsQueryable();

            query = query.OrderByDescending(e => e.CreationTime);

            var totalCount = await query.LongCountAsync();

            var entities = await _dbContext.Set<TEntity>()
                 .AsNoTracking()
                 .ToListAsync();

            var results = await MapEntitiesToDtosAsync(entities);

            return new PagedResultDto<TEntityListDto>(
        allCount: entities.Count,
        totalCount: totalCount,
        items: results
    );
        }


        public async Task<TEntitySingleDto> GetAsync(TKey id)
        {
            var targetEntity = await _dbContext.Set<TEntity>()
.Where(t => t.Id.Equals(id))
.FirstOrDefaultAsync();

            if (targetEntity is null)
                throw new ValidationException($"There Is No {typeof(TEntity).Name} With Id = {id}");

            return await MapToGetOutputDtoAsync(targetEntity);
        }

        //Mappings

        public virtual List<TEntity> MapCreateDtosToEntities(List<TCreateDto> createDtos)
        {
            var mappedEntities = new List<TEntity>();

            createDtos.ForEach(createDto =>
            {
                var mappedEntity = Activator.CreateInstance<TEntity>();
                createDto.CopyPropertiesFrom(mappedEntity);
                mappedEntities.Add(mappedEntity);
            });

            return mappedEntities;
        }

        public virtual TEntity MapCreateDtoToEntity(TCreateDto createDto)
        {
            var mappedEntity = Activator.CreateInstance<TEntity>();
            mappedEntity.CopyPropertiesFrom(createDto);

            return mappedEntity;
        }


        public virtual List<TEntity> MapUpdateDtosToEntities(List<TCreateDto> createDtos)
        {
            var mappedEntities = new List<TEntity>();

            createDtos.ForEach(createDto =>
            {
                var mappedEntity = Activator.CreateInstance<TEntity>();
                createDto.CopyPropertiesFrom(mappedEntity);
                mappedEntities.Add(mappedEntity);
            });

            return mappedEntities;
        }

        public virtual TEntity MapUpdateDtoToEntity(TUpdateDto createDto)
        {
            var mappedEntity = Activator.CreateInstance<TEntity>();
            mappedEntity.CopyPropertiesFrom(createDto);

            return mappedEntity;
        }

        public virtual TEntitySingleDto MapEntityToDto(TEntity createDto)
        {
            var mappedEntity = Activator.CreateInstance<TEntitySingleDto>();
            mappedEntity.CopyPropertiesFrom(createDto);

            return mappedEntity;
        }

        public virtual List<TEntityListDto> MapEntitiesToDtos(List<TEntity> entities)
        {
            var mappedDtos = new List<TEntityListDto>();

            entities.ForEach(createDto =>
            {
                var mappedDto = Activator.CreateInstance<TEntityListDto>();
                createDto.CopyPropertiesFrom(mappedDto);
                mappedDtos.Add(mappedDto);
            });

            return mappedDtos;
        }

        public virtual TCreateDto MapEntityToCreateDto(TEntity createDto)
        {
            var mappedEntity = Activator.CreateInstance<TCreateDto>();
            mappedEntity.CopyPropertiesFrom(createDto);

            return mappedEntity;
        }

        public virtual List<TCreateDto> MapEntitiesToCreateDtos(List<TEntity> entities)
        {
            var mappedDtos = new List<TCreateDto>();

            entities.ForEach(createDto =>
            {
                var mappedDto = Activator.CreateInstance<TCreateDto>();
                createDto.CopyPropertiesFrom(mappedDto);
                mappedDtos.Add(mappedDto);
            });

            return mappedDtos;
        }

        public virtual TUpdateDto MapEntityToUpdateDto(TEntity createDto)
        {
            var mappedEntity = Activator.CreateInstance<TUpdateDto>();
            mappedEntity.CopyPropertiesFrom(createDto);

            return mappedEntity;
        }

        public virtual List<TUpdateDto> MapEntitiesToUpdateDtos(List<TEntity> entities)
        {
            var mappedDtos = new List<TUpdateDto>();

            entities.ForEach(createDto =>
            {
                var mappedDto = Activator.CreateInstance<TUpdateDto>();
                createDto.CopyPropertiesFrom(mappedDto);
                mappedDtos.Add(mappedDto);
            });

            return mappedDtos;
        }


        public async virtual Task<List<TEntity>> MapCreateDtosToEntitiesAsync(List<TCreateDto> createDtos)
        {
            var mappedEntities = new List<TEntity>();

            await Task.Run(() =>
            {
                createDtos.ForEach(createDto =>
                {
                    var mappedEntity = Activator.CreateInstance<TEntity>();
                    mappedEntity.CopyPropertiesFrom(createDto);
                    mappedEntities.Add(mappedEntity);
                });
            });

            return mappedEntities;
        }


        public async virtual Task<List<T1>> MapToAsync<T1, T2>(List<T2> createDtos)
          where T1 : class, new()
        {
            var mappedEntities = new List<T1>();

            await Task.Run(() =>
            {
                createDtos.ForEach(createDto =>
                {
                    var mappedEntity = new T1();
                    mappedEntity.CopyPropertiesFrom(createDto);
                    mappedEntities.Add(mappedEntity);
                });
            });

            return mappedEntities;
        }



        public virtual async Task<TEntity> MapCreateDtoToEntityAsync(TCreateDto createDto)
        {
            return await Task.Run(() =>
            {
                var mappedEntity = Activator.CreateInstance<TEntity>();
                mappedEntity.CopyPropertiesFrom(createDto);
                return mappedEntity;
            });
        }

        public virtual async Task<List<TEntity>> MapUpdateDtosToEntitiesAsync(List<TCreateDto> createDtos)
        {
            var mappedEntities = new List<TEntity>();

            await Task.Run(() =>
            {
                createDtos.ForEach(createDto =>
                {
                    var mappedEntity = Activator.CreateInstance<TEntity>();
                    createDto.CopyPropertiesFrom(mappedEntity);
                    mappedEntities.Add(mappedEntity);
                });
            });

            return mappedEntities;
        }

        public virtual async Task<TEntity> MapUpdateDtoToEntityAsync(TEntity entity, TUpdateDto updateDto)
        {
            return await Task.Run(() =>
            {
                entity.CopyPropertiesFrom(updateDto);
                return entity;
            });
        }

        public virtual async Task<TEntity> MapDtoToEntityAsync(TEntitySingleDto createDto)
        {
            return await Task.Run(() =>
            {
                var entity = Activator.CreateInstance<TEntity>();
                createDto.CopyPropertiesFrom(entity);
                return entity;
            });
        }

        public virtual async Task<TEntitySingleDto> MapEntityToDtoAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                var mappedDto = Activator.CreateInstance<TEntitySingleDto>();
                mappedDto.CopyPropertiesFrom(entity);
                return mappedDto;
            });
        }

        public virtual async Task<List<TEntityListDto>> MapEntitiesToDtosAsync(List<TEntity> entities)
        {
            var mappedDtos = new List<TEntityListDto>();

            await Task.Run(() =>
            {
                entities.ForEach(entity =>
                {
                    var mappedDto = Activator.CreateInstance<TEntityListDto>();
                    entity.CopyPropertiesTo(mappedDto);
                    mappedDtos.Add(mappedDto);
                });
            });

            return mappedDtos;
        }

        public virtual async Task<TCreateDto> MapEntityToCreateDtoAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                var mappedDto = Activator.CreateInstance<TCreateDto>();
                mappedDto.CopyPropertiesFrom(entity);
                return mappedDto;
            });
        }

        public virtual async Task<List<TCreateDto>> MapEntitiesToCreateDtosAsync(List<TEntity> entities)
        {
            var mappedDtos = new List<TCreateDto>();

            await Task.Run(() =>
            {
                entities.ForEach(entity =>
                {
                    var mappedDto = Activator.CreateInstance<TCreateDto>();
                    entity.CopyPropertiesFrom(mappedDto);
                    mappedDtos.Add(mappedDto);
                });
            });

            return mappedDtos;
        }

        public virtual async Task<TUpdateDto> MapEntityToUpdateDtoAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                var mappedDto = Activator.CreateInstance<TUpdateDto>();
                entity.CopyPropertiesTo(mappedDto);
                return mappedDto;
            });
        }

        public virtual async Task<List<TUpdateDto>> MapEntitiesToUpdateDtosAsync(List<TEntity> entities)
        {
            var mappedDtos = new List<TUpdateDto>();

            await Task.Run(() =>
            {
                entities.ForEach(entity =>
                {
                    var mappedDto = Activator.CreateInstance<TUpdateDto>();
                    entity.CopyPropertiesFrom(mappedDto);
                    mappedDtos.Add(mappedDto);
                });
            });

            return mappedDtos;
        }

        protected async Task<TEntitySingleDto> MapToGetOutputDtoAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                var mappedDto = Activator.CreateInstance<TEntitySingleDto>();
                entity.CopyPropertiesTo(mappedDto);
                return mappedDto;
            });
        }


    }
}
