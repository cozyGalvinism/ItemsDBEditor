using System.Collections.Generic;
using LiteDB;

namespace ItemDBEditor.Data {
    public class LDBItem
    {
        [BsonId]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Prefab { get; set; }
        public int Grist { get; set; }
        public string Strifekind { get; set; }
        public string Weaponsprite { get; set; }
        public bool Custom { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public int Speed { get; set; }
        public bool Spawn { get; set; }
        public List<string> Aliases { get; set; }
        public List<string> Tags { get; set; }
        public string Prototyping { get; set; }
    }

    public class LDBRecipe
    {
        public enum Methods
        {
            AND,
            OR
        }

        public LDBItem Result { get; set; }
        public string ItemA { get; set; }
        public string ItemB { get; set; }
        public Methods Method { get; set; }

        public string MethodString() {
            if(Method == Methods.AND) return "&&";
            else if(Method == Methods.OR) return "||";
            else return "??";
        }
    }
}