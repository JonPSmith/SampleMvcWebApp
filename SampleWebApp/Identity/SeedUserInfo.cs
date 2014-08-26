namespace SampleWebApp.Identity
{
    public class SeedUserInfo
    {
        /// <summary>
        /// This is a friendly name format for showing the user
        /// </summary>
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string OriginalPassword { get; set; }

        /// <summary>
        /// Thsi contains the SQL Login name to use for this ASP.NET Users when logging into sql server
        /// </summary>
        public string DatabaseLogin { get; set; }

        /// <summary>
        /// This contains the password to go with the DatabaseUser property
        /// </summary>
        public string DatabasePassword { get; set; }

    }
}