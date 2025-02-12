namespace BloodCenter.Models
{
    public class BloodGroups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        ICollection<BloodDonors> BloodDonors { get; set; }
        ICollection<Requests> Requests { get; set; }
    }
}
