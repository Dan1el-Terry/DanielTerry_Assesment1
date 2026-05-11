using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DanielTerry_Assesment1.DataStructures;
using DanielTerry_Assesment1.Models;

namespace DanielTerry_Assesment1.Services
{
    /// <summary>
    /// MovieService manages all core movie operations for the system.
    /// It stores movies using a Linked List for ordered storage and a Hashtable for fast MovieId lookup.
    /// It supports searching, sorting, borrowing/returning movies, and managing a FIFO waitlist queue
    /// when movies are unavailable.
    /// </summary>
    public class MovieService
    {
        private int _nextId = 1;

        private readonly MovieLinkedList _movies = new MovieLinkedList();
        private readonly Hashtable _lookup = new Hashtable();
        private readonly Dictionary<string, Queue<string>> _waitLists = new();

        public List<string> Notifications { get; } = new();

        // ---------------- ADD MOVIE ----------------

        public bool AddMovie(Movie movie)
        {
            if (string.IsNullOrEmpty(movie.MovieId))
                movie.MovieId = _nextId.ToString();

            _nextId++;

            movie.IsAvailable = true;

            _movies.Add(movie);
            _lookup[movie.MovieId] = movie;

            return true;
        }

        public List<Movie> GetAll() => _movies.GetAll().ToList();

        public int Count => _movies.Count;

        // ---------------- SEARCH ----------------

        public List<Movie> SearchByTitle(string title)
        {
            return _movies
                .Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Movie? SearchById(string id)
        {
            return _lookup.ContainsKey(id) ? _lookup[id] as Movie : null;
        }

        // ---------------- BORROW ----------------

        public string BorrowMovie(string movieId, string personName)
        {
            var movie = SearchById(movieId);

            if (movie == null)
                return "movie not found";

            if (movie.IsAvailable)
            {
                movie.IsAvailable = false;
                movie.BorrowedBy = personName;

                return "borrowed";
            }

            if (!_waitLists.ContainsKey(movieId))
                _waitLists[movieId] = new Queue<string>();

            _waitLists[movieId].Enqueue(personName);

            return "added to queue";
        }

        // ---------------- RETURN ----------------

        public string ReturnMovie(string movieId)
        {
            var movie = SearchById(movieId);

            if (movie == null)
                return "movie not found";

            if (_waitLists.TryGetValue(movieId, out var queue) && queue.Count > 0)
            {
                string next = queue.Dequeue();

                movie.BorrowedBy = next;
                movie.IsAvailable = false;

                Notifications.Add($"assigned to {next}");

                return "assigned to next user";
            }

            movie.IsAvailable = true;
            movie.BorrowedBy = "";

            return "returned";
        }

        // ---------------- QUEUE ----------------

        public Queue<string> GetWaitList(string movieId)
        {
            return _waitLists.TryGetValue(movieId, out var q)
                ? q
                : new Queue<string>();
        }
        public List<Movie> BubbleSortByTitle()
        {
            var list = GetAll();

            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count - 1 - i; j++)
                {
                    if (string.Compare(list[j].Title, list[j + 1].Title, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    }
                }
            }

            return list;
        }
        public List<Movie> MergeSortByYear()
        {
            return MergeSort(GetAll());
        }

        private List<Movie> MergeSort(List<Movie> list)
        {
            if (list.Count <= 1)
                return list;

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
    }

}