using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public override string? ToString()
        {
            return (username + " " + password);
        }
    }
}
