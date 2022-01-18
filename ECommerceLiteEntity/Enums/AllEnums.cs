using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Enums
{
    public class AllEnums
    {

    }

    public enum TheIdentityRoles : byte
    {
        Passive=0,
        Admin=1,
        Customer=2,
        Supplier=3, //tedarikçi
        Editor=4,   //içerik yöneticisi
        Active=5
    }
}
