using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WpfApp1.ProjectFiles.DataStructures;
namespace WpfApp1.ProjectFiles.Models
{
    public class Movie
    {
        public required string MovieId { get; set; }
        public required string Title { get; set; }
        public required string Director { get; set; }
        public required string Genre { get; set; }
        public required int ReleaseYear { get; set; }

        // Single, correct declaration (initialize to true/false as needed)
        public bool IsAvailable { get; set; } = true;
    }
}