namespace DanielTerry_Assesment1.Models
{
    /// <summary>
    /// Represents a single movie record in the library.
    /// Each movie has a unique auto-assigned ID and tracks borrowing state.
    /// </summary>
    public class Movie
    {
        // Unique identifier — auto-assigned by MovieService (starts at 1, increments)
        public string MovieId { get; set; } = "";

        // Core descriptive fields required by the brief
        public required string Title { get; set; }
        public required string Director { get; set; }
        public required string Genre { get; set; }
        public required int ReleaseYear { get; set; }

        // Availability flag — true = on the shelf, false = checked out
        public bool IsAvailable { get; set; } = true;

        // Name of the person currently holding the movie (empty when available)
        public string BorrowedBy { get; set; } = "";
    }
}