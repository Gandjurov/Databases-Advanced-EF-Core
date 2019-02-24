using System.Collections.Generic;

namespace MiniORM
{
	// TODO: Create your DbSet class here.
    public class DbSet<T>
    {
        public List<T> Entities { get; set; }
    }

}