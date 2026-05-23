using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchReadLibrary.Models
{
    public class LibraryItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть назву")]
        [StringLength(120, MinimumLength = 1,
            ErrorMessage = "Назва повинна містити від 1 до 120 символів")]
        [Display(Name = "Назва")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть категорію")]
        [StringLength(50)]
        [Display(Name = "Категорія")]
        public string Category { get; set; } = string.Empty;

        [StringLength(2500,
            ErrorMessage = "Опис не може перевищувати 2500 символів")]
        [Display(Name = "Опис")]
        public string Description { get; set; } = string.Empty;

        [StringLength(2500,
            ErrorMessage = "Відгук не може перевищувати 2500 символів")]
        [Display(Name = "Відгук")]
        public string Review { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? CoverFile { get; set; }

        [Range(0, 10,
            ErrorMessage = "Рейтинг повинен бути від 0 до 10")]
        [Display(Name = "Рейтинг")]
        public int Rating { get; set; }

        [Range(1800, 2100,
            ErrorMessage = "Некоректний рік")]
        [Display(Name = "Рік")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Оберіть статус")]
        [StringLength(50)]
        [Display(Name = "Статус")]
        public string Status { get; set; } = string.Empty;

        [Range(0, 10000,
            ErrorMessage = "Поточний прогрес не може бути від’ємним")]
        [Display(Name = "Поточний прогрес")]
        public int? CurrentProgress { get; set; }

        [Range(0, 10000,
            ErrorMessage = "Загальна кількість має бути додатною")]
        [Display(Name = "Загальна кількість")]
        public int? TotalProgress { get; set; }

        [StringLength(30)]
        [Display(Name = "Тип прогресу")]
        public string ProgressType { get; set; } = string.Empty;

        [Display(Name = "Обкладинка")]
        public string? CoverPath { get; set; }

        [Display(Name = "Улюблене")]
        public bool IsFavorite { get; set; }

        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}