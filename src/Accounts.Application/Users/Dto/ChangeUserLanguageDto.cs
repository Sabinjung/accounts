using System.ComponentModel.DataAnnotations;

namespace Accounts.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}