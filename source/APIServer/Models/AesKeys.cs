using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIServer.Models
{
    public class AesKeys
    {
        [PrimaryKey]
        [AutoIncrement]
        public int AesKeyID { get; set; }

        [References(typeof(Conversations))]
        public int ConversationID { get; set; }

        [References(typeof(Users))]
        public int UserID { get; set; }

        [Column(TypeName = "varbinary(32)")]
        public byte[] EncryptedAesKey { get; set; }

        [Column(TypeName = "varbinary(16)")]
        public byte[] IV { get; set; }
    }
}
