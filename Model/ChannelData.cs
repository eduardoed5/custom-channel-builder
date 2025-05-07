using System;
using System.Collections.Generic;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that manages the attributes of a channel
    /// </summary>
    public class ChannelData
    {
        public Guid ChannelId { get; set; }
        public Guid LocaleIdALT { get; set; }
        public Guid LocaleIdENG { get; set; }
        public EntityData ConfigurationEntity { get; set; }
        public FormData ConfigurationForm { get; set; }
        public List<MessagePartData> MessagesParts { get; set; }
        public EntityData EditorEntity { get; set; }
        public FormData EditorForm { get; set; }
        public LabelData LabelAlternative { get; set; }
        public LabelData LabelEnglish { get; set; }
        public bool ExistingSolution { get; set; }
        public SolutionData Solution { get; set; }
        public string SolutionName { get; set; }
        public Publisher Publisher { get; set; }
        public string ChannelName { get; set; }
        public CustomAPI CustomAPI { get; set; }
        public string Description { get; set; }
        public bool AllowInbound { get; set; }
        public bool AllowDelivery { get; set; }
        public bool AllowAttachment { get; set; }
        public bool AllowBinary { get; set; }
        public bool RequiresSpecialConsent { get; set; }
        public bool PublishStart { get; set; }
        public List<AdditionalEntity> AdditionalEntities { get; set; }
    }
}
