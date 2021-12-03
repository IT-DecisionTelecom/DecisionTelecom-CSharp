namespace ITDecision.Viber.Models
{
    public enum MessageType
    {
        /// <summary>
        /// Text only (convenient for transactional messages)
        /// </summary>
        TextOnly = 106,
        
        /// <summary>
        /// Text+image+button (convenient for promotional messages)
        /// </summary>
        TextImageButton = 108,
        
        /// <summary>
        /// Text only 2way (convenient for transactional messages)
        /// </summary>
        TextOnly2Way = 206,
        
        /// <summary>
        /// Text+image+button 2way (convenient for promotional messages)
        /// </summary>
        TextImageButton2Way = 208,
    }
}