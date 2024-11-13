using Fun_Funding.Domain.Enum;

namespace Fun_Funding.Application.ViewModel.MarketplaceFileDTO
{
    public class MarketplaceFileInfoResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? URL { get; set; }
        public string? Version { get; set; }
        public string? Description { get; set; }
        public FileType FileType { get; set; }
        public bool IsDelete { get; set; }
    }
}
