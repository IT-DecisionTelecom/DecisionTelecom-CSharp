namespace ITDecision.Viber.Models
{
    public enum MessageSourceType
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