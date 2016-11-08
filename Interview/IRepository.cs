using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview
{
    // Please create an in memory implementation of IRepository<T> 

    public interface IRepository<T> where T : IStoreable
    {
        void Save(T item);
        void Update(T item);
        void Delete(IComparable id);
        IEnumerable<T> All();
        T FindById(IComparable id);
        void Commmit();
        void RollBack();
    }

    public class InMemoryRepository<T> : IRepository<T> where T : IStoreable
    {
        public Dictionary<Guid, T> dbRepository = new Dictionary<Guid, T>();
        private Dictionary<Guid, T> itemsToAdd = new Dictionary<Guid, T>();
        private Dictionary<Guid, T> itemsToUpdate = new Dictionary<Guid, T>();
        private Dictionary<Guid, IComparable> itemsToDelete = new Dictionary<Guid, IComparable>();

        public void Save(T item)
        {
            itemsToAdd.Add(Guid.NewGuid(), item);
        }

        public void Delete(IComparable id)
        {
            itemsToDelete.Add(Guid.NewGuid(), id);
        }

        public void Update(T item)
        {
            itemsToUpdate.Add(Guid.NewGuid(), item);
        }

        public IEnumerable<T> All()
        {
            return dbRepository.Values;
        }

        public T FindById(IComparable id)
        {
            T selectedItem = dbRepository.Values.Where(a => a.Id == id).SingleOrDefault();
            return selectedItem;
        }

        public void RollBack()
        {
            itemsToAdd.Clear();
            itemsToUpdate.Clear();
            itemsToDelete.Clear();
        }

        public void Commmit() {
            Dictionary<Guid, T> backup = dbRepository;
            try
            {
                //ADD
                if (itemsToAdd.Count() > 0)
                {
                    foreach (KeyValuePair<Guid, T> item in itemsToAdd)
                    {
                        dbRepository.Add(item.Key, item.Value);
                    };
                };
                //UPDATE
                if (itemsToUpdate.Count() > 0)
                {
                    foreach (KeyValuePair<Guid, T> item in itemsToUpdate)
                    {
                        KeyValuePair<Guid, T> selecteditem = dbRepository.Where(a => a.Value.Id == item.Value.Id).SingleOrDefault();
                        if (!selecteditem.Equals(default(KeyValuePair<Guid, T>)))
                        {
                            dbRepository.Remove(selecteditem.Key);
                            dbRepository.Add(item.Key, item.Value);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                };
                //DELETE
                if (itemsToDelete.Count() > 0)
                {
                    foreach (KeyValuePair<Guid, IComparable> item in itemsToDelete)
                    {
                        KeyValuePair<Guid, T> selecteditem = dbRepository.Where(a => a.Value.Id == item.Value).SingleOrDefault();
                        if (!selecteditem.Equals(default(KeyValuePair<Guid, T>)))
                        {
                            dbRepository.Remove(selecteditem.Key);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                };
                //EMPTY Cache
                RollBack();
            }
            catch (Exception e)
            {
                //restore List
                dbRepository = backup;
                throw e;
            }


        }

    }


}
