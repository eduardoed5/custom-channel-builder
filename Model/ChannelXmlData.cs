using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles channel attributes for XML files
    /// </summary>
    public class ChannelXmlData
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string EndpointOutput { get; set; }
        public bool AllowInbound { get; set; }
        public bool AllowDelivery { get; set; }
        public bool AllowAccounts { get; set; }
        public bool RequiresSpecialConsent { get; set; }
        public string ConfigurationEntity { get; set; }
        public Guid ConfigurationForm { get; set; }
        public Guid EditorForm { get; set; }
        public bool AllowAttachment { get; set; }
        public bool AllowBinary { get; set; }
        public int State { get; set; }
        public int Status { get; set; }
        public string AccountEntity { get; set; }
        public Guid AccountForm { get; set; }

        public ChannelXmlData() { }
        public ChannelXmlData(ChannelData channel)
        {
            Id = channel.ChannelId;
            DisplayName = channel.ChannelName;
            Description = channel.Description;
            Type = channel.ChannelType;
            EndpointOutput = "/" + channel.CustomAPI.UniqueName;
            AllowInbound = channel.AllowInbound;
            AllowDelivery = channel.AllowDelivery;
            AllowAccounts = false;
            RequiresSpecialConsent = channel.RequiresSpecialConsent;
            ConfigurationEntity = channel.ConfigurationEntity.LogicalName;
            ConfigurationForm = channel.ConfigurationForm.Id;
            EditorForm = channel.EditorForm?.Id ?? Guid.Empty;
            AllowAttachment = channel.AllowAttachment;
            AllowBinary = channel.AllowBinary;
            State = 0;
            Status = 1;
            if (channel.AccountEntity != null && channel.ChannelType.Equals("SMS"))
            {
                AccountEntity = channel.AccountEntity.LogicalName;
                AccountForm = channel.AccountForm.Id;
            }
            else
            {
                AccountEntity = null;
                AccountForm = Guid.Empty;
            }

        }
    }
}
