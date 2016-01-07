using System;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Metadata.Exceptions;
using EntityFramework.Metadata.Extensions;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;


namespace EntityFramework.Metadata.Mappers
{
    internal class DbFirstMapper : MapperBase
    {
        public DbFirstMapper(MetadataWorkspace metadataWorkspace, EntityContainer entityContainer)
            : base(metadataWorkspace, entityContainer)
        {
        }

        protected override PrepareMappingRes PrepareMapping(string typeFullName, EdmType edmItem)
        {
            string tableName = GetTableName(typeFullName);

            // find existing parent storageEntitySet
            // thp derived types does not have storageEntitySet
            EntitySet storageEntitySet;
            EdmType baseEdmType = edmItem;
            while (!EntityContainer.TryGetEntitySetByName(tableName, false, out storageEntitySet))
            {
                if (baseEdmType.BaseType == null)
                {
                    break;
                }
                baseEdmType = baseEdmType.BaseType;
            }

            if (storageEntitySet == null)
            {
                return null;
            }

            var isRoot = baseEdmType == edmItem;
            if (!isRoot)
            {
                var parent = _entityMaps.Values.FirstOrDefault(x => x.EdmType == baseEdmType);
                // parent table has not been mapped yet
                if (parent == null)
                {
                    throw new ParentNotMappedYetException();
                }
            }

            return new PrepareMappingRes { TableName = tableName, StorageEntitySet = storageEntitySet, IsRoot = isRoot, BaseEdmType = baseEdmType };
        }
        
        protected string GetTableName(string typeFullName)
        {
            // Get the entity type from the model that maps to the CLR type
            var entityType = MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).Single(e => e.FullName == typeFullName);
            
            // Get the entity set that uses this entity type
            var entitySet = MetadataWorkspace
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);
            
            var entitySetMappings = (IEnumerable<object>)MetadataWorkspace.GetItems(DataSpace.CSSpace)
                .First()
                .GetPrivateFieldValue("EntitySetMappings");

            object mapping = entitySetMappings
                .FirstOrDefault(map => map.GetPrivateFieldValue("EntitySet") == entitySet);


            object entityTypeMapping = ((IEnumerable<object>)mapping.GetPrivateFieldValue("EntityTypeMappings")).FirstOrDefault();
            var tables = (IEnumerable<object>)entityTypeMapping.GetPrivateFieldValue("MappingFragments");

            return (string)tables.FirstOrDefault().GetPrivateFieldValue("Table").GetPrivateFieldValue("Name");
        }
    }
}