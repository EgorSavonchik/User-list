﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserList.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}