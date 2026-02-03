using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HappyTools.Contract.Dtos;
using HappyTools.Contract.Interfaces;
using HappyTools.CrossCutting.Data;
using HappyTools.CrossCutting.Event;
using HappyTools.DependencyInjection.Contracts;
using HappyTools.Domain.Entities.Audit.Abstractions;
using HappyTools.Domain.Entities.Concurrency;
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.EfCore.Uow;
using HappyTools.Shared;
using HappyTools.Shared.Identity;
using HappyTools.Shared.MultiTenancy;
using HappyTools.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace HappyTools.Application
{
    public class EfCrudService<TDbContext, TEntity, TEntityListDto, TEntitySingleDto, TKey, TPageAndSortRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto> :
       ICrudService<TEntityListDto, TEntitySingleDto, TKey, TPageAndSortRequestDto, TFilterModel, TCreateDto, TUpdateDto, TReturnDto>
       where TEntity : class, IEntity<TKey>, IHasConcurrencyStamp, IHasCreationTime, ICreationAuditedObject
       where TDbContext : DbContext
       where TEntityListDto : EntityDto<TKey>, new()
       where TEntitySingleDto : EntityDto<TKey>, new()
       where TFilterModel : BaseFilterModel
       where TPageAndSortRequestDto : PageAndSortRequestDto
       where TReturnDto : CrudResponseDto<TKey>, new()
    {
        private readonly IServiceProvider _provider;
        protected IUnitOfWorkManager UnitOfWorkManager => _provider.GetRequiredService<IUnitOfWorkManager>();
        protected TDbContext DbContext => UnitOfWorkManager.Current.GetDbContext<TDbContext>();
        protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();
        protected IDataFilter<ISoftDelete> SoftDeleteDataFilter => _provider.GetRequiredService<IDataFilter<ISoftDelete>>();
        protected ICurrentTenant CurrentTenant => _provider.GetRequiredService<ICurrentTenant>();
        protected ICurrentUser CurrentUser => _provider.GetRequiredService<ICurrentUser>();
        protected ILocalEventBus LocalEventBus => _provider.GetRequiredService<ILocalEventBus>();

        public EfCrudService(IServiceProvider provider)
        {
            _provider = provider;
        }


        public virtual async Task<TReturnDto> CreateAsync(TCreateDto input)
        {
            var entity = MapCreateDtoToEntity(input);

            await DbContext.Set<TEntity>()
                .AddAsync(entity);

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

            var entity = await DbContext.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id!.Equals(id));

            if (entity == null)
                throw new ValidationException($"No entity {typeof(TEntity).Name} with Id ==> {id}");

            var mappedEntity = MapUpdateDtoToEntity(entity, input);

            mappedEntity.ConcurrencyStamp = entity.ConcurrencyStamp;

            //DbSet.Update(mappedEntity);

            return new TReturnDto
            {
                Id = entity.Id
            };
        }

        public async Task CreateBulkAsync(List<TCreateDto> inputs)
        {
            var entities = MapCreateDtosToEntities(inputs);

            if (entities.Any())
                await DbSet.AddRangeAsync(entities);
        }


        // Soft delete
        public virtual async Task<TReturnDto> SoftDeleteAsync(TKey id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var entity = await DbContext.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id.Equals(id));

            if (entity == null)
                throw new ValidationException($"No entity {typeof(TEntity).Name} with Id ==> {id}");

            DbContext.Set<TEntity>().Remove(entity);

            return new TReturnDto { Id = entity.Id };
        }

        public virtual async Task<TReturnDto> HardDeleteAsync(TKey id)
        {
            using (SoftDeleteDataFilter.Disable())
            {
                if (id is null)
                    throw new ArgumentNullException(nameof(id));

                var entity = await DbSet.FindAsync(id);
                if (entity == null)
                    throw new KeyNotFoundException($"Entity with id {id} not found.");

                DbSet.Remove(entity);
                await DbContext.SaveChangesAsync();

                return new TReturnDto { Id = id };
            }
        }




        public virtual async Task<List<TEntityListDto>> GetFilteredListAsync(TFilterModel filterModel)
        {
            var entities = await DbSet
                   .AsNoTracking()
                   .ToListAsync();

            return MapEntitiesToDtos(entities);

        }


        public virtual async Task<PagedResultDto<TEntityListDto>> GetFilteredPagedListAsync(TPageAndSortRequestDto input, TFilterModel filterModel)
        {
            var query = DbSet
                .AsNoTracking()
                .AsQueryable();

            query = query.OrderByDescending(e => e.CreationTime);

            query = ApplyPagedQuery(query, input);

            var totalCount = await query.LongCountAsync();

            var allCount = await DbSet
                 .LongCountAsync();

            if (allCount <= 0)
                return new PagedResultDto<TEntityListDto>(0, 0, null);

            var results = await ProjectToListDto(query).ToListAsync();

            return new PagedResultDto<TEntityListDto>(
        allCount: allCount,
        totalCount: totalCount,
        items: results
    );
        }

        public virtual IQueryable<TEntity> CreateFilteredQuery(TFilterModel filterModel, bool asNoTracking = false)
        {
            return DbContext.Set<TEntity>()
                 .AsNoTracking()
                 //ApplyFilteres
                 .AsQueryable();
        }

        public virtual async Task<IQueryable<TEntity>> CreateFilteredQueryAsync(TFilterModel filterModel, bool asNoTracking = false)
        {
            return DbContext.Set<TEntity>()
                 .AsNoTracking()
                 //ApplyFilteres
                 .AsQueryable();
        }

        public static IQueryable<TEntity> ApplyPagedQuery<TEntity>(
    IQueryable<TEntity> query,
  PageAndSortRequestDto input)
            where TEntity : ICreationAuditedObject
        {
            if (input.SkipCount > 0)
                query = query.Skip(input.SkipCount);
            if (input.MaxResultCount > 0)
                query = query.Take(input.MaxResultCount);
            query = query.OrderByDescending(e => e.CreationTime);

            return query;
        }


        public virtual IQueryable<TEntityListDto> ProjectToListDto(
           IQueryable<TEntity> query)
        {
            return query.SelectInto(e => new TEntityListDto
            {
                // mapping
            });
        }

        public virtual async Task<TEntitySingleDto> ProjectToSingleDtoAsync(
            IQueryable<TEntity> query)
        {
            return await query.SelectInto(e => new TEntitySingleDto
            {
                // mapping
            }).FirstOrDefaultAsync();
        }


        public virtual async Task<TEntitySingleDto> GetAsync(TKey id)
        {
            var targetQuery = DbSet
.Where(t => t.Id.Equals(id));

            var entity = await ProjectToSingleDtoAsync(targetQuery);

            if (entity is null)
                throw new ValidationException($"There Is No {typeof(TEntity).Name} With Id = {id}");

            return entity;
        }
        public virtual async Task<TEntity> GetEntityAsync(TKey id)
        {
            return await DbSet
.Where(t => t.Id.Equals(id))
.FirstOrDefaultAsync();
        }


        public virtual async Task<List<TEntity>> GetEntitiesAsync()
        {
            return await DbSet.
                 AsNoTracking()
                 .ToListAsync();
        }

        public async Task InsertAsync(TEntity entity, bool autoSave = false)
        {
            DbSet.Add(entity);
            if (autoSave)
                await SaveChangesAsync();
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            DbSet.AddRange(entities);
            if (autoSave)
                await SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity, bool autoSave = false)
        {
            DbSet.Update(entity);

            if (autoSave)
                await SaveChangesAsync();

        }
        public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            DbSet.UpdateRange(entities);
            if (autoSave)
                await SaveChangesAsync();

        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
        {
            var entities = await DbContext.Set<TEntity>()
      .Where(predicate)
      .ToListAsync();

            if (entities == null)
                throw new KeyNotFoundException();

            DbSet.RemoveRange(entities);

            if (autoSave)
            {
                await SaveChangesAsync();
            }
        }
        public Task SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(TKey id)
        {
            var entity = Activator.CreateInstance<TEntity>();
            typeof(TEntity).GetProperty("Id")!.SetValue(entity, id);

            DbSet.Attach(entity);
            DbSet.Remove(entity);

            return Task.CompletedTask;
        }

        public Task DeleteManyAsync(IEnumerable<TKey> ids)
        {
            foreach (var id in ids)
            {
                var entity = Activator.CreateInstance<TEntity>();
                typeof(TEntity).GetProperty("Id")!.SetValue(entity, id);
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }

            return Task.CompletedTask;
        }

        //Mappings

        public virtual List<TEntity> MapCreateDtosToEntities(List<TCreateDto> createDtos)
        {
            var mappedEntities = new List<TEntity>();
            foreach (var createDto in createDtos)
            {
                var mappedEntity = Activator.CreateInstance<TEntity>();
                createDto.CopyPropertiesTo(mappedEntity);
                mappedEntities.Add(mappedEntity);
            }
            return mappedEntities;
        }

        public virtual TEntity MapCreateDtoToEntity(TCreateDto createDto)
        {
            var mappedEntity = Activator.CreateInstance<TEntity>();
            createDto.CopyPropertiesTo(mappedEntity);
            return mappedEntity;
        }

        // UpdateDtos → Entities
        public virtual List<TEntity> MapUpdateDtosToEntities(List<TUpdateDto> updateDtos)
        {
            var mappedEntities = new List<TEntity>();
            foreach (var updateDto in updateDtos)
            {
                var mappedEntity = Activator.CreateInstance<TEntity>();
                updateDto.CopyPropertiesTo(mappedEntity);
                mappedEntities.Add(mappedEntity);
            }
            return mappedEntities;
        }

        public virtual TEntity MapUpdateDtoToEntity(TEntity entity, TUpdateDto updateDto)
        {
            var mappedEntity = Activator.CreateInstance<TEntity>();
            updateDto.CopyPropertiesTo(entity);
            return mappedEntity;
        }

        // Entity → Single DTO
        public virtual TEntitySingleDto MapEntityToDto(TEntity entity)
        {
            var mappedDto = Activator.CreateInstance<TEntitySingleDto>();
            entity.CopyPropertiesTo(mappedDto);
            return mappedDto;
        }

        // Entities → List DTOs
        public virtual List<TEntityListDto> MapEntitiesToDtos(List<TEntity> entities)
        {
            var mappedDtos = new List<TEntityListDto>();
            foreach (var entity in entities)
            {
                var mappedDto = Activator.CreateInstance<TEntityListDto>();
                entity.CopyPropertiesTo(mappedDto);
                mappedDtos.Add(mappedDto);
            }
            return mappedDtos;
        }

        // Entity → Create DTO
        public virtual TCreateDto MapEntityToCreateDto(TEntity entity)
        {
            var mappedDto = Activator.CreateInstance<TCreateDto>();
            entity.CopyPropertiesTo(mappedDto);
            return mappedDto;
        }

        // Entities → List of Create DTOs
        public virtual List<TCreateDto> MapEntitiesToCreateDtos(List<TEntity> entities)
        {
            var mappedDtos = new List<TCreateDto>();
            foreach (var entity in entities)
            {
                var mappedDto = Activator.CreateInstance<TCreateDto>();
                entity.CopyPropertiesTo(mappedDto);
                mappedDtos.Add(mappedDto);
            }
            return mappedDtos;
        }

        // Entity → Update DTO
        public virtual TUpdateDto MapEntityToUpdateDto(TEntity entity)
        {
            var mappedDto = Activator.CreateInstance<TUpdateDto>();
            entity.CopyPropertiesTo(mappedDto);
            return mappedDto;
        }

        // Entities → List of Update DTOs
        public virtual List<TUpdateDto> MapEntitiesToUpdateDtos(List<TEntity> entities)
        {
            var mappedDtos = new List<TUpdateDto>();
            foreach (var entity in entities)
            {
                var mappedDto = Activator.CreateInstance<TUpdateDto>();
                entity.CopyPropertiesTo(mappedDto);
                mappedDtos.Add(mappedDto);
            }
            return mappedDtos;
        }

        public virtual List<T1> MapTo<T1, T2>(List<T2> sourceList)
            where T1 : class, new()
        {
            var mappedList = new List<T1>();
            foreach (var item in sourceList)
            {
                var mappedItem = new T1();
                item.CopyPropertiesTo(mappedItem);
                mappedList.Add(mappedItem);
            }
            return mappedList;
        }

        protected virtual TEntitySingleDto MapToGetOutputDto(TEntity entity)
        {
            var mappedDto = Activator.CreateInstance<TEntitySingleDto>();
            mappedDto.CopyPropertiesTo(entity);
            return mappedDto;
        }


    }
}
