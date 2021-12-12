namespace IBL.BO
{/// <summary>
/// implement class CustomerInParcel
/// </summary>
    public class CustomerInParcel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"ID is {Id}\n";
            str += $"name is {Name}\n";
            return str;
        }
    }
}