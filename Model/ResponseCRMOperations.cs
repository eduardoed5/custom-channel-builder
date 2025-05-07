using System;
using System.Collections.Generic;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from response of a crm operation
    /// </summary>
    public class ResponseCRMOperations
    {
        public bool State { get; set; }
        public string Message { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<EntityData> Entities { get; set; }
        public List<CustomAPI> Apis { get; set; }
        public List<LanguageData> Languages { get; set; }
        public Exception Exception { get; set; }
    }
}
