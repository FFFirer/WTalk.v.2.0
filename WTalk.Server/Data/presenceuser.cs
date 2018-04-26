namespace WTalk.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wtalk.presenceusers")]
    public partial class presenceuser
    {
        [Key]
        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(50)]
        public string PresenceTime { get; set; }

        [StringLength(50)]
        public string IPAdderss { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}
