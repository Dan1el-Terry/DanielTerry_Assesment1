using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DanielTerry_Assesment1.DataStructures;
using DanielTerry_Assesment1.Models;

namespace DanielTerry_Assesment1.Services
{
    public class MovieService
    {
        private int _nextId = 1;
        private MovieLinkedList _movies = new MovieLinkedList();
        private Hashtable _lookup = new Hashtable();                  // MovieID -> Movie
        private Dictionary<string, Queue<string>> _waitingQueues = new(); // MovieID -> user queue
        public List<string> Notifications = new List<string>();

        // --- Add / Remove ---

        public bool AddMovie(Movie movie)
        {
            movie.MovieId = _nextId.ToString();
            _nextId++;

            _movies.Add(movie);
            return true;
        }

        public List<Movie> GetAll() => _movies.GetAll().ToList();

        // --- Search ---

        public List<Movie> SearchByTitle(string title) =>
            _movies.Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

        public Movie SearchById(string id) => _lookup[id] as Movie;

        // Binary search by ID (list must be sorted by ID first)
        public Movie BinarySearchById(List<Movie> sorted, string id)
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

        // --- Sort ---

        public List<Movie> BubbleSortByTitle()
        {
            var list = GetAll();
            for (int i = 0; i < list.Count - 1; i++)
                for (int j = 0; j < list.Count - 1 - i; j++)
                    if (string.Compare(list[j].Title, list[j + 1].Title) > 0)
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
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
                if (left[i].ReleaseYear <= right[j].ReleaseYear) result.Add(left[i++]);
                else result.Add(right[j++]);
            }
            result.AddRange(left.Skip(i));
            result.AddRange(right.Skip(j));
            return result;
        }

        // --- Borrow / Return ---

        public string BorrowMovie(string movieId, string userName)
        {
            if (_lookup[movieId] is not Movie movie) return "Movie not found.";
            if (movie.IsAvailable)
            {
                movie.IsAvailable = false;
                return $"'{movie.Title}' borrowed by {userName}.";
            }
            if (!_waitingQueues.ContainsKey(movieId))
                _waitingQueues[movieId] = new Queue<string>();
            _waitingQueues[movieId].Enqueue(userName);
            return $"'{movie.Title}' unavailable. {userName} added to waiting list.";
        }

        public string ReturnMovie(string movieId)
        {
            if (_lookup[movieId] is not Movie movie) return "Movie not found.";
            movie.IsAvailable = true;
            if (_waitingQueues.TryGetValue(movieId, out var queue) && queue.Count > 0)
            {
                string next = queue.Dequeue();
                movie.IsAvailable = false;
                string msg = $"'{movie.Title}' auto-assigned to {next}.";
                Notifications.Add(msg);
                return msg;
            }
            return $"'{movie.Title}' is now available.";
        }
    }
}