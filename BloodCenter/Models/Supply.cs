﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class Supply
    {
        public int Id { get; set; }
        public int BloodGroupId { get; set; } // Връзка към кръвната група

        [ForeignKey("BloodGroupId")]
        public BloodGroups BloodGroup { get; set; }
        public char RhesusFactor { get; set; } // '+' или '-'
        public double Quantity { get; set; } // Количество в литри/милилитри
        public DateTime LastUpdated { get; set; } // Дата на последна актуализация

    }
}
