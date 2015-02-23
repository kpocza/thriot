using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace IoT.Framework.Sql
{
    public class ForeignKeyNamingConvention : IStoreModelConvention<AssociationType>
    {
        public void Apply(AssociationType association, DbModel model)
        {
            if (!association.IsForeignKey) 
                return;

            var constraint = association.Constraint;
            if (DoPropertiesHaveDefaultNames(constraint.FromProperties, constraint.ToProperties))
            {
                NormalizeForeignKeyProperties(constraint.FromProperties);
            }

            if (DoPropertiesHaveDefaultNames(constraint.ToProperties, constraint.FromProperties))
            {
                NormalizeForeignKeyProperties(constraint.ToProperties);
            }
        }

        private static bool DoPropertiesHaveDefaultNames(IReadOnlyList<EdmProperty> properties, IReadOnlyList<EdmProperty> otherEndProperties)
        {
            if (properties.Count != otherEndProperties.Count)
                return false;

            for (var i = 0; i < properties.Count; ++i)
            {
                if (!properties[i].Name.EndsWith("_" + otherEndProperties[i].Name))
                {
                    return false;
                }
            }

            return true;
        }

        private static void NormalizeForeignKeyProperties(IEnumerable<EdmProperty> properties)
        {
            foreach (EdmProperty property in properties)
            {
                var defaultPropertyName = property.Name;
                var ichUnderscore = defaultPropertyName.IndexOf('_');
                if (ichUnderscore <= 0)
                    continue;

                var navigationPropertyName = defaultPropertyName.Substring(0, ichUnderscore);
                var targetKey = defaultPropertyName.Substring(ichUnderscore + 1);

                string newPropertyName;
                if (targetKey.StartsWith(navigationPropertyName))
                {
                    newPropertyName = targetKey;
                }
                else
                {
                    newPropertyName = navigationPropertyName + targetKey;
                }

                property.Name = newPropertyName;
            }
        }
    }
}
