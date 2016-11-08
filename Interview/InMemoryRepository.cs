using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interview
{
    class InMemoryRepository : IRepository<T>
    {
        List<T> _db = new List<T>();
 
        public void All() {
            return _db;
        }
    }
}
