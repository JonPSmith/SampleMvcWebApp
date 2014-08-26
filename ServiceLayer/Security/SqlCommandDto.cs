using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Security
{
    public class SqlCommandDto
    {
        /// <summary>
        /// This is the optinal prefix that can be applied to the commands 
        /// </summary>
        public string LoginPrefix { get; set; }

        /// <summary>
        /// Name of file to read or write
        /// </summary>
        [Required]
        [RegularExpression("\\.", ErrorMessage = "You cannot include a dot or an ending filetype")]
        public string Filename { get; set; }

        /// <summary>
        /// This contains a password that people have to provide for some of the methods
        /// </summary>
        public string  Password { get; set; }

        //-----------------------------------------------------
        //items to be set by the action

        /// <summary>
        /// The filepath to the App_Data directory
        /// </summary>
        [ScaffoldColumn(false)]
        public string AppDataFilePath { get; set; }

        /// <summary>
        /// This is what the hashed password should equal to pass the test
        /// </summary>
        [ScaffoldColumn(false)]
        public string HashedPassword { get; set; }

    }
}
