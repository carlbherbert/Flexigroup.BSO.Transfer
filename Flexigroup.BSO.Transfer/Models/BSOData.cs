using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flexigroup.BSO.Transfer.Models
{
    [Table("BSOData")]
    public class BsoData
    {
        public BsoData()
        {
            Created = DateTimeOffset.Now;
        }

        [Required]
        public long Id { get; set; }
        [Required]
        public Guid? CorrelationId { get; set; }
        public byte[] CustomerHash { get; set; }
        public string CustomerSource { get; set; }
        public string Json { get; set; }
        public byte[] JsonIV { get; set; }
        public byte[] JsonEncrypted { get; set; }

        public string CallbackURL { get; set; }
        public string CallbackVerb { get; set; }
        public string CallbackContentType { get; set; }

        public DateTimeOffset? Received { get; set; }
        [Required]
        public DateTimeOffset Created { get; set; }
    }
}
