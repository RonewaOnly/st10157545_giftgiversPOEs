using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace st10157545_giftgiversPOEs.Models
{
    public class CreateEventViewModel
    {
        public Events Event { get; set; }

        // AdminList should not be required for form submission, so ensure it's not bound or validated
        [BindNever]
        public List<SelectListItem> AdminList { get; set; } = new List<SelectListItem>();

        // AdminError is used only for display purposes, so it should not be bound or validated
        [BindNever]
        public string AdminError { get; set; } = "No error";


    }
}
