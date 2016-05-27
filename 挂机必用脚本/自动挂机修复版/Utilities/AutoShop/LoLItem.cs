namespace AutoBuddy.Utilities.AutoShop
{
    public class LoLItem
    {
        public readonly int baseGold;
        public readonly string cq;
        public readonly int depth;
        public readonly string description;

        public readonly int[] fromItems;
        public readonly string groups;

        public readonly int id;
        public readonly int[] intoItems;

        public readonly int[] maps;
        public readonly string name;
        public readonly string plaintext;

        public readonly bool purchasable;
        public readonly string requiredChampion;
        public readonly string sanitizedDescription;
        public readonly int sellGold;
        public readonly string[] tags;
        public readonly int totalGold;

        public LoLItem(string name, string description, string sanitizedDescription, string plaintext, int id,
            int baseGold, int totalGold, int sellGold, bool purchasable
            , string requiredChampion, int[] maps, int[] fromItems, int[] intoItems, int depth, string[] tags, string cq,
            string groups)
        {
            this.name = name;
            this.description = description;
            this.sanitizedDescription = sanitizedDescription;
            this.plaintext = plaintext;
            this.id = id;
            this.baseGold = baseGold;
            this.totalGold = totalGold;
            this.sellGold = sellGold;
            this.purchasable = purchasable;
            this.requiredChampion = requiredChampion;
            this.maps = maps;
            this.fromItems = fromItems;
            this.intoItems = intoItems;
            this.depth = depth;
            this.tags = tags;
            this.cq = cq;
            this.groups = groups;
        }

        public override string ToString()
        {
            return name;
        }

        public bool IsHealthlyConsumable()
        {
            return id == 2003 || id == 2009 || id == 2010;
        }
    }
}