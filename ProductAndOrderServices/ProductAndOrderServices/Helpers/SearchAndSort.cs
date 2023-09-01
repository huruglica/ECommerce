using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductAndOrderServices.Helpers
{
    public class SearchAndSort
    {
        public string? Name { get; set; }
        public double? StartPrice { get; set; }
        public double? EndPrice { get; set; }

        [DefaultValue(1)]
        [Range(1, 250)]
        public int Page { get; set; } = 1;
        [DefaultValue(5)]
        [Range(5, 250)]
        public int PageSize { get; set; } = 5;

        public bool? IsAscending { get; set; }
    }
}
