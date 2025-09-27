using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data
{
    public record registerDto
    {
        [Required(ErrorMessage ="Username zorunludur")]
        public string userName { get; set; }

        [Required(ErrorMessage = "password zorunludur")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "password zorunludur")]
        [Compare("password",ErrorMessage ="Şifreler uyuşmuyor")]
        [DataType(DataType.Password)]
        public string retypepassword { get; set; }
    }
}
