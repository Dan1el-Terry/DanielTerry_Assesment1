using System.Collections;
using System.Collections.Generic;
using DanielTerry_Assesment1.Models;

namespace DanielTerry_Assesment1.DataStructures
{
    /// <summary>
    /// The brief requires a Linked List as the primary movie collection — this satisfies that.
    /// Using a wrapper means we can swap the internal structure later without touching service code.
    /// </summary>
    public class MovieLinkedList : IEnumerable<Movie>
    {
        // The actual .NET doubly-linked list that stores movies in order
        private readonly LinkedList<Movie> _list = new LinkedList<Movie>();

        /// <summary>Appends a movie to the tail of the list. O(1) for LinkedList.</summary>
        public void Add(Movie movie) => _list.AddLast(movie);

        /// <summary>Removes the first node whose reference matches. O(n)</summary>
        public bool Remove(Movie movie) => _list.Remove(movie);

        /// <summary>Returns all movies as an enumerable (no copy). Used by service layer.</summary>
        public IEnumerable<Movie> GetAll() => _list;

        /// <summary>Number of movies currently stored.</summary>
        public int Count => _list.Count;

        // --- IEnumerable implementation so LINQ works directly on the list ---
        public IEnumerator<Movie> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}