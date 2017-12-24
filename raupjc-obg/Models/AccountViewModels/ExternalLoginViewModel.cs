using System.ComponentModel.DataAnnotations;

namespace raupjc_obg.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}