using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LinqCycles
{
    public class LinqCyclesFixture
    {
        private readonly List<Child> _children;
        private readonly Dictionary<string, int> _childrenLog = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _parentLog = new Dictionary<string, int>();
        private readonly List<Parent> _parents;

        public LinqCyclesFixture()
        {
            _parents = GetParents(100).ToList();
            _children = GetChildren(_parents, 100).ToList();
        }

        [Fact]
        public void Profile_Enumerations_Using_Linq_Join()
        {
            var parents = new LoggingEnumerable<Parent>(_parents, ParentsLog);
            var children = new LoggingEnumerable<Child>(_children, ChildrenLog);

            IEnumerable<int> ptc = from p in parents
                join c in children
                    on p.Id equals c.ParentId
                select ComputeHash(p, c);

            List<int> codes = ptc.ToList();

            Console.WriteLine("{0} computations completed.", codes.Count);
            foreach (var kvp in _childrenLog)
            {
                Console.WriteLine("Children_{0}:{1}", kvp.Key, kvp.Value);
            }
            foreach (var kvp in _parentLog)
            {
                Console.WriteLine("Parent_{0}:{1}", kvp.Key, kvp.Value);
            }
        }

        [Fact]
        public void Profile_Enumerations_Using_Nested_Loops()
        {
            var parents = new LoggingEnumerable<Parent>(_parents, ParentsLog);
            var children = new LoggingEnumerable<Child>(_children, ChildrenLog);

            var codes = new List<int>();
            foreach (Parent parent in parents)
            {
                Guid parentId = parent.Id;
                IEnumerable<Child> ptc = children.Where(x => x.ParentId == parentId);
                foreach (Child child in ptc)
                {
                    int hash = ComputeHash(child, parent);
                    codes.Add(hash);
                }
            }

            Console.WriteLine("{0} computations completed.", codes.Count);
            foreach (var kvp in _childrenLog)
            {
                Console.WriteLine("Children_{0}:{1}", kvp.Key, kvp.Value);
            }
            foreach (var kvp in _parentLog)
            {
                Console.WriteLine("Parent_{0}:{1}", kvp.Key, kvp.Value);
            }
        }

        private int ComputeHash(object parent, object child)
        {
            return parent.GetHashCode() & child.GetHashCode();
        }

        private void ChildrenLog(string key)
        {
            if (!_childrenLog.ContainsKey(key))
                _childrenLog[key] = 0;
            _childrenLog[key]++;
        }

        private void ParentsLog(string key)
        {
            if (!_parentLog.ContainsKey(key))
                _parentLog[key] = 0;
            _parentLog[key]++;
        }

        private IEnumerable<Child> GetChildren(IEnumerable<Parent> parents, int count)
        {
            return parents
                .Select(
                    p => Enumerable.Range(0, count)
                        .Select(c => new Child
                        {
                            Id = Guid.NewGuid(),
                            ParentId = p.Id,
                            Name = string.Format("Child{0}_Of_Parent{1}", c, p)
                        }))
                .SelectMany(x => x.ToList());
        }

        private IEnumerable<Parent> GetParents(int count)
        {
            return Enumerable.Range(0, count)
                .Select(c => new Parent
                {
                    Id = Guid.NewGuid(),
                    Name = string.Format("Parent{0}", c)
                });
        }
    }
}