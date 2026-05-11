using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DanielTerry_Assesment1.DataStructures;
using DanielTerry_Assesment1.Models;

namespace DanielTerry_Assesment1.Services
{
    /// <summary>
    /// MovieService handles all core logic for the Movie Library system.
    ///
    /// Features:
    /// - Movie storage using Linked List (ordered collection)
    /// - Fast lookup using Hashtable (MovieId → Movie)
    /// - Search (title + ID + binary search)
    /// - Sorting (Bubble Sort by title, Merge Sort by year)
    /// - Borrow/Return system with FIFO waiting queues
    /// - Notification tracking for returned movies
    /// </summary>
    public class MovieService
    {
        private int _nextId = 1;

        private readonly MovieLinkedList _movies = new MovieLinkedList();
        private readonly Hashtable _lookup = new Hashtable();
        private readonly Dictionary<string, Queue<string>> _waitLists = new();

        public List<string> Notifications { get; } = new List<string>();

        // -------------------- ADD MOVIE --------------------

        public bool AddMovie(Movie movie)
        {
            movie.MovieId = _nextId.ToString();
            _nextId++;

            movie.IsAvailable = true;

            _movies.Add(movie);
            _lookup[movie.MovieId] = movie;

            return true;
        }

        // -------------------- GET ALL --------------------

        public List<Movie> GetAll() => _movies.GetAll().ToList();

        // -------------------- SEARCH --------------------

        public List<Movie> SearchByTitle(string title)
        {
            return _movies
                .Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Movie? SearchById(string id)
        {
            return _lookup[id] as Movie;
        }

        public Movie? BinarySearchById(List<Movie> sorted, string id)
        {
            int lo = 0, hi = sorted.Count - 1;

            while (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                int cmp = string.Compare(sorted[mid].MovieId, id, StringComparison.Ordinal);

                if (cmp == 0) return sorted[mid];
                if (cmp < 0) lo = mid + 1;
                else hi = mid - 1;
            }

            return null;
        }

        // -------------------- SORT --------------------

        public List<Movie> BubbleSortByTitle()
        {
            var list = GetAll();

            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count - 1 - i; j++)
                {
                    if (string.Compare(list[j].Title, list[j + 1].Title,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    }
                }
            }

            return list;
        }

        public List<Movie> MergeSortByYear() => MergeSort(GetAll());

        private List<Movie> MergeSort(List<Movie> list)
        {
            if (list.Count <= 1) return list;

            int mid = list.Count / 2;

            var left = MergeSort(list.Take(mid).ToList());
            var right = MergeSort(list.Skip(mid).ToList());

            return Merge(left, right);
        }

        private List<Movie> Merge(List<Movie> left, List<Movie> right)
        {
            var result = new List<Movie>();
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (left[i].ReleaseYear <= right[j].ReleaseYear)
                    result.Add(left[i++]);
                else
                    result.Add(right[j++]);
            }

            result.AddRange(left.Skip(i));
            result.AddRange(right.Skip(j));

            return result;
        }

        // -------------------- BORROW --------------------

        public string BorrowMovie(string movieId, string personName)
        {
            if (string.IsNullOrWhiteSpace(personName))
                return "Enter a valid name.";

            var movie = SearchById(movieId);

            if (movie == null)
                return "Movie not found.";

            if (movie.IsAvailable)
            {
                movie.IsAvailable = false;
                movie.BorrowedBy = personName;
                return $"'{movie.Title}' checked out to {personName}.";
            }

            if (!_waitLists.ContainsKey(movieId))
                _waitLists[movieId] = new Queue<string>();

            _waitLists[movieId].Enqueue(personName);

            return $"{personName} added to waitlist for '{movie.Title}'.";
        }

        // -------------------- RETURN --------------------

        public string ReturnMovie(string movieId)
        {
            var movie = SearchById(movieId);

            if (movie == null)
                return "Movie not found.";

            if (_waitLists.TryGetValue(movieId, out var queue) && queue.Count > 0)
            {
                string next = queue.Dequeue();

                movie.BorrowedBy = next;
                movie.IsAvailable = false;

                Notifications.Add($"'{movie.Title}' assigned to {next}");

                return $"Returned → assigned to {next}.";
            }

            movie.IsAvailable = true;
            movie.BorrowedBy = "";

            return $"'{movie.Title}' returned.";
        }

        // -------------------- HELPERS --------------------

        public Queue<string> GetWaitList(string movieId)
        {
            return _waitLists.TryGetValue(movieId, out var q)
                ? q
                : new Queue<string>();
        }

        public int Count => _movies.Count;
    }
}