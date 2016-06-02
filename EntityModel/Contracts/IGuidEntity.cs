using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.Contracts
{
    public interface IGuidEntity
    {
        Guid Id { get; set; }
    }
}
