using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    ///  Class that handles information from message part
    /// </summary>
    public class MessagePartData
    {
        public Guid Id { get; set; }
        public Guid ChannelId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public int? MaxLength { get; set; }
        public int Type { get; set; }
        public int State { get; set; }
        public int Status { get; set; }
        public int? Order { get; set; }
        public int? EntityId { get; set; }
        public string Entity { get; set; }
        public string EntityName { get; set; }
        public string ViewName { get; set; }
        public Guid ViewId { get; set; }
        public string LabelNameALT { get; set; }
        public string LabelDescriptionALT { get; set; }
        public string LabelNameENG { get; set; }
        public string LabelDescriptionENG { get; set; }
    }
}
