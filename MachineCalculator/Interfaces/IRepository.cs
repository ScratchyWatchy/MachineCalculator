using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace UserDBWebRest
{
    public interface IRepository<T> where T : EntityBase
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        int Create(T entity);
        void Delete(int id);
        void Update(T entity);
    }
}
