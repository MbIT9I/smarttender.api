using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommercialServices.DTO.ExternalApi;
using Newtonsoft.Json;

namespace SmartTender.Api
{
    public class BidDtoExt
    {
        public BidderInfoDto BidderInfo { get; set; }
        public List<BidDetailDto> BidDetails { get; set; }
        public List<CriteriaDto> CriteriaValues { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public int Id { get; set; }
    }
}