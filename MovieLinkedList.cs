using System.Collections;
using System.Collections.Generic;
using WpfApp1.ProjectFiles.Models;

namespace WpfApp1.ProjectFiles.DataStructures
{
    public class MovieLinkedList : IEnumerable<Movie>
    {
        private LinkedList<Movie> _list = new LinkedList<Movie>();

        public void Add(Movie movie) => _list.AddLast(movie);
        public bool Remove(Movie movie) => _list.Remove(movie);
        public IEnumerable<Movie> GetAll() => _list;
        public int Count => _list.Count;

        public IEnumerator<Movie> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}