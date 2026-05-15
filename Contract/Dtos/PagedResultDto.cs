using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos
{
    public class PageAndSortRequestDto
    {
        [Range(0, int.MaxValue)]
        public virtual int SkipCount { get; set; }
        public virtual string? Sorting { get; set; }
        [Range(0, int.MaxValue)]
        public virtual int MaxResultCount { get; set; }
    }

    public class PagedResultDto<T>
    {
        public long TotalCount { get; set; }
        public long AllCount { get; set; }

        public IReadOnlyList<T> Items {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }
        private IReadOnlyList<T>? _items;

        public PagedResultDto()
        {

        }

        public PagedResultDto(long allCount, long totalCount, IReadOnlyList<T> items)
        {
            TotalCount = totalCount;
            AllCount = allCount;
            Items = items;
        }
    }

    public class PagedFullResultDto<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public long TotalCount { get; set; }

        public int TotalPages { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        public PagedFullResultDto()
        {

        }

        public PagedFullResultDto( long totalCount, int pageNumber, int pageSize, IReadOnlyList<T> items)
        {
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;

            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            HasPreviousPage = pageNumber > 1;

            HasNextPage = pageNumber < TotalPages;

            Items = items;
        }
    }

}
