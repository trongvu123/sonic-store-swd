using Microsoft.AspNetCore.Mvc;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Dtos
{
    public class CompositeViewModel
    {
        [BindProperty]
        public Account AccountModel { get; set; }
        public User UserModel { get; set; }
        public UserAddress UserAddress { get; set; }
    }
}
