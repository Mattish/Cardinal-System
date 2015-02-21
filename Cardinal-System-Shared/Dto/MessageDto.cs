using System;
using Cardinal_System_Shared.Entity;
using Newtonsoft.Json;

namespace Cardinal_System_Shared.Dto
{
    // TODO: Make MessageDto Immutable
    public abstract class MessageDto
    {
        public MessageFamily F { get; set; } //Family
        public MessageType T { get; set; } //Type
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntityId SI { get; set; } // SourceId
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntityId TI { get; set; } // TargetId
        public long SC { get; set; } // SourceComponent
        public long TC { get; set; } //TargetComponent
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CT { get; set; } // CreatedTime

        public abstract Message TranslateFromDto();
    }
}