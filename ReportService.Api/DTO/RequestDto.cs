using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReportService.Api.DTO
{
    public class RequestDto
    {
        [DefaultValue(0)]
        public int PageIndex { get; set; } = 0;

        [DefaultValue(10)]
        [Range(1, 100, ErrorMessage = "The value must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
/*
        public int ItemId { get; set; }

        public DateTime Start {  get; set; }

        public DateTime End { get; set; }*/
    }
}
