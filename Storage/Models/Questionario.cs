using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models
{
    public class Questionario
    {
        public Questionario() {
            Risposte = new Dictionary<string,string>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public Dictionary<string,string> Risposte {get;set;}


    }
}
