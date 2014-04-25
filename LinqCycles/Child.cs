using System;

namespace LinqCycles
{
    public class Child
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ParentId { get; set; }
    }
}