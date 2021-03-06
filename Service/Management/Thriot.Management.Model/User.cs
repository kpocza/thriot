﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Thriot.Framework;

namespace Thriot.Management.Model
{
    public class User : IEntity
    {
        [StringLength(32, MinimumLength = 32)]
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(128)]
        [Required]
        public string Email { get; set; }

        public bool Activated { get; set; }

        public string ActivationCode { get; set; }
    }
}