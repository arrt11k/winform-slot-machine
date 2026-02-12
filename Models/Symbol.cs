namespace SlotMachineAdminSystem.Models
{
    public class Symbol
    {
        public string Name { get; set; }
        public int Multiplier { get; set; }
        public string ImagePath { get; set; }

        public Symbol(string name, int multiplier, string imagePath)
        {
            Name = name;
            Multiplier = multiplier;
            ImagePath = imagePath;
        }
    }
}
