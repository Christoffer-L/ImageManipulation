using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageManipulationApi.Repository
{
    public interface IBase<T>
    {
        public bool Add(T newObject);
        public T Get(string id);

        public Task<bool> AddAsync(T newObject);
        public Task<T> GetAsync(string id);
    }
}
