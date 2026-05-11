using Microsoft.Win32;
using DanielTerry_Assesment1.Models;
using DanielTerry_Assesment1.Helpers;
using DanielTerry_Assesment1.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MovieLibrary
{
    /// <summary>
    /// Main WPF UI controller.
    /// Handles user interactions (buttons, search, grid actions)
    /// and communicates with MovieService for all logic.
    ///
    /// No business logic is stored here — only UI coordination.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MovieService _service = new MovieService();

        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        // Reload grid
        private void Refresh()
        {
            MovieGrid.ItemsSource = null;
            MovieGrid.ItemsSource = _service.GetAll();
        }

        // Get selected movie from grid
        private Movie? GetSelectedMovie() =>
            MovieGrid.SelectedItem as Movie;

        // Status bar helper
        private void SetStatus(string message) =>
            TxtStatus.Text = message;

        // Auto-fill borrow ID when selecting a movie
        private void MovieGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = GetSelectedMovie();
            if (selected != null)
                TxtBorrowMovieId.Text = selected.MovieId;
        }


        // ───────────────────────── ADD MOVIE ─────────────────────────
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtYear.Text.Trim(), out int year))
            {
                SetStatus("Invalid year.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTitle.Text) ||
                string.IsNullOrWhiteSpace(TxtDirector.Text))
            {
                SetStatus("Title and Director required.");
                return;
            }

            var movie = new Movie
            {
                Title = TxtTitle.Text.Trim(),
                Director = TxtDirector.Text.Trim(),
                Genre = TxtGenre.Text.Trim(),
                ReleaseYear = year,
                IsAvailable = true
            };

            _service.AddMovie(movie);
            SetStatus($"Added '{movie.Title}' (ID {movie.MovieId})");

            TxtTitle.Clear();
            TxtDirector.Clear();
            TxtGenre.Clear();
            TxtYear.Clear();

            Refresh();
        }


        // ───────────────────────── SEARCH ─────────────────────────
        private void BtnSearchTitle_Click(object sender, RoutedEventArgs e)
        {
            var results = _service.SearchByTitle(TxtSearch.Text.Trim());
            MovieGrid.ItemsSource = results;
            SetStatus($"{results.Count} result(s).");
        }

        private void BtnSearchId_Click(object sender, RoutedEventArgs e)
        {
            var movie = _service.SearchById(TxtSearch.Text.Trim());

            MovieGrid.ItemsSource = movie != null
                ? new List<Movie> { movie }
                : new List<Movie>();

            SetStatus(movie != null ? "Movie found." : "Not found.");
        }

        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            SetStatus("All movies shown.");
        }


        // ───────────────────────── SORT ─────────────────────────
        private void BtnSortTitle_Click(object sender, RoutedEventArgs e)
        {
            MovieGrid.ItemsSource = _service.BubbleSortByTitle();
            SetStatus("Sorted by title.");
        }

        private void BtnSortYear_Click(object sender, RoutedEventArgs e)
        {
            MovieGrid.ItemsSource = _service.MergeSortByYear();
            SetStatus("Sorted by year.");
        }


        // ───────────────────────── BORROW ─────────────────────────
        private void BtnBorrow_Click(object sender, RoutedEventArgs e)
        {
            string id = TxtBorrowMovieId.Text.Trim();
            string name = TxtBorrowName.Text.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                SetStatus("Enter ID and Name.");
                return;
            }

            SetStatus(_service.BorrowMovie(id, name));
            Refresh();
        }


        // ───────────────────────── RETURN ─────────────────────────
        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            string id = TxtBorrowMovieId.Text.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                SetStatus("Enter Movie ID.");
                return;
            }

            SetStatus(_service.ReturnMovie(id));

            if (_service.Notifications.Count > 0)
            {
                MessageBox.Show(_service.Notifications[^1],
                    "Notification",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            Refresh();
        }


        // ───────────────────────── FILE I/O ─────────────────────────
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "JSON|*.json" };

            if (dlg.ShowDialog() == true)
            {
                FileHelper.ExportJson(_service.GetAll(), dlg.FileName);
                SetStatus("Exported.");
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "JSON|*.json" };

            if (dlg.ShowDialog() == true)
            {
                var movies = FileHelper.ImportJson(dlg.FileName);

                foreach (var m in movies)
                    _service.AddMovie(m);

                Refresh();
                SetStatus("Imported movies.");
            }
        }


        // ───────────────────────── NOTIFICATIONS EXPORT ─────────────────────────
        private void BtnExportNotifications_Click(object sender, RoutedEventArgs e)
        {
            if (_service.Notifications.Count == 0)
            {
                SetStatus("No notifications.");
                return;
            }

            var dlg = new SaveFileDialog { Filter = "JSON|*.json" };

            if (dlg.ShowDialog() == true)
            {
                FileHelper.ExportNotifications(_service.Notifications, dlg.FileName);
                SetStatus("Notifications exported.");
            }
        }
    }
}