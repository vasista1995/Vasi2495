namespace PDS.Space.Common.Data.PADSModel
{
    public class DocumentPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to Document section in pads document.
        /// </summary>

        public string Version { get; set; }
        public string Type { get; set; }
        public RepetitionPads Repetition { get; set; }
        
    }
}
