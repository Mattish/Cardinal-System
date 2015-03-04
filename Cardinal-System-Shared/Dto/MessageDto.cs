using System;
using Cardinal_System_Shared.Entity;
using Newtonsoft.Json;

namespace Cardinal_System_Shared.Dto
{
    // TODO: Make MessageDto Immutable
    public abstract class MessageDto
    {
        [JsonProperty(PropertyName = "F")]
        public MessageFamily Family { get; set; }
        [JsonProperty(PropertyName = "T")]
        public MessageType Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SI")]
        public EntityId SourceId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "TI")]
        public EntityId TargetId { get; set; }
        [JsonProperty(PropertyName = "SC")]
        public long SourceComponent { get; set; }
        [JsonProperty(PropertyName = "TC")]
        public long TargetComponent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "CT")]
        public DateTime CreatedTime { get; set; }

        public abstract Message TranslateFromDto();
    }
}