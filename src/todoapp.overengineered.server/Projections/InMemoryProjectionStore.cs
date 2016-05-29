using System.Collections.Generic;

namespace todoapp.overengineered.server.Projections
{
    public class InMemoryProjectionStore
    {
        IDictionary<string, object> _items = new Dictionary<string, object>();

        public void Add(string id, object item)
        {
            _items.Add(id, item);
        }

        public object Get(string id)
        {
            return _items[id];
        }

        public IEnumerable<dynamic> GetAll()
        {
            return _items.Values;
        }
    }
}
