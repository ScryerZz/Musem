//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Musem.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Curator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Curator()
        {
            this.Curator_Exhibition = new HashSet<Curator_Exhibition>();
        }
    
        public int Id_Curator { get; set; }
        public Nullable<int> Id_User { get; set; }
        public string ContactInfo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Curator_Exhibition> Curator_Exhibition { get; set; }
        public virtual User User { get; set; }
    }
}