using HappyTools.Domain.Entities.Audit.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HappyTools.EfCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyIEntityPrimaryKeys(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IEntity<>).IsAssignableFrom(entity.ClrType))
                    continue;

                var ientityInterface = entity.ClrType
                    .GetInterfaces()
                    .FirstOrDefault(x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(IEntity<>));

                if (ientityInterface == null)
                    continue;

                var keyPropName = "Id";
                var keyProp = entity.FindProperty(keyPropName);

                if (keyProp == null)
                {
                    keyProp = entity.AddProperty(keyPropName, ientityInterface.GetGenericArguments()[0]);
                }

                entity.SetPrimaryKey(keyProp);
            }
        }
    }
}