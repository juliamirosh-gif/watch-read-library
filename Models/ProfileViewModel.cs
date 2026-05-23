namespace WatchReadLibrary.Models
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
        public List<LibraryItem> LatestItems { get; set; } = new();
        public int TotalItems { get; set; }
        public int FavoriteItems { get; set; }
        public int CompletedItems { get; set; }
        public double AverageRating { get; set; }
    }
}