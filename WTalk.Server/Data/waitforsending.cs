namespace WTalk.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wtalk.waitforsendings")]
    public partial class waitforsending
    {
        [Key]
        public int MsgId { get; set; }

        [StringLength(50)]
        public string SendTime { get; set; }

        [StringLength(50)]
        public string SenderId { get; set; }

        [StringLength(50)]
        public string ReceiverId { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Msg { get; set; }
    }
}
