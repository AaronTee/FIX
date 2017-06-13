using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIX.Data.Mapping
{
    public class MenuMap : EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            ////key
            //HasKey(t => t.ID);
            ////properties
            //Property(t => t.Category).IsRequired();
            //Property(t => t.Name).IsRequired();
            //Property(t => t.Description).IsOptional();
            ////table
            //ToTable("Menu");
        }
    }
}