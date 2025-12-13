using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HappyTools.Shared
{
    public class BaseFilterModel
    {
        [JsonIgnore]
        public List<Guid> Ids { get; set; } = [];

        [JsonIgnore]
        public PagedAndSortedFilterModel? Input = new();

        [JsonIgnore]
        public bool ExcludePagination { get; set; } = false;

        [JsonIgnore]
        public bool EnableFilter { get; set; } = true;
    }
    public class PagedAndSortedFilterModel
    {
        public string? Sorting { get; set; }
        public int SkipCount { get; set; } = 0;
        public int MaxResultCount { get; set; } = 1000;
    }
}
