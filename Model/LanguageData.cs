
namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from language
    /// </summary>
    public class LanguageData
    {
        public int Lcid { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ISOCode { get; set; }
        public string Region { get; set; }
        public override string ToString()
        {
            return  DisplayName+ " ["+ ISOCode+"] ("+Lcid+")";
        }
    }
}
