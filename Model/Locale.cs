using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from locale
    /// </summary>
    public class Locale
    {
        public int LanguageId { get; set; }
        public Guid Id { get; set; }
        public Guid ChannelId { get; set; }
        public string JSONContent { get; set; }
    }
}
