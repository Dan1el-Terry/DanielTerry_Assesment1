using Xunit;
using DanielTerry_Assesment1.Models;
using DanielTerry_Assesment1.Services;

namespace DanielTerry_Assesment1.Tests
{
    public class MovieServiceTests
    {
        [Fact]
        public void AddMovie_ShouldIncrementId()
        {
            var service = new MovieService();
            var movie = new Movie { Title = "Test", Director = "Dir", Genre = "Action", ReleaseYear = 2020 };
            
            service.AddMovie(movie);
            
            Assert.Equal("1", movie.MovieId);
        }

        [Fact]
        public void SearchById_ShouldReturnMovie()
        {
            var service = new MovieService();
            var movie = new Movie { Title = "Inception", Director = "Nolan", Genre = "Sci-Fi", ReleaseYear = 2010 };
            service.AddMovie(movie);
            
            var result = service.SearchById("1");
            
            Assert.NotNull(result);
            Assert.Equal("Inception", result.Title);
        }

        [Fact]
        public void BorrowMovie_WhenAvailable_ShouldMarkAsBorrowed()
        {
            var service = new MovieService();
            var movie = new Movie { Title = "Avatar", Director = "Cameron", Genre = "Sci-Fi", ReleaseYear = 2009 };
            service.AddMovie(movie);
            
            var result = service.BorrowMovie("1", "John");
            
            var borrowed = service.SearchById("1");
            Assert.False(borrowed.IsAvailable);
            Assert.Equal("John", borrowed.BorrowedBy);
        }

        [Fact]
        public void BorrowMovie_WhenUnavailable_ShouldAddToQueue()
        {
            var service = new MovieService();
            var movie = new Movie { Title = "Avatar", Director = "Cameron", Genre = "Sci-Fi", ReleaseYear = 2009 };
            service.AddMovie(movie);
            
            service.BorrowMovie("1", "John");
            var result = service.BorrowMovie("1", "Jane");
            
            Assert.Contains("queue", result.ToLower());
        }

        [Fact]
        public void ReturnMovie_WithWaitingQueue_ShouldAssignToNext()
        {
            var service = new MovieService();
            var movie = new Movie { Title = "Avatar", Director = "Cameron", Genre = "Sci-Fi", ReleaseYear = 2009 };
            service.AddMovie(movie);
            
            service.BorrowMovie("1", "John");
            service.BorrowMovie("1", "Jane");
            var result = service.ReturnMovie("1");
            
            var updated = service.SearchById("1");
            Assert.Equal("Jane", updated.BorrowedBy);
            Assert.False(updated.IsAvailable);
        }

        [Fact]
        public void SearchByTitle_ShouldFindPartialMatches()
        {
            var service = new MovieService();
            service.AddMovie(new Movie { Title = "Inception", Director = "Nolan", Genre = "Sci-Fi", ReleaseYear = 2010 });
            service.AddMovie(new Movie { Title = "Interstellar", Director = "Nolan", Genre = "Sci-Fi", ReleaseYear = 2014 });
            
            var results = service.SearchByTitle("Inter");
            
            Assert.Single(results);
            Assert.Equal("Interstellar", results[0].Title);
        }

        [Fact]
        public void BubbleSortByTitle_ShouldSortAlphabetically()
        {
            var service = new MovieService();
            service.AddMovie(new Movie { Title = "Zulu", Director = "D1", Genre = "Action", ReleaseYear = 2010 });
            service.AddMovie(new Movie { Title = "Avatar", Director = "D2", Genre = "Sci-Fi", ReleaseYear = 2009 });
            
            var sorted = service.BubbleSortByTitle();
            
            Assert.Equal("Avatar", sorted[0].Title);
            Assert.Equal("Zulu", sorted[1].Title);
        }

        [Fact]
        public void MergeSortByYear_ShouldSortChronologically()
        {
            var service = new MovieService();
            service.AddMovie(new Movie { Title = "Inception", Director = "Nolan", Genre = "Sci-Fi", ReleaseYear = 2010 });
            service.AddMovie(new Movie { Title = "Avatar", Director = "Cameron", Genre = "Sci-Fi", ReleaseYear = 2009 });
            
            var sorted = service.MergeSortByYear();
            
            Assert.Equal(2009, sorted[0].ReleaseYear);
            Assert.Equal(2010, sorted[1].ReleaseYear);
        }

        [Fact]
        public void BinarySearchById_ShouldFindSortedMovie()
        {
            var service = new MovieService();
            service.AddMovie(new Movie { Title = "A", Director = "D1", Genre = "Action", ReleaseYear = 2010 });
            service.AddMovie(new Movie { Title = "B", Director = "D2", Genre = "Drama", ReleaseYear = 2011 });
            var sorted = service.GetAll().OrderBy(m => m.MovieId).ToList();
            
            var result = service.BinarySearchById(sorted, "1");
            
            Assert.NotNull(result);
            Assert.Equal("A", result.Title);
        }
    }
}