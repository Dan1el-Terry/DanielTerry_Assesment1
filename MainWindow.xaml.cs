using Microsoft.Win32;
using WpfApp1.ProjectFiles.Helpers;
using WpfApp1.ProjectFiles.Models;
using WpfApp1.ProjectFiles.Services;
using System.Collections.Generic;
using System.Windows;

namespace MovieLibrary
{
    public partial class MainWindow : Window
    {
        private readonly MovieService _service = new MovieService();

        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            MovieGrid.ItemsSource = null;
            MovieGrid.ItemsSource = _service.GetAll();
        }

        private Movie GetSelectedMovie()
        {
            return MovieGrid.SelectedItem as Movie;
        }

        // ADD MOVIE
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtYear.Text, out int year))
            {
                TxtStatus.Text = "Invalid year.";
                return;
            }

            var movie = new Movie
            {
                MovieId = TxtId.Text,
                Title = TxtTitle.Text,
                Director = TxtDirector.Text,
                Genre = TxtGenre.Text,
                ReleaseYear = year,
                IsAvailable = true
            };

            TxtStatus.Text = _service.AddMovie(movie)
                ? "Movie added."
                : "Duplicate ID!";

            Refresh();
        }

        // SEARCH
        private void BtnSearchTitle_Click(object sender, RoutedEventArgs e)
        {
            MovieGrid.ItemsSource = _service.SearchByTitle(TxtSearch.Text);
        }

        private void BtnSearchId_Click(object sender, RoutedEventArgs e)
        {
            var movie = _service.SearchById(TxtSearch.Text);
            MovieGrid.ItemsSource = movie != null ? new List<Movie> { movie } : new List<Movie>();
        }

        // SORT
        private void BtnSortTitle_Click(object sender, RoutedEventArgs e)
        {
            MovieGrid.ItemsSource = _service.BubbleSortByTitle();
        }

        private void BtnSortYear_Click(object sender, RoutedEventArgs e)
        {
            MovieGrid.ItemsSource = _service.MergeSortByYear();
        }

        // BORROW
        private void BtnBorrow_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedMovie();

            if (selected == null)
            {
                TxtStatus.Text = "Select a movie first.";
                return;
            }

            var result = _service.BorrowMovie(selected.MovieId, TxtUser.Text);
            TxtStatus.Text = result;

            Refresh();
        }

        // RETURN
        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedMovie();

            if (selected == null)
            {
                TxtStatus.Text = "Select a movie first.";
                return;
            }

            var result = _service.ReturnMovie(selected.MovieId);
            TxtStatus.Text = result;

            Refresh();
        }

        // EXPORT
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "JSON|*.json" };

            if (dlg.ShowDialog() == true)
            {
                FileHelper.ExportJson(_service.GetAll(), dlg.FileName);
                TxtStatus.Text = "Exported successfully.";
            }
        }

        // IMPORT
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "JSON|*.json" };

            if (dlg.ShowDialog() == true)
            {
                var movies = FileHelper.ImportJson(dlg.FileName);

                foreach (var m in movies)
                    _service.AddMovie(m);

                Refresh();
                TxtStatus.Text = "Import complete.";
            }
        }
    }
}