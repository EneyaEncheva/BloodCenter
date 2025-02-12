using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gallery.Models.ViewModels
{
    public class PaintingViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        public SelectList? Authors { get; set; }

        [Display(Name = "Художник")]
        public int SelectedAuthor { get; set; }

        [Display(Name = "Година на издаване")]
        public int ReleaseYear { get; set; }

        public List<SelectListItem>? Genres { get; set; }

        [Display(Name = "Жанр")]
        public byte SelectedGenre { get; set; }

        public List<SelectListItem>? Styles { get; set; }

        [Display(Name = "Стил")]
        public byte SelectedStyle { get; set; }

        public List<SelectListItem>? ArtTechniques { get; set; }

        [Display(Name = "Техника на рисуване")]
        public byte SelectedArtTechnique { get; set; }

        [Display(Name = "Снимка")]
        [DataType(DataType.Upload)]
        public IFormFile Picture { get; set; }

        public List<SelectListItem>? FloorNumbers { get; set; }

        [Display(Name = "Етаж")]
        public byte SelectedFloorNumber { get; set; }

        [Display(Name = "Зала")]
        public byte HallNumber { get; set; }
    }
}
