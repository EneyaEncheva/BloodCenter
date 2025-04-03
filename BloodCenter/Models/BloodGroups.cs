namespace BloodCenter.Models
{
    public class BloodGroups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<BloodDonors> BloodDonors { get; set; } = 
            new List<BloodDonors>();
    }
}
