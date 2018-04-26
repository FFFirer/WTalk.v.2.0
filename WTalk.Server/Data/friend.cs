namespace WTalk.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wtalk.friends")]
    public partial class friend
    {
        [Key]
        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(50)]
        public string FriendId { get; set; }
    }
}
