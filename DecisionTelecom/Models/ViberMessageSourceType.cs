namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents Viber message source type
    /// </summary>
    public enum ViberMessageSourceType
    {
        /// <summary>
        /// Message can have text, picture and button 
        /// </summary>
        Promotional = 1,
        
        /// <summary>
        /// Message can have only text
        /// </summary>
        Transactional = 2
    }
}