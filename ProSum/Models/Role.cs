using System;

namespace ProSum.Models
{
    public class Role
    {
        private Guid id;

        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }


        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value.Length > 0)
                {
                    name = value;
                }
            }
        }

        public Role()
        {

        }

        public Role(string name)
        {
            id = new Guid();
            this.name = name;
        }
    }

    public enum RolesEnum
    {
        Employee,
        Manager,
        Admin
    }
}
