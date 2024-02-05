using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mituzsuki.ConsoleUtilities
{
    class Student
    {
        public Guid StudentId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Max Powers";

        //Navigation props
        public virtual List<Subject> Subjects {get;set;}
    }

    class Subject
    {
        public Guid SubjectId { get; set; } = Guid.NewGuid();
        public Guid DepartmentId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Warhammer40k Lore";

        //Navigation props
        public virtual Department Department { get; set; }
    }

    class Department
    {
        public Guid DepartmentId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Lore";
    }

    class StudentSubject
    {
        public Guid StudentId { get; set; } = Guid.NewGuid();
        public Guid SubjectId { get; set; } = Guid.NewGuid();
    }
}
