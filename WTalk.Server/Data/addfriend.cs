namespace WTalk.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wtalk.addfriends")]
    public partial class addfriend
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(45)]
        public string TIme { get; set; }

        [StringLength(45)]
        public string SenderId { get; set; }

        [StringLength(45)]
        public string ReceiverId { get; set; }

        [Column(TypeName = "binary")]
        public byte[] Status { get; set; }
    }
}
