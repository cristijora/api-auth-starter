using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Contracts;

namespace Repository.Contracts
{
    public interface IQueryFilter<T> where T : IGuidEntity
    {
        IQueryable<T> FilterQueryable(IQueryable<T> queryabable);
    }
}
