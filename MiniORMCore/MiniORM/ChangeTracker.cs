using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniORM
{
	// TODO: Create your ChangeTracker class here.
    internal class ChangeTracker<T>
        where T : class, new()
    {
        private readonly List<T> allEntities;
        private readonly List<T> removed;
        private readonly List<T> added;

        public ChangeTracker(IEnumerable<T> entities)
        {
            this.removed = new List<T>();
            this.added = new List<T>();

            this.allEntities = CloneEntities(entities);
        }

        public IReadOnlyCollection<T> Added
            => this.added.AsReadOnly();

        public IReadOnlyCollection<T> Removed
            => this.removed.AsReadOnly();

        public IReadOnlyCollection<T> AllEntities
            => this.allEntities.AsReadOnly();


        private List<T> CloneEntities(IEnumerable<T> entities)
        {
            var clonedEntities = new List<T>();

            var propertiesToClone = typeof(T).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            foreach (var entity in entities)
            {
                var clonedEntity = Activator.CreateInstance<T>();

                foreach (var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(clonedEntity, value);
                }

                clonedEntities.Add(clonedEntity);
            }

            return clonedEntities;
        }
    }

}