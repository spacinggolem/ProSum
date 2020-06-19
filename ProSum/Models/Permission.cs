using System;


namespace ProSum.Models
{

    public class Permission
    {
        private Guid id;
        public Guid Id { 
            get
            {
                return id;
            }
            set
            {
                if(id == Guid.Empty)
                {
                    id = value;
                }
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
                if (!string.IsNullOrEmpty(value))
                {
                    name = value;
                }
            }
        }

        public Permission(Guid id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public Permission()
        {
        }
    }
}
