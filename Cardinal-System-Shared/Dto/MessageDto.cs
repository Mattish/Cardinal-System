using System;
using Cardinal_System_Shared.Entity;
using Newtonsoft.Json;

namespace Cardinal_System_Shared.Dto
{
    // TODO: Make the Dto smaller with field naming
    // TODO: Make MessageDto Immutable
    public abstract class MessageDto
    {
        public MessageFamily Family { get; set; }
        public MessageType Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntityId SourceId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntityId TargetId { get; set; }
        public long SourceComponent { get; set; }
        public long TargetComponent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedTime { get; set; }

        public abstract Message TranslateFromDto();
    }
}